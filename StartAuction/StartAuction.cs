using System;
using System.Text;
using StackExchange.Redis;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using System.Configuration;

/*
 *  The documentation was consulted on how to use both third party libraries for Redis and 0mq  
 *  I also looked up how to use configuration files on StackOverflow
 *  0mq -> https://github.com/zeromq/netmq
 *  Redis -> https://github.com/StackExchange/StackExchange.Redis/blob/master/Docs/Basics.md
 *  Config -> http://stackoverflow.com/questions/10864755/adding-and-reading-from-a-config-file
*/

namespace Auction
{
    public class StartAuction { 
        private NetMQContext _context = NetMQContext.Create();
        private PublisherSocket _publisher;

        static void Main(string[] args) { new StartAuction().subscribeToStartAuction(); }

        private void subscribeToStartAuction() {
            _publisher = _context.CreatePublisherSocket();
            _publisher.Bind(ConfigurationManager.AppSettings["pubAddr"]);

            var startAuctionSub = _context.CreateSubscriberSocket();
            var startAuctionTopic = ConfigurationManager.AppSettings["startAuctionTopic"];
            startAuctionSub.Connect(ConfigurationManager.AppSettings["startAuctionAddr"]);
            startAuctionSub.Subscribe(startAuctionTopic);
            Console.WriteLine("SUB: " + startAuctionTopic);

            new Thread(new ThreadStart(subToNotifyBiddersAck)).Start();
            new Thread(new ThreadStart(subToAuctionStartedAck)).Start();
            new Thread(new ThreadStart(subToHeartbeat)).Start();

            while (true) {
                string startAuctionCmd = startAuctionSub.ReceiveString();
                Console.WriteLine("REC: " + startAuctionCmd);
                publishAcknowledgement(startAuctionCmd);
                string id = parseMessage(startAuctionCmd, "<id>", "</id>");
                string[] emails = getBidderEmails(id);

                if (emails != null)
                    publishNotifyBiddersCommand(id, emails);
                    publishAuctionStartedEvent(id);
            }
        }

        private void publishAuctionStartedEvent(string id) {
            string auctionStartedEvent = string.Concat(
                ConfigurationManager.AppSettings["auctionStartedTopic"], " <id>", id, "</id>");
            publish(auctionStartedEvent);
        }

        private void publishNotifyBiddersCommand(string id, string[] emails) {
            StringBuilder bidderEmails = new StringBuilder();

            foreach (string address in emails)
                bidderEmails.Append(address + ";");

            string notifyBiddersCmd = string.Concat(
                ConfigurationManager.AppSettings["notifyBiddersTopic"],
                " <id>", id, "</id>", " <params>", bidderEmails.ToString().Substring(0,
                bidderEmails.ToString().Length - 1), "</params>");
            publish(notifyBiddersCmd);
        }

        private string[] getBidderEmails(string id) {
            IDatabase database = null;

            try {
                ConnectionMultiplexer redisConn = 
                    ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["serverName"]);
                database = 
                    redisConn.GetDatabase(Int32.Parse(ConfigurationManager.AppSettings["namespace"]));
            } catch (RedisConnectionException e) {
                Console.WriteLine("Could not connect to database - " + e.Message);
                return null;
            }
            return Array.ConvertAll(database.SetMembers(id), x => (string)x);
        }

        private string parseMessage(string message, string startTag, string endTag) {
            int startIndex = message.IndexOf(startTag) + startTag.Length;
            string substring = message.Substring(startIndex);
            return substring.Substring(0, substring.LastIndexOf(endTag));
        }

        private void publishAcknowledgement(string message) {
            string ack = string.Concat("ACK ", message);
            publish(ack);
        }

        private void subToNotifyBiddersAck() {
            var notifyBiddersAckSub = _context.CreateSubscriberSocket();
            notifyBiddersAckSub.Connect(
                ConfigurationManager.AppSettings["notifyBiddersAckAddr"]);
            notifyBiddersAckSub.Subscribe(
                ConfigurationManager.AppSettings["notifyBiddersAckTopic"]);

            while (true) Console.WriteLine("REC: " + notifyBiddersAckSub.ReceiveString());
        }

        private void subToAuctionStartedAck() {
            var auctionStartedAckSub = _context.CreateSubscriberSocket();
            auctionStartedAckSub.Connect(
                ConfigurationManager.AppSettings["auctionStartedAckAddr"]);
            auctionStartedAckSub.Subscribe(
                ConfigurationManager.AppSettings["auctionStartedAckTopic"]);

            while (true) Console.WriteLine("REC: " + auctionStartedAckSub.ReceiveString());
        }

        private void publish(string message) {
            _publisher.Send(message);
            Console.WriteLine("PUB: " + message);
        }

        private void subToHeartbeat() {
            var heartbeatSub = _context.CreateSubscriberSocket();
            heartbeatSub.Connect(ConfigurationManager.AppSettings["heartbeatSubAddr"]);
            heartbeatSub.Subscribe(ConfigurationManager.AppSettings["checkHeartbeatTopic"]);

            while (true) {
                Console.WriteLine("REC: " + heartbeatSub.ReceiveString());
                string message = String.Concat(ConfigurationManager.AppSettings["checkHeartbeatTopicResponse"], " <params>",
                    ConfigurationManager.AppSettings["serviceName"], "</params>");
                _publisher.Send(message);
                Console.WriteLine("PUB: " + message);
            }
        }
    }
}
