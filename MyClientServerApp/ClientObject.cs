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

        private readonly string _token;
        private NetworkStream _stream = null;

        private readonly TcpClient _client;
        
        public ClientObject(TcpClient tcpClient)
        {
            _client = tcpClient;
            _token = Guid.NewGuid().ToString();
        }
 
        public void Process()
        {
            try
            {
                _stream = _client.GetStream();
                
                MultiThreadSocketServer.clients.AddOrUpdate(_token, _stream, (k, v) => v = _stream); // client registers in clients list and can get messages
                
                var tokenDict = new Dictionary<string, string>
                {
                    ["token"] = _token,
                };
                    
                var jsonToken = JsonConvert.SerializeObject(tokenDict, Formatting.Indented);
                var msg = Encoding.ASCII.GetBytes(jsonToken);
                _stream.Write(msg);
                
                string data = null;
                var bytes = new byte[1024];
                while (true)
                {
                    var bytesRec = _stream.Read(bytes);  
                    var receivedData = Encoding.ASCII.GetString(bytes,0,bytesRec);
                    var isThereCloseCommand = false;
                   
                    if (receivedData.IndexOf(CLOSE_COMMAND) > -1)
                    {
                        var firstIndex = receivedData.IndexOf(CLOSE_COMMAND);
                        receivedData = receivedData.Substring(0, firstIndex);
                        isThereCloseCommand = true;
                    }
            
                    while (receivedData.IndexOf(END_OF_MESSAGE_COMMAND) > -1)
                    {
                        var firstIndex = receivedData.IndexOf(END_OF_MESSAGE_COMMAND);
                        var lastIndex = firstIndex+END_OF_MESSAGE_COMMAND.Length;
                        data = data + receivedData.Substring(0, firstIndex);
                        Console.WriteLine($"Text received : {data}");
                        _stream.Write( Encoding.ASCII.GetBytes(data) );
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
                MultiThreadSocketServer.clients.TryRemove(_token, out _stream); // client is removed from the clients list and cannot get messages
                
                Console.WriteLine("Server closed the connection with the client {0}.", _token);
                
                _stream?.Close();
                _client?.Close();
            }
        }
    }
}