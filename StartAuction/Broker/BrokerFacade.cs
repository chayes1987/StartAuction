﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auction.Utils;

namespace Auction.Broker
{
    /// <summary>
    /// Broker Facade
    /// </summary>
    public class BrokerFacade
    {
        /// <summary>
        /// Broker
        /// </summary>
        private static IBroker broker;


        /// <summary>
        /// Get Broker
        /// </summary>
        /// <returns>The Broker</returns>
        public static IBroker GetBroker()
        {
            if (Constants.BROKER == MessageBroker.ZeroMq)
            {
                broker = new ZeroMqBroker();
            }
            // Other Brokers may be added
            return broker;
        }

    }
}
