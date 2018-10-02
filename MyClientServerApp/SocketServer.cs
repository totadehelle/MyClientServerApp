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
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");

                    Socket handler = listener.Accept();


                    string token = Guid.NewGuid().ToString();
                    Dictionary<string, string> tokenDict = new Dictionary<string, string>
                    {
                        ["Token"] = token,
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