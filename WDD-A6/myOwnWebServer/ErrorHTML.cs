namespace myOwnWebServer
{
    public static class ErrorHTML
    {
        // Name: GetHTML
        // Purpose: Returns HTML body string appropriate to HTTP status code
        public static string GetHTML(string statusCode)
        {
            if (statusCode.StartsWith("404")) // NOT FOUND
            {
                return "<!DOCTYPE html><html>" +
                    "<head><title>404 Not Found</title></head>" +
                    "<body><h1>404 - NOT FOUND</h1>" +
                    "<p>The requested item does not exist</p>" +
                    "<br/ ><hr>" +

                    "<p>Use a known file name or search through the server's directory!</p>" +
                    "</body></html>";
            }
            if (statusCode.StartsWith("415")) // UNSUPORTED MEDIA
            {
                return "<!DOCTYPE html><html>" +
                    "<head><title>415 Unsupported Media Type</title></head>" +
                    "<body><h1>415 - Unsupported Media Type</h1>" +
                    "<p>Media type is not supported by this server.</p>" +
                    "<br/ ><hr>" +

                    "<p>Only files with extensions...</p>" +
                    "<p>.htm, .html, .htmls, .jfif, .jpe, .jpeg, .jpg, .jfif-tbnl, .gif, .txt</p>" +
                    "<p>... are processed by the server!</p>" +
                    "</body></html>";
            }
            if (statusCode.StartsWith("501")) // NOT IMPLEMENTED
            {
                return "<!DOCTYPE html><html>" +
                    "<head><title>501 Not Implemented</title></head>" +
                    "<body><h1>501 - Not Implemented</h1>" +
                    "<p>Method not implemented.</p>" +
                    "<br/ ><hr>" +

                    "<p>Only GET is allowed!</p>" +
                    "</body></html>";
            }
            if (statusCode.StartsWith("505")) // HTTP VERSION NOT IMPLEMENTED
            {
                return "<!DOCTYPE html><html>" +
                    "<head><title>505 HTTP Version Not Supported</title></head>" +
                    "<body><h1>505 - HTTP Version Not Supported</h1>" +
                    "<p>The requested item does not exist</p>" +
                    "<br/ ><hr>" +

                    "<p>Use a known file name or search through the server's directory!</p>" +
                    "</body></html>";
            }
            return "";
        }
    }
}