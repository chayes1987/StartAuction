using Auction.Utils;

// Author - Conor Hayes

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
        private static IDatabase database;


        /// <summary>
        /// Get Database
        /// </summary>
        /// <returns>The Database</returns>
        public static IDatabase GetDatabase()
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
