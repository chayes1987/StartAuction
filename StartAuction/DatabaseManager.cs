using System;
using System.Collections.Generic;
using StackExchange.Redis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Auction
{
    static class DatabaseManager
    {
        public static string[] getBidderEmails(string id)
        {
            IDatabase database = null;
            try
            {
                ConnectionMultiplexer redisConn =
                    ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["serverName"]);
                database =
                    redisConn.GetDatabase(Int32.Parse(ConfigurationManager.AppSettings["namespace"]));
            }
            catch (RedisConnectionException e)
            {
                Console.WriteLine("Could not connect to database - " + e.Message);
                return null;
            }
            return Array.ConvertAll(database.SetMembers(id), x => (string)x);
        }
    }
}
