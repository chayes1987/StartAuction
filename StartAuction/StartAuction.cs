using System;
using System.Text;
using StackExchange.Redis;
using NetMQ;

/*
 *  The documentation was consulted on how to use both third party libraries for Redis and 0mq  
 *  0mq -> https://github.com/zeromq/netmq
 *  Redis -> https://github.com/StackExchange/StackExchange.Redis/blob/master/Docs/Basics.md
*/

namespace StartAuction
{
    class StartAuction
    {
        private const string SUBSCRIBER_ADDRESS = "tcp://127.0.0.1:1000", PUBLISHER_ADDRESS = "tcp://127.0.0.1:1001",
            TOPIC = "StartAuction", SERVER_NAME = "localhost";        
        private const int NAMESPACE = 0;
        private NetMQContext context = NetMQContext.Create();

        static void Main(string[] args) {
            new StartAuction().subscribe();
        }

        private void subscribe() {
            var subscriber = context.CreateSubscriberSocket();
            subscriber.Connect(SUBSCRIBER_ADDRESS);
            subscriber.Subscribe(TOPIC);
            Console.WriteLine("Subscribed to " + TOPIC + " command...");
            var publisher = context.CreatePublisherSocket();
            publisher.Bind(PUBLISHER_ADDRESS);

            while (true) {
                string command = subscriber.ReceiveString();
                Console.WriteLine("Received command: " + command);

                string id = parseMessage(command, "<id>", "</id>");
                string[] emails = getBidderEmails(id);

                if (emails != null) {
                    publishNotifyBiddersCommand(id, publisher, emails);
                    publishAuctionStartedEvent(id, publisher);
                }
            }
        }

        private void publishAuctionStartedEvent(string id, NetMQ.Sockets.PublisherSocket publisher) {
            string message = "AuctionStarted <id>" + id + "</id>";
            publisher.Send(message);
            Console.WriteLine("Published " + message + " event...");
        }

        private void publishNotifyBiddersCommand(string id, NetMQ.Sockets.PublisherSocket publisher, string[] emails) {
            StringBuilder addresses = new StringBuilder();

            foreach (string address in emails) {
                addresses.Append(address + ";");
            }

            string message = "NotifyBidder <id>" + id + "</id>" + " <params>" + addresses.ToString().Substring(0, addresses.ToString().Length - 1) + "</params>";
            publisher.Send(message);
            Console.WriteLine("Published " + message + " command");
        }

        private string[] getBidderEmails(string id) {
            IDatabase database = null;

            try {
                ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(SERVER_NAME);
                database = connection.GetDatabase(NAMESPACE);
            }
            catch (RedisConnectionException e) {
                Console.WriteLine("Could not connect to database - " + e.Message);
                return null;
            }

            return Array.ConvertAll(database.SetMembers(id), x => (string)x);
        }

        private string parseMessage(string message, string startTag, string endTag) {
            int startIndex = message.IndexOf(startTag) + startTag.Length;
            string substring = message.Substring(startIndex);
            return substring.Substring(0, substring.LastIndexOf(endTag));
        }
    }
}
