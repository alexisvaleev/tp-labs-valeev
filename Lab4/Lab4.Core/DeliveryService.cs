namespace Lab4.Core;

// БОНУС: класс-менеджер. Хранит коллекцию отправлений и даёт
// по ней статистику — вся работа с коллекцией через LINQ.
public class DeliveryService
{
    private readonly List<Shipment> _shipments = new();

    // Наружу отдаём только чтение — внутренний список снаружи не изменить.
    public IReadOnlyList<Shipment> Shipments => _shipments;

    public void Add(Shipment shipment)
    {
        ArgumentNullException.ThrowIfNull(shipment);
        _shipments.Add(shipment);
    }

    // Суммарная выручка от всех доставок.
    public double TotalRevenue() => _shipments.Sum(s => s.Cost());

    // Средний вес отправления (0 для пустого списка).
    public double AverageWeight()
        => _shipments.Count == 0 ? 0 : _shipments.Average(s => s.WeightKg);

    // Только срочные отправления.
    public List<Shipment> ExpressShipments()
        => _shipments.Where(s => s.IsExpress).ToList();

    // Группировка по типу отправления (Letter / Parcel / Freight).
    public IEnumerable<IGrouping<string, Shipment>> GroupByType()
        => _shipments.GroupBy(s => s.GetType().Name);

    // Отправления, отсортированные по возрастанию стоимости
    // (использует IComparable<Shipment> из базового класса).
    public List<Shipment> SortedByCost()
    {
        var copy = new List<Shipment>(_shipments);
        copy.Sort();
        return copy;
    }
}
