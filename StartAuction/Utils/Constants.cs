using Auction.Broker;
using Auction.Database;

// Author - Conor Hayes

namespace Auction.Utils
{
    /// <summary>
    /// Constants
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Broker
        /// </summary>
        public static MessageBroker BROKER = MessageBroker.ZeroMq;
        
        /// <summary>
        /// Database
        /// </summary>
        public static DatabaseType DATABASE = DatabaseType.Redis;
    }
}
