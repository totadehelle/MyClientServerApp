using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MyClientServerApp
{
    public class MultiThreadSocketServer
    {
        Dictionary<string, NetworkStream> clients = new Dictionary<string, NetworkStream>();
        const int PORT_FOR_CLIENTS = 11009;
        const int PORT_FOR_HTTP_SERVER = 11000;
        private TcpListener listener;
        
        public void StartListeningToClients()
        {
            try
            {
                IPAddress[] x  = Dns.GetHostAddresses(Dns.GetHostName());
                var ipAddress = new IPAddress(x[0].GetAddressBytes());
                
                listener = new TcpListener(ipAddress, PORT_FOR_CLIENTS);
                listener.Start();
                Console.WriteLine("Socket-server is waiting for a connection... at " + ipAddress + " at port " + PORT_FOR_CLIENTS);
                while(true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);
 
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                    Dictionary<string, NetworkStream> clientId = clientObject.GetClientId();
                    Console.WriteLine(clientId.Count);
                    clients.Add(clientId.ElementAt(0).Key, clientId.ElementAt(0).Value);
                    Console.WriteLine(clients.ElementAt(0).Key.ToString(), clients.ElementAt(0).Value);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if(listener!=null)
                    listener.Stop();
            }
        }

        public void StartListeningToHttpServer()
        {
            try
            {
                IPAddress[] x  = Dns.GetHostAddresses(Dns.GetHostName());
                var ipAddress = new IPAddress(x[0].GetAddressBytes());
                
                listener = new TcpListener(ipAddress, PORT_FOR_HTTP_SERVER);
                listener.Start();
                Console.WriteLine("Socket-server is waiting for a connection with Http-server... at " + ipAddress + " at port " + PORT_FOR_HTTP_SERVER);
                while(true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream stream = null;
                    try
                    {
                        stream = client.GetStream();
                
                        string data = null;
                        byte[] bytes = new byte[1024]; // буфер для получаемых данных
                        
                        int bytesRec = stream.Read(bytes);  
                        data = Encoding.ASCII.GetString(bytes,0,bytesRec);
                        Console.WriteLine("Socket-server says: Data form the HTTP-Server received: {0}", data);
                        
                        
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        Console.WriteLine("Socket-server says: Server closed the connection with the HTTP-server.");
                
                        if (stream != null)
                            stream.Close();
                        if (client != null)
                            client.Close();
                    }

                }
                    
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if(listener!=null)
                    listener.Stop();
            }
        }
        
    }
}