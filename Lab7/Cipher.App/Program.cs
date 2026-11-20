using Cipher.Core;
using ConsoleTables;

// Лабораторная 7, вариант 9 — «Шифратор текстов».
// Консольное приложение: только меню и ввод-вывод,
// вся логика живёт в библиотеке Cipher.Core.

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Шифратор текстов ===");

// табличка доступных шифров — NuGet-пакет ConsoleTables
var table = new ConsoleTable("N", "Шифр", "Что делает");
table.AddRow(1, "Цезарь", "сдвиг букв на N позиций");
table.AddRow(2, "Виженер", "сдвиг по ключевому слову");
table.Write();

CipherService service = CreateService();

while (true)
{
    Console.WriteLine();
    Console.WriteLine($"Шифр: {service.Cipher.Name}. Что делаем?");
    Console.WriteLine("1 — зашифровать текст");
    Console.WriteLine("2 — расшифровать текст");
    Console.WriteLine("3 — зашифровать файл");
    Console.WriteLine("4 — расшифровать файл");
    Console.WriteLine("5 — сменить шифр");
    Console.WriteLine("0 — выход");
    Console.Write("Выбор: ");
    string choice = Console.ReadLine() ?? "";

    if (choice == "0")
        break;
    if (choice == "5")
    {
        service = CreateService();
        continue;
    }

    if (choice == "1" || choice == "2")
    {
        Console.Write("Текст: ");
        string text = Console.ReadLine() ?? "";
        Console.WriteLine("Результат: " +
            (choice == "1" ? service.Encrypt(text) : service.Decrypt(text)));
    }
    else if (choice == "3" || choice == "4")
    {
        Console.Write("Входной файл: ");
        string input = Console.ReadLine() ?? "";
        Console.Write("Выходной файл: ");
        string output = Console.ReadLine() ?? "";
        try
        {
            if (choice == "3")
                service.EncryptFile(input, output);
            else
                service.DecryptFile(input, output);
            Console.WriteLine("Готово, записано в " + output);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.WriteLine("Ошибка файла: " + ex.Message);
        }
    }
    else
    {
        Console.WriteLine("Неизвестный пункт.");
    }
}

// спрашиваем у пользователя шифр и ключ, собираем сервис
CipherService CreateService()
{
    while (true)
    {
        Console.Write("Номер шифра: ");
        string choice = Console.ReadLine() ?? "";
        if (choice == "1")
            return new CipherService(new CaesarCipher(ReadInt("Сдвиг (например 3): ")),
                                     new FileTextStorage());
        if (choice == "2")
        {
            Console.Write("Ключевое слово (буквы): ");
            try
            {
                return new CipherService(new VigenereCipher(Console.ReadLine() ?? ""),
                                         new FileTextStorage());
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Такого шифра нет, попробуйте ещё.");
        }
    }
}

int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int n))
            return n;
        Console.WriteLine("Нужно целое число.");
    }
}
