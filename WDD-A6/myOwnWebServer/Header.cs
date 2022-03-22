

using System;
using System.IO;

namespace myOwnWebServer
{
    public class Header
    {
        private const string kHTTPVer = "HTTP/1.1";
        public string ContentPath = "";
        public bool ValidRequest = false;

        private string FirstLine = "";
        public string StatusCode;
        public string ContentType = "text/html"; // Default, as any error page will be sent as HTML
        public string ServerDetails = Environment.MachineName; // Does not save actual server, but machine name of server
        public string ReplyDate = DateTime.Now.ToString();
        public int ContentLength = 0;


        //      METHOD: MakeHeader
        // DESCRIPTION: Takes the HTTP request as a string and splits it, processes each field in the header and returns the assembled header string
        public string MakeHeader(string request, string root)
        {
            string[] sepString = request.Split(' ');

            ContentPath = root + sepString[1];

            if (GetStatusCode(sepString))
            {
                ValidRequest = true;
            }

            FirstLine = GetHTTPVersion(sepString[2]) + " " + StatusCode;
            ContentType = GetMIME(ContentPath);
            ContentLength = GetContentLength(ContentPath);

            return FirstLine + "\r\nContent-Type: " + ContentType +
                               "\r\nServer: " + ServerDetails +
                               "\r\nDate: " + ReplyDate +
                               "\r\nContent-Length: " + ContentLength +
                               "\r\n\r\n";
        }

        //      METHOD: GetContentLength
        // DESCRIPTION: Determines if the file exists and gets the length of the file. If file does not exist, processes HTML error page and recursively calls itself to get that error page's length
        private int GetContentLength(string dir)
        {
            string buffer = null;
            int len = 0;

            if (File.Exists(dir))
            {
                using (StreamReader sr = File.OpenText(dir))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        buffer += s;
                    }
                }
            }
            else
            {
                len = ErrorHTML.GetHTML(StatusCode).Length;
            }

            if (buffer != null)
            {
                len = buffer.Length;
            }
            return len;
        }

        //      METHOD: GetHTTPVersion
        // DESCRIPTION: Cuts out end of split string element to return HTTP version only
        private string GetHTTPVersion(string version)
        {
            version = version.Remove(version.IndexOf("\r\n"));
            return version;
        }

        // METHOD: GetStatusCode
        // DESCRIPTION: An incomplete status code function that checks through the different helper functions and returns the appropriate status code string
        private bool GetStatusCode(string[] sepString)
        {
            // Status codes:
            // 200 OK
            // 404 not found
            // 415 Unsupported Media Type
            // 501 not implemented
            // 505 HTTP version not supported (check HTTP ver)
            bool retCode = false;

            if (CheckGET(sepString[0]))
            {
                StatusCode = "501 Not Implemented";
            }
            else if (CheckHTTPVer(GetHTTPVersion(sepString[2])))
            {
                StatusCode = "505 HTTP Version Not Supported";
            }
            else if (CheckContentExists(ContentPath))
            {
                StatusCode = "404 Not Found";
            }
            else if (CheckMime(GetMIME(ContentPath)))
            {
                StatusCode = "415 Unsupported Media Type";
            }
            else
            {
                StatusCode = "200 OK";
                retCode = true;
            }
            return retCode;
        }

        // METHOD: CheckContentExists
        // DESCRIPTION: Checks if file in the path exists, if it does, return false
        private bool CheckContentExists(string path)
        {
            
            return !File.Exists(path);
        }

        // METHOD: GetMIME
        // DESCRIPTION: Checks extension for MIME type, if it exists, returns type, otherwise returns blank
        private string GetMIME(string dir)
        {
            string[] kHTMLExtensions = { ".htm", ".html", ".htmls" };
            string[] kJpegExtensions = { ".jfif", ".jpe", ".jpeg", ".jpg", ".jfif-tbnl" };
            const string kGifExtension = ".gif";
            const string kTxtExtension = ".txt";

            string ext = Path.GetExtension(dir);
            string retCode = "";

            if (ext == kGifExtension) // Is .gif?
            {
                retCode = "image/gif";
                ValidRequest = true;
            }
            else if (ext == kTxtExtension) // Is .txt
            {
                retCode = "text/plain";
                ValidRequest = true;
            }
            else
            {
                for (int i = 0; i < kHTMLExtensions.Length; i++)
                {
                    if (kHTMLExtensions[i] == ext) // Any type of html file?
                    {
                        retCode = "text/html";
                        ValidRequest = true;
                        break;
                    }
      
                }
                for (int ii = 0; ii < kJpegExtensions.Length; ii++) // Any type of Jpeg?
                {
                    if (kJpegExtensions[ii] == ext)
                    {
                        retCode = "image/jpeg";
                        ValidRequest = true;
                        break;
                    }
                }
            }
            return retCode;
        }

        // METHOD: ChecksMime
        // DESCRIPTION: Checks if mime is null or empty
        private bool CheckMime(string mime)
        {
              return string.IsNullOrEmpty(mime);
        }

        // METHOD: CheckHTTPVer
        // DESCRIPTION: Checks if version string is equal to expected constant, returns true if it isn't
        private bool CheckHTTPVer(string v)
        {
            return v != kHTTPVer;
        }

        // METHOD: CheckGET
        // DESCRIPTION: Checks first element in split array for GET, returns true if NOT GET
        private bool CheckGET(string v)
        {
            return v != "GET";
        }
    }
}
