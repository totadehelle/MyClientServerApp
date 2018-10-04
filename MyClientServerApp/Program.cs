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
            
            
            
            
            HttpMessageServer httpServer = new HttpMessageServer();
            MultiThreadSocketServer server = new MultiThreadSocketServer();
            
            Thread httpThread = new Thread(new ThreadStart(httpServer.process));
            Thread socketThread = new Thread(new ThreadStart(server.StartListening));
            
            httpThread.Start();
            socketThread.Start();
            
            //SocketServer socketServer = new SocketServer();
        }
    }


}