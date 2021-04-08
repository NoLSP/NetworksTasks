using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TrueTimeClient
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Клиент запущен...\nНажмите Enter, чтобы сделать запрос");
            while(true)
            {
                Console.ReadKey();
                MakeRequest();
            }
        }

        private static void MakeRequest()
        {
            var req = Encoding.ASCII.GetBytes("request");

            UdpClient sender = new UdpClient(); // создаем UdpClient для отправки сообщений
            try
            {
                sender.Send(req, req.Length, "127.0.0.1", 123); // отправка    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sender.Close();
            }

            UdpClient receiver = new UdpClient(124); // UdpClient для получения данных
            IPEndPoint remoteIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 124); // адрес входящего подключения
            try
            {
                byte[] data = receiver.Receive(ref remoteIp); // получаем данные
                Console.WriteLine("Получен ответ...");
                Console.WriteLine("Время системы: " + DateTime.Now);
                Console.WriteLine("Ответ сервера: " + Encoding.ASCII.GetString(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
            }
        }
    }
}
