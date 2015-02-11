using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("<<Capture the flag>> CLIENT\n");

            Client cl = new Client();
            //cl.CTF("127.0.0.1");
            cl.start();
        }
    }
       
}
