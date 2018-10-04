using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

namespace MyClientServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpServer = new HttpMessageServer();
            var server = new MultiThreadSocketServer();
            
            var httpThread = new Thread(new ThreadStart(httpServer.process));
            var socketForClientsThread = new Thread(new ThreadStart(server.StartListeningToClients));
            var socketForHTTPThread = new Thread(new ThreadStart(server.StartListeningToHttpServer));
            
            httpThread.Start();
            socketForClientsThread.Start();
            socketForHTTPThread.Start();
            
            //SocketServer socketServer = new SocketServer();
        }
    }


}