// Lab3. Вариант 9: дерево каталога.
// Пользователь задаёт папку, глубину и файл результата;
// программа пишет в файл дерево с псевдографикой, у файлов — размер.
using System.Text;

// UTF-8 и псевдографика — без этой строки Windows-консоль
// может напечатать кракозябры вместо ├ └ │
Console.OutputEncoding = Encoding.UTF8;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Дерево каталога ===");
    Console.WriteLine("1 — записать дерево папки в файл");
    Console.WriteLine("0 — выход");
    Console.Write("> ");

    string? choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1": TaskBuildTree(); break;
        case "0": case null: return;
        default:
            Console.WriteLine("Нет такого пункта. Введите 0 или 1.");
            break;
    }
}

// ------------------- обработка пункта меню -------------------

static void TaskBuildTree()
{
    Console.Write("Путь к папке: ");
    string? folder = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
    {
        Console.WriteLine("Ошибка: такой папки нет.");
        return;
    }

    int depth = ReadInt("Максимальная глубина (0 = без ограничения): ");
    if (depth < 0)
    {
        Console.WriteLine("Ошибка: глубина не может быть отрицательной.");
        return;
    }

    Console.Write("Файл результата (например, tree.txt): ");
    string? outFile = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(outFile))
    {
        Console.WriteLine("Ошибка: не указано имя файла.");
        return;
    }

    try
    {
        int lines = BuildTree(folder, depth, outFile);
        Console.WriteLine($"Готово: записано {lines} строк в файл \"{outFile}\".");
    }
    catch (UnauthorizedAccessException)
    {
        Console.WriteLine("Ошибка: нет доступа к одной из папок или к файлу результата.");
    }
    catch (IOException e)
    {
        Console.WriteLine($"Ошибка ввода-вывода: {e.Message}");
    }
}

// ------------------------- логика -------------------------

// Строит дерево папки folder глубиной maxDepth (0 = без ограничения)
// и пишет его в файл outFile. Возвращает число записанных строк.
static int BuildTree(string folder, int maxDepth, string outFile)
{
    int lines = 0;

    using var writer = new StreamWriter(outFile, false, Encoding.UTF8);
    writer.WriteLine(new DirectoryInfo(folder).Name + Path.DirectorySeparatorChar);
    lines++;

    lines += WriteLevel(folder, prefix: "", depthLeft: maxDepth, writer);
    return lines;
}

// Рекурсивно пишет содержимое одной папки.
// prefix — «отступ» из вертикальных линий для текущего уровня.
// Возвращает число записанных строк.
static int WriteLevel(string folder, string prefix, int depthLeft, StreamWriter writer)
{
    int lines = 0;

    // Если глубина кончилась — ниже не спускаемся.
    // depthLeft == 0 означает «без ограничения», поэтому уменьшаем
    // счётчик только когда он положительный.
    bool unlimited = depthLeft == 0;

    // Сначала папки, потом файлы — так дерево читается привычнее.
    string[] dirs;
    string[] files;
    try
    {
        dirs = Directory.GetDirectories(folder);
        files = Directory.GetFiles(folder);
    }
    catch (UnauthorizedAccessException)
    {
        writer.WriteLine(prefix + "└── [нет доступа]");
        return 1;
    }

    var entries = new List<string>();
    entries.AddRange(dirs);
    entries.AddRange(files);

    for (int i = 0; i < entries.Count; i++)
    {
        bool isLast = i == entries.Count - 1;
        string branch = isLast ? "└── " : "├── ";
        string path = entries[i];

        if (Directory.Exists(path))
        {
            writer.WriteLine(prefix + branch + Path.GetFileName(path) + Path.DirectorySeparatorChar);
            lines++;

            if (unlimited || depthLeft > 1)
            {
                string childPrefix = prefix + (isLast ? "    " : "│   ");
                int nextDepth = unlimited ? 0 : depthLeft - 1;
                lines += WriteLevel(path, childPrefix, nextDepth, writer);
            }
        }
        else
        {
            var info = new FileInfo(path);
            writer.WriteLine(prefix + branch + info.Name + $"  ({FormatSize(info.Length)})");
            lines++;
        }
    }

    return lines;
}

// Человеческий размер файла: Б / КБ / МБ.
static string FormatSize(long bytes)
{
    if (bytes < 1024)
        return $"{bytes} Б";
    if (bytes < 1024 * 1024)
        return $"{bytes / 1024.0:F1} КБ";
    return $"{bytes / 1024.0 / 1024.0:F1} МБ";
}

// Читает целое число; повторяет вопрос при неверном вводе.
static int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int value))
            return value;
        Console.WriteLine("Ошибка: нужно целое число.");
    }
}
