using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace MyClientServerApp
{
    class Program
    {
        
        
        
        static void Main(string[] args)
        {
            HttpMessageServer httpServer = new HttpMessageServer();
            MultiThreadSocketServer server = new MultiThreadSocketServer();
            
            //SocketServer socketServer = new SocketServer();
        }
    }


}