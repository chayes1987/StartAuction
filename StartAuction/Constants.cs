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
        public const string NOTIFY_BIDDERS_ACK_TOPIC = "ACK: NotifyBidder";
        public const string START_AUCTION_ADR = "tcp://172.31.32.20:1001";
        public const string NOTIFY_BIDDERS_ACK_ADR = "tcp://172.31.32.22:1010";
        public const string PUB_ADR = "tcp://*:1011";
        public const string AUCTION_STARTED_ACK_ADR = "tcp://172.31.32.23:1100";
    }
}
