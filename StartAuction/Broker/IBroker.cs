// Author - Conor Hayes

namespace Auction.Broker
{
    /// <summary>
    /// IBroker
    /// </summary>
    public interface IBroker
    {
        /// <summary>
        /// Subscribe To Start Auction Command
        /// </summary>
        void subscribeToStartAuctionCmd();

        /// <summary>
        /// Subscribe To Notify Bidders Acknowledgement
        /// </summary>
        void subscribeToNotifyBiddersAck();

        /// <summary>
        /// Subscribe To Auction Started Acknowledgement
        /// </summary>
        void subscribeToAuctionStartedAck();

        /// <summary>
        /// Subscribe To Heartbeat
        /// </summary>
        void subscribeToHeartbeat();

        /// <summary>
        /// Publish Auction Started Event
        /// </summary>
        /// <param name="id">The Auction ID</param>
        void publishAuctionStartedEvent(string id);

        /// <summary>
        /// Publish Notify Bidders Command
        /// </summary>
        /// <param name="id">The Auction ID</param>
        /// <param name="emails">The bidders emails</param>
        void publishNotifyBiddersCommand(string id, string[] emails);

        /// <summary>
        /// Publish Acknowledgement
        /// </summary>
        /// <param name="message">The message to publish</param>
        void publishAcknowledgement(string message);
    }
}
