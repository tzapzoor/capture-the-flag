using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace ConsoleApplication1
{
    class handleClient
    {
        TcpClient clientSocket;
        string clNo;
        string uid;
        //string flag_value;
        string[] ipArray;
        int k = 0;
        Random rnd = new Random();
        int rand;
        bool read = false;
        string chars = "abcdefghijklmnopqrstuvABCDEFGHIJKLMNOPQRSTUV1234567890#._-?!";
        public void startClient(TcpClient inClientSocket, string clineNo, string UID)
        {

            this.clientSocket = inClientSocket;
            this.uid = UID;
            this.clNo = clineNo;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();

        }

        private void doChat()
        {

            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            requestCount = 0;

            bool ok = true;
            //clientSocket.ReceiveTimeout = 50;
            while ((ok))
            {
                if (!clientSocket.Connected)
                {
                    Console.WriteLine("[client]: Disconnected!");
                    clientSocket.Close();
                    ok = false;

                }
                try
                {
                    //files
                    StreamReader s1, s2, s3; StreamWriter s5, s6; string line;
                    string flag_val = ""; string flag_bool = "";
                    try
                    {
                        s2 = new StreamReader("flag_val.txt");
                        s3 = new StreamReader("flag_bool.txt");
                        flag_val = s2.ReadLine();
                        flag_bool = s3.ReadLine();
                        if (read == false)
                        {
                            s1 = new StreamReader("ip.txt");
                            line = s1.ReadLine();
                            ipArray = new string[100];
                            k = 0;
                            while (line != null)
                            {
                                ipArray[k++] = line;
                                line = s1.ReadLine();
                            } k--;
                            s1.Close();
                            read = true;
                        }
                        s2.Close(); s3.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("[system]: Err: Cannot read from files!");
                    }
                    bytesFrom = new byte[10025];
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, 1024);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);

                    dataFromClient = dataFromClient.Substring(0, dataFromClient.LastIndexOfAny(chars.ToCharArray()) + 1);
                    if (dataFromClient != "") Console.WriteLine("[client]: " + dataFromClient);
                    if (dataFromClient == "who_are_you?")
                    {
                        Console.WriteLine("[system]: " + "I am " + uid);
                        serverResponse = uid;
                    }
                    if (dataFromClient == "have_flag?")
                    {
                        if (flag_bool.IndexOf("1") != -1)
                        {
                            Console.WriteLine("[system]: " + "YES " + flag_val);
                            serverResponse = "YES " + flag_val;
                        }
                        else
                        {
                            Console.WriteLine("[system]: NO!");
                            serverResponse = "NO";
                        }
                    }
                    int p = dataFromClient.IndexOf("capture_flag");
                    string flagzoor;
                    if (p != -1) //try to capture flag
                    {

                        flagzoor = dataFromClient.Substring(p + 13);
                        /* Console.WriteLine(flagzoor+'\n');
                         Console.WriteLine(flag_bool);
                         Console.WriteLine(flag_val);*/
                        int okz = 0;
                        if (flagzoor == flag_val && flag_bool.IndexOf("1") != -1)
                        {
                            okz = 1;
                            serverResponse = flag_val;
                            Console.WriteLine("[system]: Ok. Trying to capture flag " + flagzoor);
                            flag_bool = "0";
                            flag_val = "";
                            try
                            {

                                s5 = new StreamWriter("flag_val.txt");
                                s6 = new StreamWriter("flag_bool.txt");
                                s6.WriteLine(flag_bool);
                                s5.WriteLine(flag_val);
                                s5.Close(); s6.Close();
                                Console.WriteLine("[system]: Server lost flag!");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(" >!< Cannot write to files (1)!");
                            }

                        }
                        if (okz == 0)
                        {
                            // Console.WriteLine("Error!! TrickWOHOO");
                            Console.WriteLine("[system]: ERRR! Client trying to trick the server!");
                            serverResponse = "ERR: You're trying to trick me!";
                        }
                    }
                    if (dataFromClient == "next_server")
                    {
                        //Console.WriteLine(k);
                        if (k != 0)
                            rand = rnd.Next(0, k - 1);
                        else
                            rand = 0;
                        Console.WriteLine("[system]: Next_server " + ipArray[rand]);
                        //serverResponse = "192.170.1.119";
                        serverResponse = ipArray[rand];
                    }
                    p = dataFromClient.IndexOf("hide_flag");
                    if (p != -1)
                    {
                        flagzoor = dataFromClient.Substring(p + 10);
                        flag_bool = "1";
                        flag_val = flagzoor;
                        try
                        {

                            s5 = new StreamWriter("flag_val.txt");
                            s6 = new StreamWriter("flag_bool.txt");
                            s6.WriteLine(flag_bool);
                            s5.WriteLine(flag_val);
                            s5.Close(); s6.Close();
                            Console.WriteLine("[system]: Flag hidden" + flagzoor);
                            serverResponse = "Flag Hidden" + flagzoor;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(" >!< Cannot write to files(2)!");
                        }


                    }
                    sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                }
                catch (Exception ex)
                {
                    // Console.WriteLine(" >< Cannot read from stream");
                }

            }

        }

    }
}
