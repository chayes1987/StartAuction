using Auction.Broker;
using Auction.Database;

// Author - Conor Hayes

namespace Auction.Utils
{
    public class Constants
    {
        public static MessageBroker BROKER = MessageBroker.ZeroMq;
        public static DatabaseType DATABASE = DatabaseType.Redis;
    }
}
