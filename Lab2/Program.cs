// Lab2. Вариант 9: служба доставки.
// Иерархия Shipment -> Letter, Parcel, Freight + менеджер DeliveryService.
using Lab2;

// Коллекция базового типа: сюда можно положить объект любого наследника.
var service = new DeliveryService();
service.Add(new Letter("L-1001", "Москва", 0.3, isExpress: false));
service.Add(new Letter("L-1002", "Санкт-Петербург", 0.15, isExpress: true));
service.Add(new Parcel("P-2001", "Казань", 4.5, isExpress: false, isFragile: true));
service.Add(new Parcel("P-2002", "Новосибирск", 12.0, isExpress: true, isFragile: false));
service.Add(new Freight("F-3001", "Екатеринбург", 120, isExpress: false,
                        lengthM: 2.0, widthM: 1.5, heightM: 1.0));
service.Add(new Freight("F-3002", "Владивосток", 450, isExpress: true,
                        lengthM: 3.0, widthM: 2.0, heightM: 1.8));

// Полиморфизм: в цикле работаем через базовый тип, но для каждого
// объекта вызываются его собственные Cost(), DeliveryDays() и ToString().
Console.WriteLine("=== Отправления ===");
foreach (Shipment s in service.Shipments)
    Console.WriteLine(s);

// Статистика менеджера — внутри всё через LINQ над List<Shipment>.
Console.WriteLine($"\nСуммарная выручка: {service.TotalRevenue():F2} руб.");
Console.WriteLine($"Средний вес отправления: {service.AverageWeight():F1} кг");

Console.WriteLine("\n=== Срочные отправления ===");
foreach (Shipment s in service.ExpressShipments())
    Console.WriteLine(s);

Console.WriteLine("\n=== По возрастанию стоимости (IComparable) ===");
foreach (Shipment s in service.SortedByCost())
    Console.WriteLine($"{s.TrackingNumber,-7} {s.Cost(),10:F2} руб.");

Console.WriteLine("\n=== Количество по типам ===");
foreach (var group in service.GroupByType())
    Console.WriteLine($"{group.Key}: {group.Count()} шт., выручка {group.Sum(s => s.Cost()):F2} руб.");

// Демонстрация защиты состояния: некорректные данные отклоняются.
Console.WriteLine("\n=== Проверка корректности ===");
try
{
    var bad = new Letter("L-ERR", "Москва", weightKg: -1, isExpress: false);
}
catch (ArgumentOutOfRangeException e)
{
    Console.WriteLine($"Отрицательный вес отклонён: {e.ParamName}");
}
try
{
    var heavy = new Letter("L-ERR2", "Москва", weightKg: 5, isExpress: false);
}
catch (ArgumentOutOfRangeException e)
{
    Console.WriteLine($"Слишком тяжёлое письмо отклонено: {e.Message}");
}
