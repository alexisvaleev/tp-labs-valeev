// Lab5. Вариант 9: модель касс супермаркета.
// Покупатели встают в общую очередь; N потоков-касс её разбирают.
// Время обслуживания случайное (1–10 у.е.), новый покупатель — каждые 3–5 у.е.
// Остановка симуляции — по клавише Enter.
using System.Diagnostics;

const int UnitMs = 200;   // одна условная единица времени = 200 мс

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Модель касс супермаркета ===");
    Console.WriteLine("1 — симуляция (с выводом состояния)");
    Console.WriteLine("2 — сравнение по числу касс (таблица)");
    Console.WriteLine("0 — выход");
    Console.Write("> ");

    string? choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1": TaskSimulate(); break;
        case "2": TaskCompare(); break;
        case "0": case null: return;
        default:
            Console.WriteLine("Нет такого пункта. Введите число от 0 до 2.");
            break;
    }
}

// ------------------------- обработчики меню -------------------------

static void TaskSimulate()
{
    int cashiers = ReadInt("Число касс: ", 1, 16);
    int seconds = ReadInt("Длительность симуляции (сек): ", 5, 600);

    SimResult r = Simulate(cashiers, UnitMs, seconds, live: true);

    Console.WriteLine();
    Console.WriteLine($"Пришло покупателей:  {r.Arrived}");
    Console.WriteLine($"Обслужено:           {r.Served}");
    Console.WriteLine($"Осталось в очереди:  {r.Arrived - r.Served}");
    Console.WriteLine($"Средняя длина очереди: {r.AvgQueue:F1}");
}

static void TaskCompare()
{
    int seconds = ReadInt("Длительность одного прогона (сек): ", 10, 600);

    Console.WriteLine();
    Console.WriteLine($"{"Кассы",6} | {"Обслужено",9} | {"Ср. очередь",11} | {"Ускорение",9}");
    Console.WriteLine(new string('-', 47));

    int first = -1;
    foreach (int n in new[] { 1, 2, 3, 4, 6, 8 })
    {
        // тихий прогон: единица времени короче, чтобы не ждать вечность
        SimResult r = Simulate(n, unitMs: 10, seconds, live: false);
        if (first < 0) first = r.Served;
        double speedup = (double)r.Served / first;
        Console.WriteLine($"{n,6} | {r.Served,9} | {r.AvgQueue,11:F1} | {speedup,9:F2}x");
    }
}

// ------------------------- сама симуляция -------------------------

// Гонит симуляцию runSeconds секунд и возвращает статистику.
// live = true — поток-наблюдатель печатает состояние каждые 2 секунды,
// а Enter останавливает симуляцию досрочно.
static SimResult Simulate(int cashierCount, int unitMs, int runSeconds, bool live)
{
    var queue = new Queue<int>();        // общая очередь покупателей
    var queueLock = new object();        // замок для потокобезопасного доступа к ней
    var served = new int[cashierCount];  // сколько обслужила каждая касса
    int arrived = 0;
    double queueLenSum = 0;
    int queueSamples = 0;

    // БОНУС: остановка через CancellationToken — корректный способ
    // «попросить поток завершиться», без грубого Thread.Abort.
    using var cts = new CancellationTokenSource();

    // Поток-генератор: новый покупатель каждые 3–5 у.е.
    var generator = new Thread(() =>
    {
        var rnd = new Random();
        while (!cts.IsCancellationRequested)
        {
            Thread.Sleep(rnd.Next(3, 6) * unitMs);
            lock (queueLock)
            {
                arrived++;
                queue.Enqueue(arrived);
            }
        }
    });
    generator.Start();

    // Потоки-кассы.
    var cashiers = new Thread[cashierCount];
    for (int i = 0; i < cashierCount; i++)
    {
        int id = i;
        cashiers[i] = new Thread(() =>
        {
            var rnd = new Random();
            while (!cts.IsCancellationRequested)
            {
                int customer;
                lock (queueLock)
                    customer = queue.Count > 0 ? queue.Dequeue() : -1;

                if (customer < 0)
                {
                    Thread.Sleep(10);   // очередь пуста — ждём, не грузим процессор
                    continue;
                }

                Thread.Sleep(rnd.Next(1, 11) * unitMs); // обслуживание 1–10 у.е.
                served[id]++;
            }
        });
        cashiers[i].Start();
    }

    // Поток-наблюдатель: замеряет длину очереди каждые 100 мс
    // (для средней), а на экран выводит состояние раз в 2 секунды.
    var watcher = new Thread(() =>
    {
        var sw = Stopwatch.StartNew();
        long lastPrint = -2000;
        while (!cts.IsCancellationRequested)
        {
            Thread.Sleep(100);
            lock (queueLock)
            {
                queueLenSum += queue.Count;
                queueSamples++;
                if (live && sw.ElapsedMilliseconds - lastPrint >= 2000)
                {
                    lastPrint = sw.ElapsedMilliseconds;
                    Console.WriteLine($"[{sw.Elapsed:mm\\:ss}] очередь: {queue.Count,3} | " +
                                      $"обслужено по кассам: {string.Join(", ", served)}");
                }
            }
        }
    });
    watcher.Start();

    if (live)
    {
        Console.WriteLine("Enter — остановить симуляцию досрочно.");
        // Ждём либо конец времени, либо клавишу — опросом KeyAvailable,
        // чтобы не оставлять отдельный поток висеть на ReadLine.
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < runSeconds * 1000)
        {
            if (!Console.IsInputRedirected && Console.KeyAvailable)
            {
                Console.ReadKey(intercept: true);
                break;
            }
            Thread.Sleep(100);
        }
    }
    else
    {
        Thread.Sleep(runSeconds * 1000);
    }
    cts.Cancel();

    generator.Join();
    foreach (var t in cashiers)
        t.Join();
    watcher.Join();

    lock (queueLock)
    {
        return new SimResult
        {
            Arrived = arrived,
            Served = served.Sum(),
            AvgQueue = queueSamples == 0 ? 0 : queueLenSum / queueSamples,
        };
    }
}

// Читает целое в диапазоне; повторяет вопрос при неверном вводе.
static int ReadInt(string prompt, int min, int max)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
            return value;
        Console.WriteLine($"Ошибка: нужно целое от {min} до {max}.");
    }
}

// Результат прогона симуляции (объявление класса — в конце файла,
// после инструкций верхнего уровня: иначе не скомпилируется).
class SimResult
{
    public int Arrived;
    public int Served;
    public double AvgQueue;
}
