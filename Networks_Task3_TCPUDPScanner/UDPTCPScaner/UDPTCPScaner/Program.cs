using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace UDPTCPScaner
{
    static class Program
    {
        private static int from = 0;
        private static int to = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Сканер запущен...\nВведите диапазон портов(через пробел)");
            var input = Console.ReadLine();
            from = int.Parse(input.Split(" ")[0]);
            to = int.Parse(input.Split(" ")[1]);
            Console.WriteLine("Сканирование запущено...");
            for (int i = from; i <= to; i++)
            {
                CheckIfPortIsOpen(i);
            }
            //rConsole.WriteLine("Сканирование завершено...");

        }

        public static void CheckIfPortIsOpen(int port)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Socket g_SvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var localAddress = new IPEndPoint(IPAddress.Any, port);
                try
                {
                    g_SvSocket.Bind(localAddress);
                    g_SvSocket.Listen(4);
                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    {
                        Console.WriteLine(localAddress.ToString() + " open");
                    }
                    
                }
            }));
            thread.Start();
        }
    }
}
