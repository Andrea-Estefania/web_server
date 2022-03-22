using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace myOwnWebServer
{
    class Server
    {
        private const int kBufferSize = 8192;
        volatile bool runforever = true;

        private Header Head = new Header();
        TcpListener server = null;

        private string Root = null;
        private IPAddress IP = null;
        private int Port = 0;

        /* 
           Name	    : Server
           Purpose  : To instantiate a new server object to establish communication between the browser and the server
           Inputs	:	    string webRoot: Folder that contains all the files the server has "access"
                            IPAddress: IP address of the server
                            int webPort: port number the server lsitens to
           Outputs	:	NONE
           Returns	:	Nothing
        */

        public Server(string webRoot, IPAddress webIP, int webPort)
        {
            Root = webRoot;
            IP = webIP;
            Port = webPort;
        }

        // Name: StartListen
        // Purpose: Starts server to begin listening for browser requests, receives them, processes a response complete with header and body and then sends the response back to the requesting browser
        public void StartListen()
        {
            try
            {
                server = new TcpListener(IP, Port); // Our server
                server.Start();

                Logger.Log("[SERVER STARTED] - webRoot=" + Root + ", webIP=" + IP.ToString() + ", -port=" + Port.ToString()); // Los the server started

                while(runforever)
                {
                    // Connect and get data stream from client
                    TcpClient client = server.AcceptTcpClient(); 
                    NetworkStream stream = client.GetStream();

                    // Read data stream
                    byte[] data = new byte[kBufferSize];
                    int i = stream.Read(data, 0, data.Length);

                    // Encode request data to string
                    string request = Encoding.ASCII.GetString(data, 0 , i);

                    // Logs our current request
                    int posToGetRequest = request.IndexOf("HTTP");
                    string requestToLog = request.Substring(0, posToGetRequest - 1);
                    Logger.Log(requestToLog);

                    // Sets the root to "match" the behavior of IIS
                    Root = Root.Replace("\\", "/");
                    Root = Root.TrimEnd('/');

                    // Process request into header string and save into response
                    string response = Head.MakeHeader(request, Root);

                    // Contents to log if invalid request
                    int posToGetResponse = response.IndexOf("\r\nContent-Type");
                    string error = response.Substring(0, posToGetResponse);

                    if (error != "HTTP/1.1 200 OK")
                    {
                        Logger.Log(error);
                    }

                    // Encode header into data
                    data = Encoding.ASCII.GetBytes(response);

                    // Encode body into data
                    data = AddContent(data, Head.ContentPath);
                    stream.Write(data, 0, data.Length);
                    Head.ValidRequest = false;
                    client.Close();

                }
            } catch (SocketException ex)
            {
                Logger.Log(ex.Message);
            }
        }

        // Name: AddContent
        // Purpose: Used to construct body data, whether it's an image or not. Reads all the data into a string, encodes it into bytes, appends it to existing data and returns compiled data
        // Inputs: byte[] data - Our encode header data
        //         string path - Path to get the file the browser needs to display
                   
        private byte[] AddContent(byte[] data, string path)
        {
            List<byte> byteList = new List<byte>(data);

            if (!Head.ValidRequest) // If invalid extension/request
            {
                byteList.AddRange(Encoding.ASCII.GetBytes(ErrorHTML.GetHTML(Head.StatusCode))); // Displays the correct HTTP status code
            }
            else
            {
                try
                {
                    if (Head.ContentType.StartsWith("image")) /// Do we need to read an image/gif?
                    {
                        byteList.AddRange(File.ReadAllBytes(path));

                    }
                    else // The rest of the valid extensions
                    {
                        byteList.AddRange(Encoding.ASCII.GetBytes(File.ReadAllText(path)));
                    }
                }
                catch (IOException ie)
                {
                    Console.WriteLine("File Not Found! - {0}", ie);
                    byteList.AddRange(Encoding.ASCII.GetBytes(ErrorHTML.GetHTML(Head.StatusCode)));
                }

                // A 200 OK is log
                string okLog =     "\r\nContent-Type: " + Head.ContentType +
                                   "\r\nContent-Length: " + Head.ContentLength +
                                   "\r\nServer: " + Head.ServerDetails +
                                   "\r\nDate: " + Head.ReplyDate;
                Logger.Log(okLog);

            }
            return byteList.ToArray();
        }
    }
}
