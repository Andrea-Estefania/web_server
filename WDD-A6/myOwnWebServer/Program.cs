/*         FILE : Program.cs
*       PROJECT : PROG2001 - Assignment 6
*   PROGRAMMERS : Andrea Ferro and Sebastian Sannes
* FIRST VERSION : 2021-11-25
*   DESCRIPTION : Starts listener class according to command line arguments passed in.
*/

using System;
using System.Net;
using System.Threading;
using System.IO;

namespace myOwnWebServer
{
    class Program
    {
        private const int kArgLimit = 3;

        static void Main(string[] args)
        {
            string path = "";

            if (args.Length != kArgLimit) // Error out if there not exactly 3 arguments
            {
                Console.WriteLine("You MUST enter all 3 arguments after the command! e.g. <command> -webRoot:<path> -webIP:<IP> -webPort:<port>");
                Console.WriteLine("Exiting program...");
                return;
            }

            try
            {
                path = Path.GetFullPath(args[0].Substring(args[0].IndexOf("=") + 1));
                string ipToValidate = args[1].Substring(args[1].IndexOf("=") + 1);
                string portToValidate = args[2].Substring(args[2].IndexOf("=") + 1);

                if (!Directory.Exists(path)) // Checks if the webRoot given by the user exists
                {
                    Console.WriteLine("You MUST enter a valid directory for the root!");
                    Console.WriteLine("Exiting program...");
                    return;
                }
                else
                {
                    bool validIP = IPAddress.TryParse(ipToValidate, out IPAddress ip);
                    bool validPort = int.TryParse(portToValidate, out int port);

                    if (validIP)
                    {
                        if (validPort)
                        {
                            if (port <= 0)
                            {
                                Console.WriteLine("Invalid Port.");
                                Console.WriteLine("Exiting program...");
                                return;

                            } else
                            {
                                Server server = new Server(path, ip, port);
                                Thread t = new Thread(new ThreadStart(server.StartListen));
                                t.Start(); // Engage listener!
                            }
                        }
                    } else
                    {
                        Console.WriteLine("Invalid IP Address.");
                        Console.WriteLine("Exiting program...");
                        return;
                    }
                }
            }
            catch (PathTooLongException ptle)
            {
                Console.WriteLine("Path Too Long Exception: {0}", ptle);
            }
            catch (OverflowException oe)
            {
                Console.WriteLine("Overflow Exception: {0}", oe);
            }
            catch (FormatException fe)
            {
                Console.WriteLine("Format Exception: {0}", fe);
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("Argument Null Exception: {0}", ane);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine("Argument Exception: {0}", ae);
            }
        }
    }
}
