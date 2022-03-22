using System;
using System.IO;

namespace myOwnWebServer
{
    static class Logger
    {
        //      METHOD: Log
        // DESCRIPTION: Processes log message into format and appends to log file. If log file doesn't exist, creates new log file with log message
        public static void Log(string msg)
        {
            string logMsg = DateTime.Now.ToString() + " " + msg;
            FileStream file = null;
            StreamWriter sw= null;

            string logPath = Directory.GetCurrentDirectory() + "\\myOwnWebServer.log";

            try
            {
                file = File.Open(logPath, FileMode.Append);
                sw = new StreamWriter(file);
                sw.WriteLine(logMsg);
            }
            catch(IOException ie)
            {
                Console.WriteLine("File Not Found! - {0}", ie);
                file = File.Create(logPath);
                sw.WriteLine(logMsg);
            }
            finally
            {
                sw.Flush();
                sw.Close();
                file.Close();
            }
        }
    }
}
