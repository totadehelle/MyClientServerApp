using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;


namespace MyClientServerApp
{
    public class HttpMessageServer
    {
        HttpListener server;
        bool flag = true;

        public HttpMessageServer()
        {
            process();
        }
        
        public void process()
        {
            //resource for requests
            string uri = @"http://192.168.10.1:8080/say/";
            StartServer(uri);
        }


        private void StartServer(string prefix)
        {
            string message = null;
            
            
            server = new HttpListener();

            if (!HttpListener.IsSupported) return;

            //add prefix (say/)
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException("prefix");
            server.Prefixes.Add(prefix);

            server.Start();
            Console.WriteLine("Http-server is started!");

            while (server.IsListening)
            {
                //wait for incoming requests
                HttpListenerContext context = server.GetContext();
                //get incoming request
                HttpListenerRequest request = context.Request;
                //process POST-request
                if (request.HttpMethod == "POST")
                {
                    message = ShowRequestData(request);
                    //close connection
                    if (!flag) return;
                }

                string responseString = "The message received: " + message;
                HttpListenerResponse response = context.Response;
                response.ContentType = "text; charset=ASCII";
                byte[] buffer = Encoding.ASCII.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public string ShowRequestData (HttpListenerRequest request)
        {
            string clientRequest = null;
            
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return null;
            }
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
            if (request.ContentType != null)
            {
                Console.WriteLine("Client data content type {0}", request.ContentType);
            }
            Console.WriteLine("Client data content length {0}", request.ContentLength64);
   
            Console.WriteLine("Start of client data:");
            
            // Convert the data to a string and display it on the console.
            string s = reader.ReadToEnd();
            Console.WriteLine(s);
            Console.WriteLine("End of client data:");
            clientRequest = s;
            body.Close();
            reader.Close();

            return clientRequest;
        }
    }
}