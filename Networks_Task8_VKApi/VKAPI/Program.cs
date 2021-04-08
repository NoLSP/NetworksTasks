using System;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using System.Linq;

namespace VKAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Программа запущена");
			var api = new VkApi();
			Console.WriteLine("У вас двухфакторная аутентиикация? Y/N");
			var answer = Console.ReadLine().ToLower();
			var isNotLogged = true;
			if (answer == "n")
				while (isNotLogged)
				{
					Console.WriteLine("Авторизация...\nВведите логин: ");
					var login = Console.ReadLine();
					Console.WriteLine("Введите пароль:");
					var password = Console.ReadLine();
					try
					{
						api.Authorize(new ApiAuthParams
						{
							ApplicationId = 123456,
							Login = login,
							Password = password,
							Settings = Settings.All
						});
						isNotLogged = false;
					}
					catch (Exception ex)
					{
						isNotLogged = true;
						Console.WriteLine("Неверный логин или пароль");
					}
				}
			else
				while (isNotLogged)
				{
					Console.WriteLine("Авторизация...\nВведите логин: ");
					var login = Console.ReadLine();
					Console.WriteLine("Введите пароль:");
					var password = Console.ReadLine();
					try
					{
						api.Authorize(new ApiAuthParams
						{
							ApplicationId = 123456,
							Login = login,
							Password = password,
							Settings = Settings.All,
							TwoFactorAuthorization = () =>
							{
								Console.WriteLine("Введите код:");
								return Console.ReadLine();
							}
						});
						isNotLogged = false;
					}
					catch (Exception ex)
					{
						isNotLogged = true;
						Console.WriteLine("Неверный логин или пароль");
					}
				}
			Console.WriteLine(api.Token);
			Console.WriteLine("Ваши подписки:");
			var res = api.Groups.Get(new GroupsGetParams());
			foreach (var group in api.Groups.GetById(res.Select(r => r.Id + "").ToArray(), null, null))
				Console.WriteLine(group.Name);

			Console.WriteLine("Введите идентификатор пользователя:");
			var id = long.Parse(Console.ReadLine());
			var user = api.Users.Get(new long[1] { id }).FirstOrDefault();
			Console.WriteLine("Пользователь: " + user.FirstName + " " + user.LastName);
			Console.WriteLine("Список друзей пользователя: ");
			var friends = api.Friends.Get(new VkNet.Model.RequestParams.FriendsGetParams
			{
				UserId = id,
				Count = 10,
			});
			foreach (var friend in api.Users.Get(friends.Select(f => f.Id)))
			{
				Console.WriteLine(friend.FirstName + " " + friend.LastName);
			}
		}
    }
}
