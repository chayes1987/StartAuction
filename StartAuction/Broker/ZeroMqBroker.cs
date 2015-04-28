using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;
using System.Configuration;
using Auction.Utils;
using Auction.Database;

/*  0mq -> https://github.com/zeromq/netmq */

namespace Auction.Broker
{
    /// <summary>
    /// Zero Mq Broker Implements IBroker
    /// </summary>
    public class ZeroMqBroker : IBroker
    {
        /// <summary>
        /// Context
        /// </summary>
        private NetMQContext _context = NetMQContext.Create();

        /// <summary>
        /// Publisher
        /// </summary>
        private PublisherSocket _publisher;


        #region Subscriber Code

        /// <summary>
        /// Subscribe To Start Auction
        /// </summary>
        public void subscribeToStartAuctionCmd()
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

            while (true)
            {
                string startAuctionCmd = startAuctionSub.ReceiveString();
                Console.WriteLine("REC: " + startAuctionCmd);
                publishAcknowledgement(startAuctionCmd);
                // Extract the ID and get the bidders emails
                string id = MessageParser.parseMessage(startAuctionCmd, "<id>", "</id>");
                IDatabaseManager database = DatabaseFacade.GetDatabase();
                string[] emails = database.getBidderEmails(id);

                if (emails != null)
                    publishNotifyBiddersCommand(id, emails);
                publishAuctionStartedEvent(id);
            }
        }

        /// <summary>
        /// Subscribe To Notify Bidders Acknowledgement
        /// </summary>
        public void subscribeToNotifyBiddersAck()
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
        public void subscribeToAuctionStartedAck()
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
        public void subscribeToHeartbeat()
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
        public void publishAuctionStartedEvent(string id)
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
        public void publishNotifyBiddersCommand(string id, string[] emails)
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
        public void publishAcknowledgement(string message)
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
