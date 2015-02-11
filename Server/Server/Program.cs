using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.IO;


namespace ConsoleApplication1
{

    static public class Program
    {
        static string UID = "SMECHER.BASTAN2014"; //server UID
        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(System.Net.IPAddress.Any, 80);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;
            serverSocket.Start();
            Console.WriteLine("<<<<<<<Capture the flag>>>>>>>\n\n" + "[server]: Server Started...");
            counter = 0;

            while (true)
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine("[server]: " + "Client No:" + Convert.ToString(counter) + " connected!");
                handleClient client = new handleClient();
                client.startClient(clientSocket, Convert.ToString(counter),UID);
            }
            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("[system]: " + "Exit!");
            Console.ReadLine();
        }
    }
    //Class o handle each client request separatly
}
