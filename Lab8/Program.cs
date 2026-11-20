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
var sorted = new List<int>(amounts);
for (int i = 0; i < sorted.Count - 1; i++)
    for (int j = 0; j < sorted.Count - i - 1; j++)
        if (sorted[j] > sorted[j + 1])
            (sorted[j], sorted[j + 1]) = (sorted[j + 1], sorted[j]);
Console.WriteLine($"Медианная сумма: {sorted[sorted.Count / 2]}  ({sw.ElapsedMilliseconds} мс)");

// --- 5. Сумма покупок каждого покупателя ---
sw.Restart();
var totals = new List<(string Buyer, long Total)>();
foreach (string b in unique)
{
    long total = 0;
    for (int i = 0; i < buyers.Count; i++)
        if (buyers[i] == b)
            total += amounts[i];
    totals.Add((b, total));
}
Console.WriteLine($"Максимум потратил: {totals.MaxBy(t => t.Total).Buyer}  ({sw.ElapsedMilliseconds} мс)");

Console.WriteLine($"ИТОГО: {swTotal.ElapsedMilliseconds} мс");
