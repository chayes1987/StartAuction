using System;
using StackExchange.Redis;
using System.Configuration;

namespace Auction
{
    /// <summary>
    /// Database Manager Class
    /// </summary>
    static class DatabaseManager
    {
        /// <summary>
        /// Get Bidder E-mails
        /// </summary>
        /// <param name="id">The ID of the Auction</param>
        /// <returns>The e-mail addresses of the bidders registered for the auction</returns>
        public static string[] getBidderEmails(string id)
        {
            IDatabase database = null;

            // Connect to the database
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
            // Return all of the values in the set under the given key
            return Array.ConvertAll(database.SetMembers(id), x => (string)x);
        }
    }
}
