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
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(SERVER_NAME);
            IDatabase database = connection.GetDatabase(NAMESPACE);
            Console.WriteLine(database.Database);

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
            }
        }
    }
}
