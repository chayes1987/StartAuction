using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartAuction
{
    public static class Constants {
        public const string SERVER_NAME = "localhost";
        public const int REDIS_NAMESPACE = 0;
        public const string START_AUCTION_TOPIC = "StartAuction";
        public const string AUCTION_STARTED_ACK_TOPIC = "ACK: AuctionStarted";
        public const string NOTIFY_BIDDERS_ACK_TOPIC = "ACK: NotifyBidders";
        public const string START_AUCTION_ACK_ADR = "tcp://127.0.0.1:1000";
        public const string START_AUCTION_ADR = "tcp://127.0.0.1:1001";
        public const string NOTIFY_BIDDERS_ACK_ADR = "tcp://127.0.0.1:1010";
        public const string PUB_ADR = "tcp://127.0.0.1:1011";
        public const string AUCTION_STARTED_ACK_ADR = "tcp://127.0.0.1:1100";
    }
}
