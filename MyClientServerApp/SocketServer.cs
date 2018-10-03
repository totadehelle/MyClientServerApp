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
        
        // Incoming data from the client.  
        public static string data = null;

        public void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];
            
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
                    Console.WriteLine("Waiting for a connection... at " + ipAddress + " at port 11009");

                    Socket handler = listener.Accept();
                    data = null;
                    
                    string token = Guid.NewGuid().ToString();
                    Dictionary<string, string> tokenDict = new Dictionary<string, string>
                    {
                        ["token"] = token,
                    };
                    
                    string jsonToken = JsonConvert.SerializeObject(tokenDict, Formatting.Indented);

                    byte[] msg = Encoding.ASCII.GetBytes(jsonToken);

                    handler.Send(msg);
                    
                    
                    // An incoming connection needs to be processed.  
                    while (true) {  
                        int bytesRec = handler.Receive(bytes);  
                        data += Encoding.ASCII.GetString(bytes,0,bytesRec);
                        if (data.IndexOf("<CLOSE>") > -1)
                        {
                            Console.WriteLine($"Text received : {data}");
                            Console.WriteLine("Server closed the connection with the client.");
                            break;
                        }
                        
                        if (data.IndexOf("<EOF>") > -1) 
                        {  
                            Console.WriteLine($"Text received : {data}");
                            data = null;
                        }
                        
                    }
                    
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