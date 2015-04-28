using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using Auction.Utils;

/*
 *  The documentation was consulted on how to use the third party libraries  
 *  0mq -> https://github.com/zeromq/netmq
 *  Testing Private Methods -> http://stackoverflow.com/questions/9122708/unit-testing-private-methods-in-c-sharp
 *  Coding Standards -> http://www.dofactory.com/reference/csharp-coding-standards
 */

namespace TestStartAuction
{
    /// <summary>
    /// Start Auction Tests
    /// </summary>
    [TestClass]
    public class StartAuctionTests
    {
        /// <summary>
        /// Context
        /// </summary>
        private NetMQContext _context = NetMQContext.Create();


        /// <summary>
        /// Test Parse Message
        /// </summary>
        [TestMethod]
        public void TestParseMessage()
        {
            Assert.AreEqual("1", MessageParser.parseMessage("StartAuction <id>1</id>", "<id>", "</id>"));
            Assert.AreEqual("Hello#&World", MessageParser.parseMessage("#Hello#&World&", "#", "&"));
            Assert.AreNotEqual("Hello#", MessageParser.parseMessage("#Hello#&World&", "#", "&"));
        }

        /// <summary>
        /// Test Publish/Subscribe
        /// </summary>
        [TestMethod]
        public void TestPubSub()
        {
            // Create publisher and bind it to a localhost address and port
            PublisherSocket publisher = _context.CreatePublisherSocket();
            publisher.Bind("tcp://127.0.0.1:9999");
            // Allow time to bind
            Thread.Sleep(1000);

            // Create a thread to subscribe to the published message
            Thread subscriber = new Thread(new ThreadStart(subscribe));
            subscriber.Start();

            // Publish a test message
            publisher.Send("Test");
            Thread.Sleep(1000);
            // Kill the thread
            subscriber.Abort();
        }

        /// <summary>
        /// Subscribe
        /// </summary>
        private void subscribe()
        {
            // Create a subscriber and connect to the same address and port as the publisher
            SubscriberSocket subscriber = _context.CreateSubscriberSocket();
            subscriber.Connect("tcp://127.0.0.1:9999");
            // Set the topic
            subscriber.Subscribe("Test");
            string message = String.Empty;

            // Receive the message and perform the assertion
            while (message == String.Empty)
            {
                message = subscriber.ReceiveString();
            }
            Console.WriteLine(message);
            Assert.AreEqual("Test", message);
        }
    }
}
