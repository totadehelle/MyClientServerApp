using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace MyClientServerApp
{
    public class HttpClientObject
    {
        
        private readonly HttpListenerContext _context;

        public HttpClientObject(HttpListenerContext context)
        {
            _context = context;
        }

        public void Process()
        {
            try
            {
                var request = _context.Request;
                string responseString = null;
                if (request.HttpMethod == "POST")
                {
                    var message = ShowRequestData(request);
                    

                    try
                    {
                        var messageData =
                            JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

                        var bytes = Encoding.ASCII.GetBytes(messageData["message"]);
                        MultiThreadSocketServer.clients[messageData["token"]].Write(bytes);


                        responseString = $"Http-server says: The message received&sent: {message}";
                    }
                    catch (Exception e)
                    {
                        responseString = $"Http-server says: The message {message} is incorrect.";
                    }
                }

                var response = _context.Response;
                response.ContentType = "text; charset=ASCII";
                var buffer = Encoding.ASCII.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                using (var output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private string ShowRequestData(HttpListenerRequest request)
        {
            string clientRequest = null;

            if (!request.HasEntityBody)
            {
                Console.WriteLine("Http-server says: No client data was sent with the request.");
                return null;
            }

            var body = request.InputStream;
            var encoding = request.ContentEncoding;
            var reader = new System.IO.StreamReader(body, encoding);
            
            if (request.ContentType != null)
            {
                Console.WriteLine("Http-server says: Client data content type {0}", request.ContentType);
            }

            Console.WriteLine("Http-server says: Client data content length {0}", request.ContentLength64);

            Console.WriteLine("Http-server says: Start of client data:");

            // Convert the data to a string and display it on the console.
            clientRequest = reader.ReadToEnd();
            Console.WriteLine(clientRequest);
            
            Console.WriteLine("Http-server says: End of client data.");
            
            body.Close();
            reader.Close();

            return clientRequest;
        }
    }
}