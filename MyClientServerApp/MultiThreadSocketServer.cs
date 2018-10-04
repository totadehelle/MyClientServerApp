using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MyClientServerApp
{
    public class MultiThreadSocketServer
    {
        const int PORT = 11009;
        private TcpListener listener;
        
        public void StartListening()
        {
            try
            {
                IPAddress[] x  = Dns.GetHostAddresses(Dns.GetHostName());
                var ipAddress = new IPAddress(x[0].GetAddressBytes());
                
                listener = new TcpListener(ipAddress, PORT);
                listener.Start();
                Console.WriteLine("Waiting for a connection... at " + ipAddress + " at port " + PORT);
 
                while(true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);
 
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
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
    }
}