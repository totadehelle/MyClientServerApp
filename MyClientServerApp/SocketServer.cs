using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace MyClientServerApp
{
    class SocketServer
    {
        public SocketServer()
        {
            StartListening();
        }

        public void StartListening()
        {
            IPAddress[] x  = Dns.GetHostAddresses(Dns.GetHostName());
            //var y = x.t
            //IPAddress ipAddress = //ipHostInfo.AddressList[0];
            var ipAddress = new IPAddress(x[0].GetAddressBytes());
           

             
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11009);

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection... at, " + ipAddress + "at port 11009");

                    Socket handler = listener.Accept();
                    Console.WriteLine("Connection started!");

                    string token = Guid.NewGuid().ToString();
                    Dictionary<string, string> tokenDict = new Dictionary<string, string>
                    {
                        ["token"] = token,
                    };
                    
                    string jsonToken = JsonConvert.SerializeObject(tokenDict, Formatting.Indented);

                    byte[] msg = Encoding.ASCII.GetBytes(jsonToken);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }
    }
}