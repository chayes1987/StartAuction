using System.Threading;
using Auction.Broker;

/*
 *  The documentation was consulted on how to use the third party libraries 
 *  I also looked up how to use configuration files on StackOverflow
 *  Config -> http://stackoverflow.com/questions/10864755/adding-and-reading-from-a-config-file
 *  Coding Standards -> http://www.dofactory.com/reference/csharp-coding-standards
 */

namespace Auction
{
    /// <summary>
    /// Start Auction Class
    /// </summary>
    public class StartAuction
    {
        /// <summary>
        /// Main Function
        /// </summary>
        /// <param name="args">Command Line Args</param>
        static void Main(string[] args)
        {
            IBroker broker = BrokerFacade.GetBroker();

            // Initialize threads to subscribe to other commands and events
            new Thread(new ThreadStart(broker.subscribeToNotifyBiddersAck)).Start();
            new Thread(new ThreadStart(broker.subscribeToAuctionStartedAck)).Start();
            new Thread(new ThreadStart(broker.subscribeToHeartbeat)).Start();

            broker.subscribeToStartAuctionCmd();
        }

    }
}
