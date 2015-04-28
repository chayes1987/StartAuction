using Auction.Broker;
using Auction.Database;

namespace Auction.Utils
{
    public class Constants
    {
        public static MessageBroker BROKER = MessageBroker.ZeroMq;
        public static DatabaseType DATABASE = DatabaseType.Redis;
    }
}
