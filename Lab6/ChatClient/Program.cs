// ChatClient — клиент чата.
// Подключается к серверу, регистрирует ник, в фоновой задаче печатает
// входящие сообщения, а главный поток читает ввод пользователя.
// Команды: /list — кто в чате, /w ник текст — личное, /exit — выход.
using System.Net.Sockets;
using System.Text;

// чтобы кириллица не превращалась в кракозябры при выводе
Console.OutputEncoding = Encoding.UTF8;

Console.Write("Адрес сервера (Enter = localhost): ");
string host = Console.ReadLine() is { Length: > 0 } h ? h : "localhost";

Console.Write("Порт (Enter = 5555): ");
int port = int.TryParse(Console.ReadLine(), out int p) ? p : 5555;

using var client = new TcpClient();
try
{
    await client.ConnectAsync(host, port);
}
catch (SocketException)
{
    Console.WriteLine($"Не удалось подключиться к {host}:{port} — сервер недоступен.");
    return;
}

var stream = client.GetStream();
var reader = new StreamReader(stream);
var writer = new StreamWriter(stream) { AutoFlush = true };

// --- регистрация ника: сервер отвечает NICK_OK или NICK_BUSY ---
while (true)
{
    Console.Write("Ваш ник: ");
    string nick = Console.ReadLine() ?? "";
    await writer.WriteLineAsync(nick);

    string? answer = await reader.ReadLineAsync();
    if (answer == "NICK_OK")
        break;
    if (answer is null)
    {
        Console.WriteLine("Сервер закрыл соединение.");
        return;
    }
    Console.WriteLine("Этот ник занят, попробуйте другой.");
}

// приём сообщений — параллельно с вводом, в фоновой задаче
_ = Task.Run(async () =>
{
    try
    {
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
            Console.WriteLine(line);
    }
    catch (IOException) { }
    Console.WriteLine("Соединение с сервером потеряно. Нажмите Enter для выхода.");
});

Console.WriteLine("Подключено. Команды: /list — кто в чате, /w ник текст — личное, /exit — выход.");

// главный цикл: читаем ввод и шлём на сервер
while (true)
{
    string? msg = Console.ReadLine();
    if (msg is null) break;          // конец ввода (Ctrl+Z / закрытие пайпа)
    if (msg.Length == 0) continue;
    try
    {
        await writer.WriteLineAsync(msg);
    }
    catch (IOException)
    {
        Console.WriteLine("Не удалось отправить: сервер отключился.");
        break;
    }
    if (msg == "/exit") break;
}
