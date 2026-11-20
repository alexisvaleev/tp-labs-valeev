// Lab1. Вариант 9.
// Консольное приложение с меню из четырёх заданий.
// Вся логика — в отдельных методах, консоль — только ввод/вывод.
using System.Globalization;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Лабораторная 1. Вариант 9 ===");
    Console.WriteLine("1 — факториал числа n");
    Console.WriteLine("2 — числа Фибоначчи от 0 до n");
    Console.WriteLine("3 — функция A = sin(19/(x-5)) * ch(e^x + e^(4x))");
    Console.WriteLine("4 — сумма ряда ln(1+x) с точностью 1e-6");
    Console.WriteLine("0 — выход");
    Console.Write("> ");

    string? choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1": TaskFactorial(); break;
        case "2": TaskFibonacci(); break;
        case "3": TaskFunctionA(); break;
        case "4": TaskLnSeries(); break;
        case "0": case null: return;      // 0 или конец ввода — выходим
        default:
            Console.WriteLine("Нет такого пункта. Введите число от 0 до 4.");
            break;
    }
}

// ====================== ВСПОМОГАТЕЛЬНЫЙ ВВОД ======================

// Читает целое число; повторяет вопрос, пока ввод не станет корректным.
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

// Читает вещественное число; принимает и запятую (русская локаль),
// и точку (инвариантная культура) как разделитель.
static double ReadDouble(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? s = Console.ReadLine();
        if (double.TryParse(s, out double value) ||
            double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            return value;
        Console.WriteLine("Ошибка: нужно вещественное число.");
    }
}

// ========================== ЗАДАНИЕ 1 ==========================

static void TaskFactorial()
{
    int n = ReadInt("Введите n (0..20): ");
    if (n < 0 || n > 20)
    {
        Console.WriteLine("Ошибка: нужно целое от 0 до 20 (21! не помещается в long).");
        return;
    }
    Console.WriteLine($"{n}! = {Factorial(n)}");
}

// Факториал итеративно. Ограничение n <= 20: 21! уже не помещается в long.
static long Factorial(int n)
{
    long result = 1;
    for (int i = 2; i <= n; i++)
        result *= i;
    return result;
}

// ========================== ЗАДАНИЕ 2 ==========================

static void TaskFibonacci()
{
    int n = ReadInt("Введите n: ");
    if (n < 0)
    {
        Console.WriteLine("Ошибка: n должно быть неотрицательным.");
        return;
    }
    Console.WriteLine(string.Join(", ", FibonacciUpTo(n)));
}

// Числа Фибоначчи от 0 до n включительно.
static List<long> FibonacciUpTo(long n)
{
    var result = new List<long>();
    long a = 0, b = 1;
    while (a <= n)
    {
        result.Add(a);
        long next = a + b;
        a = b;
        b = next;
    }
    return result;
}

// ========================== ЗАДАНИЕ 3 ==========================

static void TaskFunctionA()
{
    double x = ReadDouble("Введите x: ");
    double a = FunctionA(x);
    if (!double.IsFinite(a))
        Console.WriteLine("Функция не определена при данном x (деление на ноль или переполнение).");
    else
        Console.WriteLine($"A = {a}");
}

// Вариант 9: A = sin(19/(x-5)) * ch(e^x + e^(4x)),
// ch(t) — гиперболический косинус, Math.Cosh.
// При x = 5 деление на ноль; при x ≳ 178 аргумент ch переполняет double.
// В обоих случаях метод честно возвращает NaN/Infinity, а вызывающий
// код сообщает, что функция не определена.
static double FunctionA(double x)
{
    if (x == 5)
        return double.NaN;

    double sinArg = 19.0 / (x - 5.0);
    double chArg = Math.Exp(x) + Math.Exp(4.0 * x);
    return Math.Sin(sinArg) * Math.Cosh(chArg);
}

// ========================== ЗАДАНИЕ 4 ==========================

static void TaskLnSeries()
{
    const double eps = 1e-6;
    double x = ReadDouble("Введите x (-1 < x <= 1): ");
    if (x <= -1 || x > 1)
    {
        Console.WriteLine("Ошибка: x должен удовлетворять -1 < x <= 1.");
        return;
    }

    var (sum, terms) = LnSeries(x, eps);
    double reference = Math.Log(1 + x);

    Console.WriteLine($"Сумма ряда:           {sum}");
    Console.WriteLine($"Math.Log(1+x):        {reference}");
    Console.WriteLine($"Разница:              {Math.Abs(sum - reference):E3}");
    Console.WriteLine($"Просуммировано членов: {terms}");
}

// Вариант 9: ln(1+x) = x - x^2/2 + x^3/3 - ... , для -1 < x <= 1.
// Суммируем, пока очередной член по модулю больше eps.
// Общий член ряда: t_k = (-1)^(k+1) * x^k / k.
static (double Sum, int Terms) LnSeries(double x, double eps)
{
    double sum = 0;
    int k = 0;
    while (true)
    {
        k++;
        double term = Math.Pow(x, k) / k;
        if (k % 2 == 0)
            term = -term;
        if (Math.Abs(term) <= eps)
            break;
        sum += term;
    }
    return (sum, k - 1);
}
