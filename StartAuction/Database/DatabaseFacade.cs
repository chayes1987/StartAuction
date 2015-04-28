using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auction.Utils;

namespace Auction.Database
{
    /// <summary>
    /// Database Facade
    /// </summary>
    public class DatabaseFacade
    {
        /// <summary>
        /// Database
        /// </summary>
        private static IDatabaseManager database;


        /// <summary>
        /// Get Database
        /// </summary>
        /// <returns>The Database</returns>
        public static IDatabaseManager GetDatabase()
        {
            if (Constants.DATABASE == DatabaseType.Redis)
            {
                database = new RedisDatabase();
            }
            //Others
            return database;
        }
    }
}
