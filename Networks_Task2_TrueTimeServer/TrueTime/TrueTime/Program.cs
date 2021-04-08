using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TrueTime
{
    static class Program
    {
        private static string ip = "127.0.0.1";
        private static int port = 123;
        private static int offset = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Сервер запущен...");
            SetTimeOffset();
            while (true)
                SendAnswer(WaitRequest());
        }

        private static byte[] WaitRequest()
        {
            UdpClient receiver = new UdpClient(port); // UdpClient для получения данных
            IPEndPoint remoteIp = new IPEndPoint(IPAddress.Parse(ip), port); // адрес входящего подключения
            try
            {
                byte[] data = receiver.Receive(ref remoteIp); // получаем данные
                Console.WriteLine("Получен запрос...");
                return Resolve();               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
                
            }
            return new byte[0];
        }

        public static void SendAnswer(byte[] data)
        {

            UdpClient sender = new UdpClient(); // создаем UdpClient для отправки сообщений
            try
            {
                sender.Send(data, data.Length, ip, 124); // отправка    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sender.Close();
            }

        }

        private static byte[] Resolve()
        {
            var dateTime = DateTime.Now;
            return Encoding.ASCII.GetBytes(dateTime.Hour + ":" + dateTime.Minute + ":" + (dateTime.Second + offset));
        }

        private static void SetTimeOffset()
        {
            offset = int.Parse(File.ReadAllText("Config.txt"));
        }
    }
}
