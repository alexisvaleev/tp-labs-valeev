using System.Numerics;
using Lab4.Core;

namespace Lab4.Tests;

// Тесты алгоритмов из лабораторной 1.
// Имена: ЧтоТестируем_ПриКакихУсловиях_ЧтоОжидаем.
public class MathAlgorithmsTests
{
    // ---- Factorial ----

    [Fact]
    public void Factorial_OfZero_ReturnsOne()
    {
        // Arrange + Act
        var result = MathAlgorithms.Factorial(0);
        // Assert
        Assert.Equal(BigInteger.One, result);
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(5, "120")]
    [InlineData(10, "3628800")]
    [InlineData(20, "2432902008176640000")]
    [InlineData(25, "15511210043330985984000000")] // BigInteger: за пределом long
    public void Factorial_ValidN_ReturnsExpected(int n, string expected)
    {
        var result = MathAlgorithms.Factorial(n);
        Assert.Equal(BigInteger.Parse(expected), result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Factorial_NegativeN_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MathAlgorithms.Factorial(n));
    }

    // ---- FibonacciUpTo ----

    [Fact]
    public void FibonacciUpTo_N10_ReturnsSequenceUpToN()
    {
        var result = MathAlgorithms.FibonacciUpTo(10);
        Assert.Equal(new long[] { 0, 1, 1, 2, 3, 5, 8 }, result);
    }

    [Fact]
    public void FibonacciUpTo_Zero_ReturnsOnlyZero()
    {
        var result = MathAlgorithms.FibonacciUpTo(0);
        Assert.Equal(new long[] { 0 }, result);
    }

    [Fact]
    public void FibonacciUpTo_NegativeN_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MathAlgorithms.FibonacciUpTo(-1));
    }

    // ---- FunctionA (вариант 9) ----

    [Fact]
    public void FunctionA_AtZero_MatchesHandCalculation()
    {
        // A = sin(19/(x-5)) * ch(e^x + e^(4x)); при x=0: sin(-3.8) * ch(2)
        double expected = Math.Sin(-3.8) * Math.Cosh(2.0);
        Assert.Equal(expected, MathAlgorithms.FunctionA(0), 1e-10);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(1)]
    [InlineData(1.5)]
    public void FunctionA_RegularPoints_ReturnsFinite(double x)
    {
        Assert.True(double.IsFinite(MathAlgorithms.FunctionA(x)));
    }

    [Fact]
    public void FunctionA_AtX5_ReturnsNaN()   // граница области определения
    {
        Assert.True(double.IsNaN(MathAlgorithms.FunctionA(5)));
    }

    // ch(e^x + e^(4x)) растёт двойной экспонентой и переполняет double
    // уже примерно при x > 1.6 — функция определена, но не представима.
    [Theory]
    [InlineData(4.9)]
    [InlineData(1000)]
    public void FunctionA_OverflowedExponent_IsNotFinite(double x)
    {
        Assert.False(double.IsFinite(MathAlgorithms.FunctionA(x)));
    }

    // ---- LnSeries (вариант 9) ----

    [Theory]
    [InlineData(0.5)]
    [InlineData(-0.5)]
    [InlineData(0.9)]
    [InlineData(1.0)]            // правая граница области сходимости
    public void LnSeries_InsideDomain_MatchesMathLog(double x)
    {
        var (sum, terms) = MathAlgorithms.LnSeries(x, 1e-6);
        Assert.Equal(Math.Log(1 + x), sum, 1e-5);
        Assert.True(terms > 0);
    }

    [Theory]
    [InlineData(-1.0)]           // левая граница — ряд расходится
    [InlineData(2.0)]
    public void LnSeries_OutsideDomain_Throws(double x)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MathAlgorithms.LnSeries(x, 1e-6));
    }

    [Fact]
    public void LnSeries_ZeroEps_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MathAlgorithms.LnSeries(0.5, 0));
    }
}
