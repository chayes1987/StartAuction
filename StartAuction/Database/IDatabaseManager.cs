using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction.Database
{
    /// <summary>
    /// Database Manager Interface
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        /// Get Bidder Emails
        /// </summary>
        /// <param name="id">The ID of the auction</param>
        /// <returns>The bidders emails</returns>
        string[] getBidderEmails(string id);
    }
}
