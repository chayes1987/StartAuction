using System;
using System.Text;
using StackExchange.Redis;
using NetMQ;
using System.Threading;

/*
 *  The documentation was consulted on how to use both third party libraries for Redis and 0mq  
 *  0mq -> https://github.com/zeromq/netmq
 *  Redis -> https://github.com/StackExchange/StackExchange.Redis/blob/master/Docs/Basics.md
*/

namespace StartAuction
{
    class StartAuction { 
        private NetMQContext context = NetMQContext.Create();
        private NetMQ.Sockets.PublisherSocket publisher;

        static void Main(string[] args) { new StartAuction().subscribeToStartAuction(); }

        private void subscribeToStartAuction() {
            publisher = context.CreatePublisherSocket();
            publisher.Bind(Constants.PUB_ADR);

            var startAuctionSub = context.CreateSubscriberSocket();
            startAuctionSub.Connect(Constants.START_AUCTION_ADR);
            startAuctionSub.Subscribe(Constants.START_AUCTION_TOPIC);
            Console.WriteLine("SUB: " + Constants.START_AUCTION_TOPIC + " command");

            new Thread(new ThreadStart(subToNotifyBiddersAck)).Start();
            new Thread(new ThreadStart(subToAuctionStartedAck)).Start();

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
            string auctionStartedEvent = "AuctionStarted <id>" + id + "</id>";
            publisher.Send(auctionStartedEvent);
            Console.WriteLine("PUB: " + auctionStartedEvent + "\n");
        }

        private void publishNotifyBiddersCommand(string id, string[] emails) {
            StringBuilder bidderEmails = new StringBuilder();

            foreach (string address in emails)
                bidderEmails.Append(address + ";");

            string notifyBiddersCmd = "NotifyBidder <id>" + id + "</id>" + " <params>" + bidderEmails.ToString().Substring(0, bidderEmails.ToString().Length - 1) + "</params>";
            publisher.Send(notifyBiddersCmd);
            Console.WriteLine("PUB: " + notifyBiddersCmd);
        }

        private string[] getBidderEmails(string id) {
            IDatabase database = null;

            try {
                ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(Constants.SERVER_NAME);
                database = connection.GetDatabase(Constants.REDIS_NAMESPACE);
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
            publisher.Send("ACK: " + message);
            Console.WriteLine("ACK SENT...");
        }

        private void subToNotifyBiddersAck() {
            var notifyBiddersAckSub = context.CreateSubscriberSocket();
            notifyBiddersAckSub.Connect(Constants.NOTIFY_BIDDERS_ACK_ADR);
            notifyBiddersAckSub.Subscribe(Constants.NOTIFY_BIDDERS_ACK_TOPIC);

            while (true)
                Console.WriteLine(notifyBiddersAckSub.ReceiveString());
        }

        private void subToAuctionStartedAck() {
            var auctionStartedAckSub = context.CreateSubscriberSocket();
            auctionStartedAckSub.Connect(Constants.AUCTION_STARTED_ACK_ADR);
            auctionStartedAckSub.Subscribe(Constants.AUCTION_STARTED_ACK_TOPIC);

            while (true)
                Console.WriteLine(auctionStartedAckSub.ReceiveString());
        }
    }
}
