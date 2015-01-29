using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace StartAuction
{
    class StartAuction
    {
        private const string SERVER_NAME = "localhost";
        private const int NAMESPACE = 0;

        static void Main(string[] args)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(SERVER_NAME);
            IDatabase database = connection.GetDatabase(NAMESPACE);
            Console.WriteLine(database.Database);
            Console.ReadLine();
        }
    }
}
