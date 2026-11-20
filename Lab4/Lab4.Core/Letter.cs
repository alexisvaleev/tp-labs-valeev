namespace Lab4.Core;

// Письмо — лёгкая корреспонденция до 2 кг.
public class Letter : Shipment
{
    public const double MaxWeightKg = 2.0;

    public Letter(string trackingNumber, string destination,
                  double weightKg, bool isExpress)
        : base(trackingNumber, destination, weightKg, isExpress)
    {
        if (weightKg > MaxWeightKg)
            throw new ArgumentOutOfRangeException(nameof(weightKg),
                $"Письмо не может весить больше {MaxWeightKg} кг.");
    }

    // Тариф письма: сбор 60 руб. + 40 руб./кг, срочность удваивает цену.
    public override double Cost()
    {
        double cost = 60 + 40 * WeightKg;
        return IsExpress ? cost * 2 : cost;
    }

    // Письма едут быстрее стандартных отправлений.
    public override int DeliveryDays() => IsExpress ? 1 : 5;
}
