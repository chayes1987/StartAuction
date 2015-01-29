using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using NetMQ;

namespace StartAuction
{
    class StartAuction
    {
        private const string SUBSCRIBER_ADDRESS = "tcp://127.0.0.1:1000", TOPIC = "StartAuction", SERVER_NAME = "localhost";
        private const int NAMESPACE = 0;
        private NetMQContext context = NetMQContext.Create();

        static void Main(string[] args) {
            new StartAuction().subscribe();
        }

        private void subscribe()
        {
            var subscriber = context.CreateSubscriberSocket();
            subscriber.Connect(SUBSCRIBER_ADDRESS);
            subscriber.Subscribe(TOPIC);
            Console.WriteLine("Subscribed to " + TOPIC + " command...");

            while (true)
            {
                string command = subscriber.ReceiveString();
                Console.WriteLine("Received command: " + command);
                string id = parseMessage(command, "<id>", "</id>");
                string[] emails = getBidderEmails(id);
            }
        }

        private string[] getBidderEmails(string id)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(SERVER_NAME);
            IDatabase database = connection.GetDatabase(NAMESPACE);
            RedisValue[] addresses = database.SetMembers(id);
            return Array.ConvertAll(addresses, x => (string)x);
        }

        private string parseMessage(string message, string startTag, string endTag)
        {
            int startIndex = message.IndexOf(startTag) + startTag.Length;
            string substring = message.Substring(startIndex);
            return substring.Substring(0, substring.LastIndexOf(endTag));
        }
    }
}
