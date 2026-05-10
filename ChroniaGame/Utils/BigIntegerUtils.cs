using System;
using System.Numerics;

namespace ChroniaPointsGame.ChroniaGame.Utils;

public static class BigIntegerUtils
{
    // ========== 辅助：安全转为 BigInteger ==========
    private static BigInteger ToBigInt(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Value cannot be NaN or Infinity.");
        return (BigInteger)Math.Round(value);
    }
    private static BigInteger ToBigInt(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
            throw new ArgumentException("Value cannot be NaN or Infinity.");
        return (BigInteger)Math.Round(value);
    }
    private static BigInteger ToBigInt(int value) => value;
    private static BigInteger ToBigInt(long value) => value;

    // ========== 加法 ==========
    public static BigInteger Add(this BigInteger a, double b) => a + ToBigInt(b);
    public static BigInteger Add(this BigInteger a, float b) => a + ToBigInt(b);
    public static BigInteger Add(this BigInteger a, int b) => a + b;
    public static BigInteger Add(this BigInteger a, long b) => a + b;

    // ========== 减法 ==========
    public static BigInteger Sub(this BigInteger a, double b) => a - ToBigInt(b);
    public static BigInteger Sub(this BigInteger a, float b) => a - ToBigInt(b);
    public static BigInteger Sub(this BigInteger a, int b) => a - b;
    public static BigInteger Sub(this BigInteger a, long b) => a - b;

    // ========== 乘法 ==========
    public static BigInteger Multiply(this BigInteger a, double b) => a * ToBigInt(b);
    public static BigInteger Multiply(this BigInteger a, float b) => a * ToBigInt(b);
    public static BigInteger Multiply(this BigInteger a, int b) => a * b;
    public static BigInteger Multiply(this BigInteger a, long b) => a * b;

    // ========== 除法 ==========
    public static BigInteger Divide(this BigInteger a, double b)
    {
        var divisor = ToBigInt(b);
        if (divisor.IsZero) throw new DivideByZeroException();
        return a / divisor;
    }
    public static BigInteger Divide(this BigInteger a, float b)
    {
        var divisor = ToBigInt(b);
        if (divisor.IsZero) throw new DivideByZeroException();
        return a / divisor;
    }
    public static BigInteger Divide(this BigInteger a, int b)
    {
        if (b == 0) throw new DivideByZeroException();
        return a / b;
    }
    public static BigInteger Divide(this BigInteger a, long b)
    {
        if (b == 0) throw new DivideByZeroException();
        return a / b;
    }

    // ========== 乘方（a ^ b）==========
    public static BigInteger Pow(this BigInteger a, double b)
    {
        var exp = ToBigInt(b);
        if (exp < 0) throw new ArgumentException("Exponent must be non-negative.");
        if (exp > int.MaxValue) throw new ArgumentOutOfRangeException("Exponent too large.");
        return BigInteger.Pow(a, (int)exp);
    }
    public static BigInteger Pow(this BigInteger a, float b)
    {
        var exp = ToBigInt(b);
        if (exp < 0) throw new ArgumentException("Exponent must be non-negative.");
        if (exp > int.MaxValue) throw new ArgumentOutOfRangeException("Exponent too large.");
        return BigInteger.Pow(a, (int)exp);
    }
    public static BigInteger Pow(this BigInteger a, int b)
    {
        if (b < 0) throw new ArgumentException("Exponent must be non-negative.");
        return BigInteger.Pow(a, b);
    }
    public static BigInteger Pow(this BigInteger a, long b)
    {
        if (b < 0) throw new ArgumentException("Exponent must be non-negative.");
        if (b > int.MaxValue) throw new ArgumentOutOfRangeException("Exponent too large.");
        return BigInteger.Pow(a, (int)b);
    }

    // ========== 平方根（整数近似，向下取整）==========
    public static BigInteger Sqrt(this BigInteger a)
    {
        if (a < 0) throw new ArgumentException("Cannot compute square root of negative number.");
        if (a.IsZero || a.IsOne) return a;

        // 牛顿法求整数平方根
        BigInteger x = a;
        BigInteger y = (x + 1) / 2;
        while (y < x)
        {
            x = y;
            y = (x + a / x) / 2;
        }
        return x;
    }

    // ========== 自然对数（整数近似，向下取整）==========
    public static BigInteger Ln(this BigInteger a)
    {
        if (a <= 0) throw new ArgumentException("Logarithm undefined for non-positive values.");
        if (a == 1) return BigInteger.Zero;

        // 转为 double 计算（仅适用于 a 不太大的情况）
        // 若 a 太大（> double.MaxValue），需用其他方法（此处简化）
        try
        {
            double logVal = Math.Log((double)a);
            return (BigInteger)Math.Floor(logVal);
        }
        catch (OverflowException)
        {
            // 极大数处理：估算位数
            // ln(a) ≈ log10(a) * ln(10) ≈ (bitLength * log10(2)) * 2.302585
            double bitLen = BigInteger.Log(a, 2) + 1;
            double approx = bitLen * Math.Log(2); // ln(a) ≈ bitLen * ln(2)
            return (BigInteger)Math.Floor(approx);
        }
    }

    // ========== OUT 形式：Add ==========
    public static void Add(this BigInteger a, double b, out BigInteger result) => result = a.Add(b);
    public static void Add(this BigInteger a, float b, out BigInteger result) => result = a.Add(b);
    public static void Add(this BigInteger a, int b, out BigInteger result) => result = a + b;
    public static void Add(this BigInteger a, long b, out BigInteger result) => result = a + b;

    // ========== OUT 形式：Sub ==========
    public static void Sub(this BigInteger a, double b, out BigInteger result) => result = a.Sub(b);
    public static void Sub(this BigInteger a, float b, out BigInteger result) => result = a.Sub(b);
    public static void Sub(this BigInteger a, int b, out BigInteger result) => result = a - b;
    public static void Sub(this BigInteger a, long b, out BigInteger result) => result = a - b;

    // ========== OUT 形式：Multiply ==========
    public static void Multiply(this BigInteger a, double b, out BigInteger result) => result = a.Multiply(b);
    public static void Multiply(this BigInteger a, float b, out BigInteger result) => result = a.Multiply(b);
    public static void Multiply(this BigInteger a, int b, out BigInteger result) => result = a * b;
    public static void Multiply(this BigInteger a, long b, out BigInteger result) => result = a * b;

    // ========== OUT 形式：Divide ==========
    public static void Divide(this BigInteger a, double b, out BigInteger result) => result = a.Divide(b);
    public static void Divide(this BigInteger a, float b, out BigInteger result) => result = a.Divide(b);
    public static void Divide(this BigInteger a, int b, out BigInteger result) => result = a / b;
    public static void Divide(this BigInteger a, long b, out BigInteger result) => result = a / b;

    // ========== OUT 形式：Pow ==========
    public static void Pow(this BigInteger a, double b, out BigInteger result) => result = a.Pow(b);
    public static void Pow(this BigInteger a, float b, out BigInteger result) => result = a.Pow(b);
    public static void Pow(this BigInteger a, int b, out BigInteger result) => result = BigInteger.Pow(a, b);
    public static void Pow(this BigInteger a, long b, out BigInteger result) => result = a.Pow(b);

    // ========== OUT 形式：Sqrt ==========
    public static void Sqrt(this BigInteger a, out BigInteger result) => result = a.Sqrt();

    // ========== OUT 形式：Ln ==========
    public static void Ln(this BigInteger a, out BigInteger result) => result = a.Ln();
}