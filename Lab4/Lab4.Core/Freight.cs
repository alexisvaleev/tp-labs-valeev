namespace Lab4.Core;

// Крупногабаритный груз — кроме веса учитываются габариты.
public class Freight : Shipment
{
    public double LengthM { get; }
    public double WidthM { get; }
    public double HeightM { get; }

    // Вычисляемое свойство: объём всегда согласован с габаритами.
    public double VolumeM3 => LengthM * WidthM * HeightM;

    public Freight(string trackingNumber, string destination,
                   double weightKg, bool isExpress,
                   double lengthM, double widthM, double heightM)
        : base(trackingNumber, destination, weightKg, isExpress)
    {
        if (lengthM <= 0 || widthM <= 0 || heightM <= 0)
            throw new ArgumentOutOfRangeException(nameof(lengthM),
                "Все габариты должны быть положительными.");

        LengthM = lengthM;
        WidthM = widthM;
        HeightM = heightM;
    }

    // Тариф крупногабарита: сбор 800 руб. + 150 руб./кг + 400 руб./м³,
    // срочность умножает цену в 2,5 раза.
    public override double Cost()
    {
        double cost = 800 + 150 * WeightKg + 400 * VolumeM3;
        return IsExpress ? cost * 2.5 : cost;
    }

    // Габаритный груз в любом случае едет долго.
    public override int DeliveryDays() => IsExpress ? 5 : 14;

    public override string ToString()
        => base.ToString() + $" | {LengthM:F1}x{WidthM:F1}x{HeightM:F1} м";
}
