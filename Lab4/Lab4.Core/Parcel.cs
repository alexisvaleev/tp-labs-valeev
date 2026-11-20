namespace Lab4.Core;

// Посылка — отправление среднего веса; бывает хрупкой.
public class Parcel : Shipment
{
    public bool IsFragile { get; }

    public Parcel(string trackingNumber, string destination,
                  double weightKg, bool isExpress, bool isFragile)
        : base(trackingNumber, destination, weightKg, isExpress)
    {
        IsFragile = isFragile;
    }

    // Тариф посылки: сбор 200 руб. + 90 руб./кг;
    // хрупкая +25%, срочная +50%.
    public override double Cost()
    {
        double cost = 200 + 90 * WeightKg;
        if (IsFragile)
            cost *= 1.25;
        if (IsExpress)
            cost *= 1.5;
        return cost;
    }
}
