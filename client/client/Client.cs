using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
namespace ConsoleApplication1
{
    class Client
    {
        Random rnd = new Random();
        public string ownIP = "127.0.0.1";
        string nums = ".0123456789";
        string chars = "abcdefghijklmnopqrstuvABCDEFGHIJKLMNOPQRSTUV1234567890#._-";
        TcpClient clientSocket = new TcpClient();
        NetworkStream netStream = default(NetworkStream);
        public string readData = null;
        public void start()
        {
            //getIP();
            connect_server(ownIP);
            string newIP = next_server();
            Console.WriteLine(">> Next server: " + newIP);
            CTF(newIP);
        }

        public void CTF(string IP)
        {
            string name;
            connect_server(IP);
            Console.WriteLine(">> Who are you?");
            ask("who_are_you?");
            Thread.Sleep(500);
            name = readData;
            Console.WriteLine(">> Talking to " + name + "\n");
            Console.WriteLine(">> Have flag?");
            ask("have_flag?");
            Thread.Sleep(500);
            Console.WriteLine(">R> YES " + readData);
            Thread.Sleep(500);
            if (readData.Substring(0, 2) == "NO")
            {
                Thread.Sleep(500);
                string newIP = next_server();
                CTF(newIP);
            }
            else
            {
                string flag = readData.Substring(4);
                Console.WriteLine(">> Capture flag " + flag);
                ask("capture_flag " + flag + "");
                Console.WriteLine(">R> "+readData);
                hide_flag(flag, name);
                string newIP = next_server();
                Console.WriteLine(">R> Next server: " + newIP);
                CTF(newIP);
            }
        }

        string next_server()
        {
            Console.WriteLine(">> Next server?");
            ask("next_server");
            Thread.Sleep(500);
            Console.WriteLine(">R> Next server: "+readData);
            int ipCif = 0;
            foreach (char c in readData)
            {
                ipCif++;
                if (nums.IndexOf(c) == -1)
                    break;
            }
           // Console.WriteLine(ipCif);
            //Console.WriteLine(readData.Substring(0, ipCif - 1) + "!!");
            return readData.Substring(0, ipCif);
        }

        public void connect_server(string IP)
        {
            try
            {
                if (clientSocket.Connected)
                {
                    clientSocket.Close();
                    clientSocket = new TcpClient();
                }
                    clientSocket.Connect(IP, 80);
                    netStream = clientSocket.GetStream();
                    Console.WriteLine(">> Connected to " + IP);
                    
            }
            catch (Exception ev)
            {
                Console.Write(ev.ToString());
            }
        } //works well

        public string getIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            //ownIP = localIP;
            return ownIP;
        }

        public void getMessage()
        {
            bool ok = true;
            while (ok)
            {
                try
                {
                    netStream = clientSocket.GetStream();
                    if (netStream.CanRead) ok = false;
                    byte[] inStream = new byte[10025];
                    netStream.Read(inStream, 0, 1024);
                    string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                    readData = "" + returndata;
                    readData = readData.Substring(0, readData.LastIndexOfAny(chars.ToCharArray()) + 1);
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.ToString());
                }
            }
           //Console.WriteLine(readData);
           //Console.ReadLine();
        }

        void hide_flag(string flag, string name)
        {
            connect_server(ownIP);
            Console.WriteLine(">> Got the flag from " + name + "");
            ask("hide_flag " + flag + "");
            int t = rnd.Next(1,10);
            Console.WriteLine(">> Resting now for " + t.ToString() + " seconds.");
            Thread.Sleep(t * 1000);
        }

        public void ask(string question)
        {
            try
            {
                readData = "";
                Byte[] sendBytes = Encoding.ASCII.GetBytes(question);
                netStream.Write(sendBytes, 0, sendBytes.Length);
                netStream.Flush();
                getMessage();
            }
            catch (Exception ev)
            {
                Console.WriteLine(ev.ToString());
            }
        }
    }
}

