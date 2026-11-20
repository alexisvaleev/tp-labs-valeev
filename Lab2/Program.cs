// Lab2. Вариант 9: служба доставки.
// Иерархия Shipment -> Letter, Parcel, Freight.
using Lab2;

// Коллекция базового типа: сюда можно положить объект любого наследника.
List<Shipment> shipments =
[
    new Letter("L-1001", "Москва", 0.3, isExpress: false),
    new Letter("L-1002", "Санкт-Петербург", 0.15, isExpress: true),
    new Parcel("P-2001", "Казань", 4.5, isExpress: false, isFragile: true),
    new Parcel("P-2002", "Новосибирск", 12.0, isExpress: true, isFragile: false),
    new Freight("F-3001", "Екатеринбург", 120, isExpress: false,
                lengthM: 2.0, widthM: 1.5, heightM: 1.0),
    new Freight("F-3002", "Владивосток", 450, isExpress: true,
                lengthM: 3.0, widthM: 2.0, heightM: 1.8),
];

// Полиморфизм: в цикле работаем через базовый тип, но для каждого
// объекта вызываются его собственные Cost(), DeliveryDays() и ToString().
Console.WriteLine("=== Отправления ===");
foreach (Shipment s in shipments)
    Console.WriteLine(s);

// Тоже полиморфизм: Cost() вызывается без проверок типа.
double total = shipments.Sum(s => s.Cost());
Console.WriteLine($"\nСуммарная стоимость всех доставок: {total:F2} руб.");

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
