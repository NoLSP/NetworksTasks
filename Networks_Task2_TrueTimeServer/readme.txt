Задача 2 Сервер точного времени.
Решение состоит из клиента и сервера. Клиент посылает запрос серверу и получает ответ, выводит его на экран.
Сервер получает запрос. Обращается к системе за точным временем, делает смещение секунд на значение, указанное в файле Networks_Task2_TrueTimeServer\TrueTime\TrueTime\bin\Debug\netcoreapp3.1\Config.txt
и отправляет клиенту ответ.

Запуск осуществляется по пути: 
Клиент - Networks_Task2_TrueTimeServer\TrueTimeClient\TrueTimeClient\bin\Debug\netcoreapp3.1\TrueTimeClient
Сервер - Networks_Task2_TrueTimeServer\TrueTime\TrueTime\bin\Debug\netcoreapp3.1\TryeTime.exe