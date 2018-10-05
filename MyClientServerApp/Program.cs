using System.Threading;

namespace MyClientServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpServer = new MultiThreadHttpServer();
            var server = new MultiThreadSocketServer();
            
            var httpThread = new Thread(new ThreadStart(httpServer.StartListening));
            var socketThread = new Thread(new ThreadStart(server.StartListening));
            //var socketForHTTPThread = new Thread(new ThreadStart(server.StartListeningToHttpServer));
            
            httpThread.Start();
            socketThread.Start();
            //socketForHTTPThread.Start();
            
            //SocketServer socketServer = new SocketServer();
        }
    }


}