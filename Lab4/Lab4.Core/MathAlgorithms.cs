using System.Numerics;

namespace Lab4.Core;

// Алгоритмы из лабораторной 1 (вариант 9), перенесённые
// в библиотеку, чтобы их можно было покрыть unit-тестами.
public static class MathAlgorithms
{
    // Факториал через BigInteger (бонус лабораторной 1):
    // переполнения нет, поэтому верхней границы 20 нет.
    public static BigInteger Factorial(int n)
    {
        BigInteger result = 1;
        for (int i = 2; i <= n; i++)
            result *= i;
        return result;
    }

    // Числа Фибоначчи от 0 до n включительно.
    public static List<long> FibonacciUpTo(long n)
    {
        if (n < 0)
            throw new ArgumentOutOfRangeException(nameof(n), "n должно быть неотрицательным.");

        var result = new List<long>();
        long a = 0, b = 1;
        while (a <= n)
        {
            result.Add(a);
            long next = a + b;
            a = b;
            b = next;
        }
        return result;
    }

    // Вариант 9: A = sin(19/(x-5)) * ch(e^x + e^(4x)).
    // Возвращает NaN/Infinity там, где функция не определена
    // или не помещается в double — вызывающий код решает, что сообщить.
    public static double FunctionA(double x)
    {
        if (x == 5)
            return double.NaN;

        double sinArg = 19.0 / (x - 5.0);
        double chArg = Math.Exp(x) + Math.Exp(4.0 * x);
        return Math.Sin(sinArg) * Math.Cosh(chArg);
    }

    // Вариант 9: ln(1+x) = x - x^2/2 + x^3/3 - ..., для -1 < x <= 1.
    // Бонус лабораторной 1: члены считаются рекуррентно —
    // t_{k+1} = t_k * (-x) * k/(k+1), без Math.Pow на каждой итерации.
    public static (double Sum, int Terms) LnSeries(double x, double eps)
    {
        if (x <= -1 || x > 1)
            throw new ArgumentOutOfRangeException(nameof(x), "Ряд определён при -1 < x <= 1.");
        if (eps <= 0)
            throw new ArgumentOutOfRangeException(nameof(eps), "Точность должна быть положительной.");

        double term = x;
        double sum = 0;
        int k = 0;
        while (Math.Abs(term) > eps)
        {
            sum += term;
            k++;
            term *= -x * k / (k + 1.0);
        }
        return (sum, k);
    }
}
