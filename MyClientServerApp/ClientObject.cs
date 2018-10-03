using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace MyClientServerApp
{
    public class ClientObject
    {
        private const string END_OF_MESSAGE_COMMAND = "<EOF>";
        private const string CLOSE_COMMAND = "<CLOSE>";
        
        public TcpClient client;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }
 
        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                
                string token = Guid.NewGuid().ToString();
                Dictionary<string, string> tokenDict = new Dictionary<string, string>
                {
                    ["token"] = token,
                };
                    
                string jsonToken = JsonConvert.SerializeObject(tokenDict, Formatting.Indented);
                byte[] msg = Encoding.ASCII.GetBytes(jsonToken);
                stream.Write(msg);
                
                string data = null;
                byte[] bytes = new byte[1024]; // буфер для получаемых данных
                while (true)
                {
                    int bytesRec = stream.Read(bytes);  
                    string receivedData = Encoding.ASCII.GetString(bytes,0,bytesRec);
                   
                    if (receivedData.IndexOf(CLOSE_COMMAND) > -1)
                    {
                        int firstIndex = receivedData.IndexOf(CLOSE_COMMAND);
                        string[] messages = receivedData.Substring(0,firstIndex).Split(END_OF_MESSAGE_COMMAND, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var message in messages)
                        {
                            data = data + message;
                            Console.WriteLine($"Text received : {data}");
                            
                            stream.Write( Encoding.ASCII.GetBytes(data) );
                            data = null;
                        }

                        break;
                    }
            
                    while (receivedData.IndexOf(END_OF_MESSAGE_COMMAND) > -1)
                    {
                        int firstIndex = receivedData.IndexOf(END_OF_MESSAGE_COMMAND);
                        int lastIndex = firstIndex+END_OF_MESSAGE_COMMAND.Length;
                        data = data + receivedData.Substring(0, firstIndex);
                        Console.WriteLine($"Text received : {data}");
                        stream.Write( Encoding.ASCII.GetBytes(data) );
                        data = null;
                        receivedData = receivedData.Substring(lastIndex);
                    }
                    
                    data = data + receivedData;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Server closed the connection with the client.");
                
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}