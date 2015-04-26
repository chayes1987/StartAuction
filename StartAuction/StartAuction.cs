using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using System.Configuration;

/*
 *  The documentation was consulted on how to use the third party libraries for Redis and 0mq  
 *  I also looked up how to use configuration files on StackOverflow
 *  0mq -> https://github.com/zeromq/netmq
 *  Redis -> https://github.com/StackExchange/StackExchange.Redis/blob/master/Docs/Basics.md
 *  Config -> http://stackoverflow.com/questions/10864755/adding-and-reading-from-a-config-file
 *  Coding Standards -> http://www.dofactory.com/reference/csharp-coding-standards
 */

namespace Auction
{
    /// <summary>
    /// Start Auction Class
    /// </summary>
    public class StartAuction {
        /// <summary>
        /// Context
        /// </summary>
        private NetMQContext _context = NetMQContext.Create();

        /// <summary>
        /// Publisher
        /// </summary>
        private PublisherSocket _publisher;


        /// <summary>
        /// Main Function
        /// </summary>
        /// <param name="args">Command Line Args</param>
        static void Main(string[] args) { new StartAuction().subscribeToStartAuction(); }

        /// <summary>
        /// Parse Message
        /// </summary>
        /// <param name="message">The String to be parsed</param>
        /// <param name="startTag">The starting delimiter</param>
        /// <param name="endTag">The ending delimiter</param>
        /// <returns>The required string</returns>
        private string parseMessage(string message, string startTag, string endTag)
        {
            int startIndex = message.IndexOf(startTag) + startTag.Length;
            string substring = message.Substring(startIndex);
            return substring.Substring(0, substring.LastIndexOf(endTag));
        }

        #region Subscriber Code

        /// <summary>
        /// Subscribe To Start Auction
        /// </summary>
        private void subscribeToStartAuction()
        {
            // Bind the publisher to the address
            _publisher = _context.CreatePublisherSocket();
            _publisher.Bind(ConfigurationManager.AppSettings["pubAddr"]);

            // Connect to address for StartAuction and subscribe to the topic - StartAuction
            var startAuctionSub = _context.CreateSubscriberSocket();
            var startAuctionTopic = ConfigurationManager.AppSettings["startAuctionTopic"];
            startAuctionSub.Connect(ConfigurationManager.AppSettings["startAuctionAddr"]);
            startAuctionSub.Subscribe(startAuctionTopic);
            Console.WriteLine("SUB: " + startAuctionTopic);

            // Initialize threads to subscribe to other commands and events
            new Thread(new ThreadStart(subToNotifyBiddersAck)).Start();
            new Thread(new ThreadStart(subToAuctionStartedAck)).Start();
            new Thread(new ThreadStart(subToHeartbeat)).Start();

            while (true)
            {
                string startAuctionCmd = startAuctionSub.ReceiveString();
                Console.WriteLine("REC: " + startAuctionCmd);
                publishAcknowledgement(startAuctionCmd);
                // Extract the ID and get the bidders emails
                string id = parseMessage(startAuctionCmd, "<id>", "</id>");
                string[] emails = DatabaseManager.getBidderEmails(id);

                if (emails != null)
                    publishNotifyBiddersCommand(id, emails);
                publishAuctionStartedEvent(id);
            }
        }

        /// <summary>
        /// Subscribe To Notify Bidders Acknowledgement
        /// </summary>
        private void subToNotifyBiddersAck()
        {
            // Create the socket, connect to it, and subscribe to the topic - ACK NotifyBidders 
            var notifyBiddersAckSub = _context.CreateSubscriberSocket();
            notifyBiddersAckSub.Connect(
                ConfigurationManager.AppSettings["notifyBiddersAckAddr"]);
            notifyBiddersAckSub.Subscribe(
                ConfigurationManager.AppSettings["notifyBiddersAckTopic"]);

            while (true) Console.WriteLine("REC: " + notifyBiddersAckSub.ReceiveString());
        }

        /// <summary>
        /// Subscribe To Auction Started Acknowledgement
        /// </summary>
        private void subToAuctionStartedAck()
        {
            // Create the socket, connect to it, and subscribe to the topic - ACK AuctionStarted
            var auctionStartedAckSub = _context.CreateSubscriberSocket();
            auctionStartedAckSub.Connect(
                ConfigurationManager.AppSettings["auctionStartedAckAddr"]);
            auctionStartedAckSub.Subscribe(
                ConfigurationManager.AppSettings["auctionStartedAckTopic"]);

            while (true) Console.WriteLine("REC: " + auctionStartedAckSub.ReceiveString());
        }

        /// <summary>
        /// Subscribe To Heartbeat
        /// </summary>
        private void subToHeartbeat()
        {
            // Create the socket, connect to it, and subscribe to the topic - CheckHeartbeat
            var heartbeatSub = _context.CreateSubscriberSocket();
            heartbeatSub.Connect(ConfigurationManager.AppSettings["heartbeatSubAddr"]);
            heartbeatSub.Subscribe(ConfigurationManager.AppSettings["checkHeartbeatTopic"]);

            while (true)
            {
                Console.WriteLine("REC: " + heartbeatSub.ReceiveString());
                // Build and send the response to the check
                string message = String.Concat(ConfigurationManager.AppSettings["checkHeartbeatTopicResponse"], " <params>",
                    ConfigurationManager.AppSettings["serviceName"], "</params>");
                _publisher.Send(message);
                Console.WriteLine("PUB: " + message);
            }
        }

        #endregion

        #region Publisher Code

        /// <summary>
        /// Publish Auction Started Event
        /// </summary>
        /// <param name="id">The ID of the Auction</param>
        private void publishAuctionStartedEvent(string id)
        {
            string auctionStartedEvent = string.Concat(
                ConfigurationManager.AppSettings["auctionStartedTopic"], " <id>", id, "</id>");
            publish(auctionStartedEvent);
        }

        /// <summary>
        /// Publish Notify Bidders Command
        /// </summary>
        /// <param name="id">The ID of the Auction</param>
        /// <param name="emails">The e-mails of the bidders</param>
        private void publishNotifyBiddersCommand(string id, string[] emails)
        {
            StringBuilder bidderEmails = new StringBuilder();

            // Build a string of the emails to send
            foreach (string address in emails)
                bidderEmails.Append(address + ";");

            string notifyBiddersCmd = string.Concat(
                ConfigurationManager.AppSettings["notifyBiddersTopic"],
                " <id>", id, "</id>", " <params>", bidderEmails.ToString().Substring(0,
                bidderEmails.ToString().Length - 1), "</params>");
            publish(notifyBiddersCmd);
        }

        /// <summary>
        /// Publish Acknowledgement
        /// </summary>
        /// <param name="message">The message that was receieved to prefix with ACK</param>
        private void publishAcknowledgement(string message)
        {
            string ack = string.Concat("ACK ", message);
            publish(ack);
        }

        /// <summary>
        /// Publish
        /// </summary>
        /// <param name="message">The message to publish</param>
        private void publish(string message)
        {
            _publisher.Send(message);
            Console.WriteLine("PUB: " + message);
        }

        #endregion
    }
}
