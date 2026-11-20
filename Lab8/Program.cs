using System.Diagnostics;
using System.Text;

const int N = 30_000;

var swTotal = Stopwatch.StartNew();

// --- 1. Генерация данных ---
var rnd = new Random(42);                       // фиксированное зерно: результат воспроизводим
var buyers  = new List<string>();
var amounts = new List<int>();
for (int i = 0; i < N; i++)
{
    buyers.Add("user" + rnd.Next(0, 5000));
    amounts.Add(rnd.Next(1, 10_000));
}

// --- 2. Список уникальных покупателей ---
var sw = Stopwatch.StartNew();
// HashSet вместо List.Contains: поиск O(1) вместо O(n)
var unique = new HashSet<string>();
foreach (string b in buyers)
    unique.Add(b);
Console.WriteLine($"Уникальных покупателей: {unique.Count}  ({sw.ElapsedMilliseconds} мс)");

// --- 3. Текстовый отчёт ---
sw.Restart();
// StringBuilder вместо +=: строка не пересоздаётся на каждой итерации
var report = new StringBuilder();
for (int i = 0; i < buyers.Count; i++)
{
    report.Append(buyers[i]).Append(';').Append(amounts[i]).Append(Environment.NewLine);
}
Console.WriteLine($"Отчёт: {report.Length} символов  ({sw.ElapsedMilliseconds} мс)");

// --- 4. Сортировка сумм ---
sw.Restart();
// встроенная сортировка O(n log n) вместо пузырька O(n²)
var sorted = new List<int>(amounts);
sorted.Sort();
Console.WriteLine($"Медианная сумма: {sorted[sorted.Count / 2]}  ({sw.ElapsedMilliseconds} мс)");

// --- 5. Сумма покупок каждого покупателя ---
sw.Restart();
// один проход с Dictionary вместо двойного цикла O(n·m)
var totals = new Dictionary<string, long>();
for (int i = 0; i < buyers.Count; i++)
{
    totals.TryGetValue(buyers[i], out long total);
    totals[buyers[i]] = total + amounts[i];
}
Console.WriteLine($"Максимум потратил: {totals.MaxBy(t => t.Value).Key}  ({sw.ElapsedMilliseconds} мс)");

Console.WriteLine($"ИТОГО: {swTotal.ElapsedMilliseconds} мс");
