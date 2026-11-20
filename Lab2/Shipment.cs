namespace Lab2;

// Абстрактный базовый класс отправления службы доставки (вариант 9).
// Создать «просто отправление» нельзя — только конкретный тип:
// письмо, посылку или крупногабаритный груз.
public abstract class Shipment
{
    // Свойства только для чтения: менять их после создания нельзя,
    // состояние объекта защищено проверками в конструкторе.
    public string TrackingNumber { get; }
    public string Destination { get; }
    public double WeightKg { get; }
    public bool IsExpress { get; }

    protected Shipment(string trackingNumber, string destination,
                       double weightKg, bool isExpress)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Трек-номер не может быть пустым.", nameof(trackingNumber));
        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentException("Не указан пункт назначения.", nameof(destination));
        if (weightKg <= 0)
            throw new ArgumentOutOfRangeException(nameof(weightKg), "Вес должен быть положительным.");

        TrackingNumber = trackingNumber;
        Destination = destination;
        WeightKg = weightKg;
        IsExpress = isExpress;
    }

    // Стоимость доставки — у каждого типа отправления своя формула,
    // поэтому метод абстрактный: наследник обязан его реализовать.
    public abstract double Cost();

    // Срок доставки в днях: базовое правило — срочная 2 дня, обычная 6.
    // Виртуальный: наследник может переопределить, если у него другие сроки.
    public virtual int DeliveryDays() => IsExpress ? 2 : 6;

    public override string ToString()
        => $"{GetType().Name,-8} {TrackingNumber,-6} -> {Destination,-15} | {WeightKg,6:F1} кг | {Cost(),10:F2} руб. | {DeliveryDays(),2} дн. | {(IsExpress ? "срочная" : "обычная")}";
}
