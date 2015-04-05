using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Auction;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;

namespace TestStartAuction
{

    // Ref -> http://stackoverflow.com/questions/9122708/unit-testing-private-methods-in-c-sharp

    [TestClass]
    public class StartAuctionTests
    {
        private NetMQContext _context = NetMQContext.Create();

        [TestMethod]
        public void TestParseMessage()
        {
            StartAuction auction = new StartAuction();
            PrivateObject obj = new PrivateObject(auction);
            String[] args = {"StartAuction <id>1</id>", "<id>", "</id>"};
            var retVal = obj.Invoke("parseMessage", args);
            Assert.AreEqual("1", retVal);
            args = new String[] {"#Hello#&World&", "#", "&"};
            retVal = obj.Invoke("parseMessage", args);
            Assert.AreEqual("Hello#&World", retVal);
            Assert.AreNotEqual("Hello#", retVal);
        }

        [TestMethod]
        public void TestPubSub()
        {
            PublisherSocket publisher = _context.CreatePublisherSocket();
            publisher.Bind("tcp://127.0.0.1:9999");
            Thread.Sleep(1000);

            Thread subscriber = new Thread(new ThreadStart(subscribe));
            subscriber.Start();

            publisher.Send("Test");
            Thread.Sleep(1000);
            subscriber.Abort();
        }

        private void subscribe()
        {
            SubscriberSocket subscriber = _context.CreateSubscriberSocket();
            subscriber.Connect("tcp://127.0.0.1:9999");
            subscriber.Subscribe("Test");
            string message = String.Empty;

            while (message == String.Empty)
            {
                message = subscriber.ReceiveString();
            }
            Console.WriteLine(message);
            Assert.AreEqual("Test", message);
        }
    }
}
