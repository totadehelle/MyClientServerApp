using System;
using System.Net;
using System.Threading;

namespace MyClientServerApp
{
    public class MultiThreadHttpServer
    {
        private HttpListener _server;
        private const string URI = @"http://127.0.1.1:8080/";

        public void StartListening()
        {
            try
            {
                _server = new HttpListener();

                if (!HttpListener.IsSupported) return;

                _server.Prefixes.Add(URI);

                _server.Start();
                Console.WriteLine("Http-server is started at " + URI);

                while (_server.IsListening)
                {
                    //wait for incoming requests
                    var context = _server.GetContext();
                    
                    var httpClientObject = new HttpClientObject(context);
                    var clientThread = new Thread(new ThreadStart(httpClientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _server?.Stop();
            }
        }
    }
}