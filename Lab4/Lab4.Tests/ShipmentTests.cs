using Lab4.Core;

namespace Lab4.Tests;

// Тесты классов службы доставки из лабораторной 2.
public class ShipmentTests
{
    // ---- Стоимость по типам ----

    [Fact]
    public void Letter_Cost_Ordinary_ComputesTariff()
    {
        var letter = new Letter("L-1", "Москва", 0.3, isExpress: false);
        Assert.Equal(60 + 40 * 0.3, letter.Cost(), 1e-9);
    }

    [Fact]
    public void Letter_Cost_Express_IsDoubled()
    {
        var letter = new Letter("L-2", "Москва", 0.15, isExpress: true);
        Assert.Equal((60 + 40 * 0.15) * 2, letter.Cost(), 1e-9);
    }

    [Fact]
    public void Parcel_Cost_FragileAndExpress_AppliesBothMultipliers()
    {
        var parcel = new Parcel("P-1", "Казань", 4.5, isExpress: true, isFragile: true);
        Assert.Equal((200 + 90 * 4.5) * 1.25 * 1.5, parcel.Cost(), 1e-9);
    }

    [Fact]
    public void Freight_Cost_IncludesWeightAndVolume()
    {
        var freight = new Freight("F-1", "Екатеринбург", 120, isExpress: false,
                                  lengthM: 2, widthM: 1.5, heightM: 1);
        // 800 + 150*120 + 400*(2*1.5*1)
        Assert.Equal(800 + 150 * 120 + 400 * 3, freight.Cost(), 1e-9);
    }

    // ---- Сроки доставки ----

    [Theory]
    [InlineData(false, 5)]
    [InlineData(true, 1)]
    public void Letter_DeliveryDays_ByUrgency(bool express, int expected)
    {
        var letter = new Letter("L-3", "Москва", 0.1, express);
        Assert.Equal(expected, letter.DeliveryDays());
    }

    [Fact]
    public void Freight_DeliveryDays_LongerThanBaseRule()
    {
        var freight = new Freight("F-2", "Тула", 100, isExpress: false, 1, 1, 1);
        Assert.Equal(14, freight.DeliveryDays()); // у базового правила было бы 6
    }

    // ---- Защита инвариантов ----

    [Fact]
    public void Letter_TooHeavy_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Letter("L-4", "Москва", weightKg: 5, isExpress: false));
    }

    [Fact]
    public void Shipment_InvalidArguments_Throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(   // отрицательный вес
            () => new Parcel("P-2", "Казань", -1, false, false));
        Assert.Throws<ArgumentException>(              // пустой трек-номер
            () => new Letter("", "Москва", 0.1, false));
        Assert.Throws<ArgumentOutOfRangeException>(   // отрицательный габарит
            () => new Freight("F-3", "Тула", 10, false, -2, 1, 1));
    }

    // ---- Полиморфизм и менеджер ----

    [Fact]
    public void Polymorphism_MixedList_TotalIsSumOfAllCosts()
    {
        // Коллекция базового типа — вызывается Cost() каждого конкретного класса.
        List<Shipment> list = [new Letter("L-5", "М", 0.1, false),
                               new Parcel("P-3", "К", 1, false, false),
                               new Freight("F-4", "Т", 10, false, 1, 1, 1)];
        double expected = list[0].Cost() + list[1].Cost() + list[2].Cost();
        double total = 0;
        foreach (Shipment s in list)
            total += s.Cost();
        Assert.Equal(expected, total, 1e-9);
    }

    [Fact]
    public void DeliveryService_TotalRevenue_SumsAllShipments()
    {
        var service = new DeliveryService();
        service.Add(new Letter("L-6", "Москва", 0.3, false));          // 72
        service.Add(new Parcel("P-4", "Казань", 1, false, false));     // 290
        Assert.Equal(72 + 290, service.TotalRevenue(), 1e-9);
    }

    [Fact]
    public void DeliveryService_ExpressShipments_OnlyExpress()
    {
        var service = new DeliveryService();
        service.Add(new Letter("L-7", "Москва", 0.3, false));
        service.Add(new Letter("L-8", "Москва", 0.2, true));
        service.Add(new Parcel("P-5", "Казань", 1, true, false));

        var express = service.ExpressShipments();

        Assert.Equal(2, express.Count);
        Assert.All(express, s => Assert.True(s.IsExpress));
    }

    [Fact]
    public void DeliveryService_Empty_TotalRevenueIsZero()   // пустая коллекция — граничный случай
    {
        Assert.Equal(0, new DeliveryService().TotalRevenue());
    }

    [Fact]
    public void Shipment_Sort_ByCost_Ascending()             // бонус IComparable
    {
        var list = new List<Shipment>
        {
            new Freight("F-5", "Тула", 100, false, 1, 1, 1),   // дорогой
            new Letter("L-9", "Москва", 0.1, false),           // дешёвый
            new Parcel("P-6", "Казань", 1, false, false),      // средний
        };

        list.Sort();

        Assert.Equal("L-9", list[0].TrackingNumber);
        Assert.Equal("F-5", list[2].TrackingNumber);
    }

    [Fact]
    public void Freight_ToString_ContainsDimensions()
    {
        var freight = new Freight("F-6", "Тула", 50, false, 2, 1, 0.5);
        string s = freight.ToString();
        Assert.Contains("x", s);      // габариты в формате ДxШxВ
        Assert.EndsWith(" м", s);
    }
}
