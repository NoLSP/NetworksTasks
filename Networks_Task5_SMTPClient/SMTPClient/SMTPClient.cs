using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SMTPClient
{
    static class SMTPClient
    {
        private static string fromAddress = "";
        private static string password = "";

        private static string body = "";
        private static string subject = "";
        private static string[] recievers;
        private static string[] attachments;

        static void Main(string[] args)
        {
            Console.WriteLine("Сервер запущен\nИнициализация компонентов письма...");
            readConfig("Config.txt");
            readBody("Body.txt");
            Console.Write("Введите адрес отправителя: ");
            fromAddress = Console.ReadLine();
            Console.Write("Введите пароль: ");
            password = Console.ReadLine();
            Console.WriteLine("Идет отправка...");
            SendMessages();
            Console.WriteLine("Отправка завершена. Нажмите Enter, чтобы завершить работу");
            Console.ReadLine();
        }

        private static void SendMessages()
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress(fromAddress, fromAddress.Split("@")[0]);
            MailMessage m = new MailMessage();
            m.From = from;
            foreach (var rec in recievers)
            {
                m.To.Add(new MailAddress(rec));
            }
            // тема письма
            m.Subject = subject;
            // текст письма
            m.Body = body;
            // письмо представляет код html
            m.IsBodyHtml = false;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential(from.Address, password);
            smtp.EnableSsl = true;
            foreach (var att in GetAttachments())
                m.Attachments.Add(att);
            smtp.Send(m);
        }

        private static void readConfig(string path)
        {
            var config = File.ReadAllLines(path);

            subject = config[0];
            recievers = config[1].Split(" ");
            attachments = config[2].Split(" ");
        }

        private static void readBody(string path)
        {
            body = File.ReadAllText(path);
        }

        private static List<Attachment> GetAttachments()
        {
            var result = new List<Attachment>();
            foreach (var att in attachments)
                result.Add(new Attachment(att));
            return result;

        }
    }
}
