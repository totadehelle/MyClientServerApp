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
        private const string EndOfMessageCommand = "<EOF>";
        private const string CloseCommand = "<CLOSE>";
        
        public SocketServer()
        {
            StartListening();
        }
        
        
        
        public void StartListening()
        {
            string data = null;
            byte[] bytes = new Byte[1024];
            
            IPAddress[] x  = Dns.GetHostAddresses(Dns.GetHostName());
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
                    Console.WriteLine("Waiting for a connection... at " + ipAddress + " at port 11009");

                    Socket handler = listener.Accept();
                    
                    string token = Guid.NewGuid().ToString();
                    Dictionary<string, string> tokenDict = new Dictionary<string, string>
                    {
                        ["token"] = token,
                    };
                    
                    string jsonToken = JsonConvert.SerializeObject(tokenDict, Formatting.Indented);

                    byte[] msg = Encoding.ASCII.GetBytes(jsonToken);

                    handler.Send(msg);
                    
                    
                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);  
                        string receivedData = Encoding.ASCII.GetString(bytes,0,bytesRec);
                        
                        if (receivedData.IndexOf(CloseCommand) > -1)
                        {
                            int firstIndex = receivedData.IndexOf(CloseCommand);
                            string[] messages = receivedData.Substring(0,firstIndex).Split(EndOfMessageCommand, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var message in messages)
                            {
                                data = data + message;
                                Console.WriteLine($"Text received : {data}");
                                data = null;
                            }

                            break;
                        }
            
                        if (receivedData.IndexOf(EndOfMessageCommand) > -1)
                        {
                            while (receivedData.IndexOf(EndOfMessageCommand) > -1)
                            {
                                int firstIndex = receivedData.IndexOf(EndOfMessageCommand);
                                int lastIndex = firstIndex+EndOfMessageCommand.Length;
                                data = data + receivedData.Substring(0, firstIndex);
                                Console.WriteLine($"Text received : {data}");
                                data = null;
                                receivedData = receivedData.Substring(lastIndex);
                            }
                            
                            data = data + receivedData;
                        }
                        
                        else
                        {
                            data = data + receivedData;
                        }
                        
                    }
                    
                    Console.WriteLine("Server closed the connection with the client.");
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