using System;
using StackExchange.Redis;
using System.Configuration;

// Author - Conor Hayes

/*
 *  The documentation was consulted on how to use the third party libraries
 *  I also looked up how to use configuration files on StackOverflow
 *  Redis -> https://github.com/StackExchange/StackExchange.Redis/blob/master/Docs/Basics.md
 *  Config -> http://stackoverflow.com/questions/10864755/adding-and-reading-from-a-config-file
 *  Coding Standards -> http://www.dofactory.com/reference/csharp-coding-standards
 */

namespace Auction.Database
{
    /// <summary>
    /// Redis Database
    /// </summary>
    public class RedisDatabase : IDatabaseManager
    {
        /// <summary>
        /// Get Bidder E-mails
        /// </summary>
        /// <param name="id">The ID of the Auction</param>
        /// <returns>The e-mail addresses of the bidders registered for the auction</returns>
        public string[] getBidderEmails(string id)
        {
            IDatabase database = null;

            // Connect to the database
            try
            {
                ConnectionMultiplexer redisConn =
                    ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["serverName"]);
                database = redisConn.GetDatabase(Int32.Parse(ConfigurationManager.AppSettings["namespace"]));
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
