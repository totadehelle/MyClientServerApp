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

        private string token;
        NetworkStream stream = null;
        
        public TcpClient client;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
            token = Guid.NewGuid().ToString();
        }
 
        public void Process()
        {
            try
            {
                stream = client.GetStream();
                
                MultiThreadSocketServer.clients.AddOrUpdate(token, stream, (k, v) => v = stream); // client registers in clients list and can get messages
                
                Dictionary<string, string> tokenDict = new Dictionary<string, string>
                {
                    ["token"] = token,
                };
                    
                string jsonToken = JsonConvert.SerializeObject(tokenDict, Formatting.Indented);
                byte[] msg = Encoding.ASCII.GetBytes(jsonToken);
                stream.Write(msg);
                
                string data = null;
                byte[] bytes = new byte[1024];
                while (true)
                {
                    int bytesRec = stream.Read(bytes);  
                    string receivedData = Encoding.ASCII.GetString(bytes,0,bytesRec);
                    bool isThereCloseCommand = false;
                   
                    if (receivedData.IndexOf(CLOSE_COMMAND) > -1)
                    {
                        int firstIndex = receivedData.IndexOf(CLOSE_COMMAND);
                        receivedData = receivedData.Substring(0, firstIndex);
                        isThereCloseCommand = true;
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
                    
                    if (isThereCloseCommand) break;
                    
                    data = data + receivedData;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                MultiThreadSocketServer.clients.TryRemove(token, out stream); // clients is removed from the clients list and cannot get messages
                
                Console.WriteLine("Server closed the connection with the client.");
                
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}