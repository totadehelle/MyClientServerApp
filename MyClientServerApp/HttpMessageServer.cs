using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;


namespace MyClientServerApp
{
    public class HttpMessageServer
    {
        HttpListener server;
        bool flag = true;
        string uri = @"http://127.0.1.1:8080/";
        
        public void StartListening()
        {
            string message = null;
            
            server = new HttpListener();

            if (!HttpListener.IsSupported) return;

            if (string.IsNullOrEmpty(uri))
                throw new ArgumentException("prefix");
            server.Prefixes.Add(uri);

            server.Start();
            Console.WriteLine("Http-server is started at " + uri);

            while (server.IsListening)
            {
                //wait for incoming requests
                HttpListenerContext context = server.GetContext();
                //get incoming request
                HttpListenerRequest request = context.Request;
                //process POST-request
                string responseString = null;
                
                if (request.HttpMethod == "POST")
                {
                    message = ShowRequestData(request);

                    try
                    {
                        Dictionary<string, string> messageData =
                            JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
                        
                        byte[] bytes = Encoding.ASCII.GetBytes(messageData["message"]);
                            MultiThreadSocketServer.clients[messageData["token"]].Write(bytes);
                        
                        
                        responseString = $"Http-server says: The message received&sent: {message}";
                    }
                    catch (Exception e)
                    {
                        responseString = $"Http-server says: The message {message} is incorrect.";
                    }
                    //sendMessageToServer(message);
                    //close connection
                    if (!flag) return;
                }

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
                Console.WriteLine("Http-server says: No client data was sent with the request.");
                return null;
            }
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
            if (request.ContentType != null)
            {
                Console.WriteLine("Http-server says: Client data content type {0}", request.ContentType);
            }
            Console.WriteLine("Http-server says: Client data content length {0}", request.ContentLength64);
   
            Console.WriteLine("Http-server says: Start of client data:");
            
            // Convert the data to a string and display it on the console.
            string s = reader.ReadToEnd();
            Console.WriteLine(s);
            Console.WriteLine("Http-server says: End of client data.");
            clientRequest = s;
            body.Close();
            reader.Close();

            return clientRequest;
        }

        /*public void sendMessageToServer(string messageForServer)
        {
            int port = 11000;
            string address = "127.0.1.1";
            
            TcpClient client = null;
            try
            {
                client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();
 
                byte[] data = Encoding.Unicode.GetBytes(messageForServer);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Http-server says: data was sent to the Socket-server: {0}", messageForServer);
                client.Close();
            }
            
        }*/
    }
}