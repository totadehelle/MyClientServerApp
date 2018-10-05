using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MyClientServerApp
{
    public class MultiThreadSocketServer
    {
        public static ConcurrentDictionary<string, NetworkStream> clients = //list of all active ClientObject's
            new ConcurrentDictionary<string, NetworkStream>();
        
        const int PORT_FOR_CLIENTS = 11009;
        //const int PORT_FOR_HTTP_SERVER = 11000;
        private TcpListener listener;
        
        public void StartListening()
        {
            try
            {
                var x  = Dns.GetHostAddresses(Dns.GetHostName());
                var ipAddress = new IPAddress(x[0].GetAddressBytes());
                
                listener = new TcpListener(ipAddress, PORT_FOR_CLIENTS);
                listener.Start();
                Console.WriteLine(
                    $"Socket-server is waiting for a connection... at {ipAddress} at port {PORT_FOR_CLIENTS}");
                while(true)
                {
                    var client = listener.AcceptTcpClient();
                    var clientObject = new ClientObject(client);
 
                    var clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
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

        /*public void StartListeningToHttpServer()
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
                        byte[] bytes = new byte[1024];
                        
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
        }*/
        
    }
}