using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
