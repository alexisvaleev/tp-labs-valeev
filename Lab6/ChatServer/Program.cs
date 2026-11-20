// ChatServer — многопользовательский чат по TCP.
// Порт задаётся аргументом командной строки (по умолчанию 5555).
// Протокол текстовый: одно сообщение = одна строка.
// Первое сообщение клиента — ник; сервер отвечает NICK_OK или NICK_BUSY.
// Команды клиента: /list, /w ник текст, /exit.
using System.Net;
using System.Net.Sockets;
using System.Text;

// чтобы кириллица не превращалась в кракозябры при выводе
Console.OutputEncoding = Encoding.UTF8;

int port = args.Length > 0 && int.TryParse(args[0], out int p) ? p : 5555;

var clients = new List<ClientInfo>();
var lockObj = new object();

// Журнал сервера: вывод на экран + дозапись в chat.log (лабораторная 3).
void Log(string text)
{
    string stamped = $"[{DateTime.Now:HH:mm:ss}] {text}";
    Console.WriteLine(stamped);
    lock (lockObj)
        File.AppendAllText("chat.log", stamped + Environment.NewLine);
}

// Рассылка сообщения всем подключённым клиентам.
async Task BroadcastAsync(string message)
{
    List<StreamWriter> snapshot;
    lock (lockObj)
        snapshot = clients.Select(c => c.Writer).ToList();  // копия под замком

    foreach (var w in snapshot)
    {
        try { await w.WriteLineAsync(message); }
        catch (IOException) { /* отвалившийся клиент удалится в своей задаче */ }
    }
}

// Обслуживание одного клиента — каждый в своей асинхронной задаче.
async Task HandleClientAsync(TcpClient client)
{
    var endpoint = client.Client.RemoteEndPoint;
    var stream = client.GetStream();
    var reader = new StreamReader(stream);
    var writer = new StreamWriter(stream) { AutoFlush = true };

    ClientInfo? me = null;
    try
    {
        // --- регистрация ника: ждём, пока не пришлют свободный ---
        string? nick = null;
        while (true)
        {
            nick = await reader.ReadLineAsync();
            if (nick is null) { client.Close(); return; }
            nick = nick.Trim();
            if (nick.Length == 0)
            {
                await writer.WriteLineAsync("NICK_BUSY");
                continue;
            }

            bool busy;
            lock (lockObj)
                busy = clients.Any(c => c.Nick == nick);   // уникальность ника

            if (!busy) break;
            await writer.WriteLineAsync("NICK_BUSY");
        }

        me = new ClientInfo { Nick = nick, Writer = writer };
        lock (lockObj) clients.Add(me);
        await writer.WriteLineAsync("NICK_OK");

        Log($"{nick} подключился ({endpoint})");
        await BroadcastAsync($"*** {nick} вошёл в чат ***");

        // --- основной цикл: читаем строки до /exit или разрыва ---
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (line == "/exit")
                break;

            if (line == "/list")
            {
                string names;
                lock (lockObj)
                    names = string.Join(", ", clients.Select(c => c.Nick));
                await writer.WriteLineAsync($"*** сейчас в чате: {names} ***");
                continue;
            }

            if (line.StartsWith("/w "))
            {
                // личное сообщение: /w ник текст
                string rest = line[3..];
                int sp = rest.IndexOf(' ');
                if (sp < 0)
                {
                    await writer.WriteLineAsync("*** формат: /w ник текст ***");
                    continue;
                }
                string to = rest[..sp];
                string text = rest[(sp + 1)..];

                StreamWriter? target;
                lock (lockObj)
                    target = clients.FirstOrDefault(c => c.Nick == to)?.Writer;

                if (target is null)
                {
                    await writer.WriteLineAsync($"*** пользователь \"{to}\" не найден ***");
                }
                else
                {
                    string pm = $"[{DateTime.Now:HH:mm:ss}] {nick} -> {to} (лично): {text}";
                    await target.WriteLineAsync(pm);
                    await writer.WriteLineAsync(pm);   // отправителю — копия
                    Log($"личное {nick} -> {to}: {text}");
                }
                continue;
            }

            // обычное сообщение — всем
            Log($"{nick}: {line}");
            await BroadcastAsync($"[{DateTime.Now:HH:mm:ss}] {nick}: {line}");
        }
    }
    catch (IOException)
    {
        // клиент оборвал соединение — не падаем, просто чистим за ним
    }
    finally
    {
        if (me != null)
        {
            lock (lockObj) clients.Remove(me);
            Log($"{me.Nick} отключился");
            await BroadcastAsync($"*** {me.Nick} покинул чат ***");
        }
        client.Close();
    }
}

var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
Log($"Сервер запущен на порту {port}. Ctrl+C — остановка.");

while (true)
{
    TcpClient client = await listener.AcceptTcpClientAsync();
    _ = HandleClientAsync(client);   // каждого клиента — в свою задачу
}

// Подключённый клиент: ник + поток для ответа ему.
// (объявления классов — в конце файла, после инструкций верхнего уровня)
class ClientInfo
{
    public required string Nick { get; init; }
    public required StreamWriter Writer { get; init; }
}
