using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.IO;

namespace POP3Client
{
    class POP3Client
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Клиент запущен...");
            // Create an instance of the Pop3Client class
            Pop3Client client = new Pop3Client();

            client.Connect("pop.gmail.com", 995, true);
            Console.WriteLine("Введите логин:");
            var login = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            var password = Console.ReadLine();
            client.Authenticate(login, password);
            var count = client.GetMessageCount();
            
            Console.WriteLine("Чтение последнего письма...");
            Message message = client.GetMessage(count);
            Console.WriteLine("Имя отправителя: " + message.Headers.From.DisplayName, "Адрес отправителя: " + message.Headers.From.Address);
            Console.WriteLine("Дата: " + message.Headers.Date);
            Console.WriteLine("Тема: " + message.Headers.Subject);
            if (!Directory.Exists("Emails"))
                Directory.CreateDirectory("Emails");
            var msgText = "";
            Console.WriteLine("Сохранение письма...");
            foreach(var msg in message.MessagePart.MessageParts)
            {
                if (msg.IsText)
                {
                    msg.Save(new FileInfo("Emails/Message.txt"));
                    msgText = msg.GetBodyAsText();
                    Console.WriteLine("Введите количество символов, которые нужно вывести. Всего: " + msgText.Length);
                    Console.WriteLine(msgText.Substring(int.Parse(Console.ReadLine())));
                }
                if (msg.IsAttachment)
                    msg.Save(new FileInfo("Emails/" + msg.FileName));
            }
            Console.WriteLine("Письмо сохранено\nНажмите любую клавишу для завершения");
            Console.ReadKey();
        }
    }
}
