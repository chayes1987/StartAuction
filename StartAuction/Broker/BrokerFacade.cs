using Auction.Utils;

// Author - Conor Hayes

namespace Auction.Broker
{
    /// <summary>
    /// Broker Facade
    /// </summary>
    public class BrokerFacade
    {
        /// <summary>
        /// Broker
        /// </summary>
        private static IBroker broker;


        /// <summary>
        /// Get Broker
        /// </summary>
        /// <returns>The Broker</returns>
        public static IBroker GetBroker()
        {
            if (Constants.BROKER == MessageBroker.ZeroMq)
            {
                broker = new ZeroMqBroker();
            }
            // Other Brokers may be added
            return broker;
        }

    }
}
