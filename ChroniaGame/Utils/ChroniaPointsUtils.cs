using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod.Core;
using ChroniaPointsGame.Cores;
using ChroniaPointsGame.Utils;

namespace ChroniaPointsGame.ChroniaGame.Utils;

public static class ChroniaPointsUtils
{
    // Systematic
    public static readonly BigInteger SCALE = 10000; // 4 位小数精度
    public static BigInteger ParsedPoints => (ChroniaPointsGameModule.GlobalData?.TruePoints / SCALE) ?? BigInteger.Zero;

    public static DateTime LastCollectableUpdate = DateTime.Now;
    public static int LastStrawberryUpdated = 0;
    public static int LastGoldenUpdated = 0;
    public static int LastCassetteUpdated = 0;
    public static int LastHeartUpdated = 0;

    // Game Setting
    public const float OfflineMultiplier = 0.7f;
    public static double LevelMultiplier = 1.2;

    public static bool cheating = false;

    // Single Collectable Multipliers
    public static bool AnyCollectableBonus = true;
    public static double AnyStrawberryBonus => 0.1 * OverallMultiplier;
    public static double AnyGoldenBerryBonus => 0.2 * OverallMultiplier;
    public static double AnyCassetteBonus => 0.5 * OverallMultiplier;
    public static double AnyHeartBonus => 0.5 * OverallMultiplier;
    
    // Vanilla Multipliers
    public static float StrawberryMultiplier => ChroniaPointsGameModule.GlobalData.MaxStrawberries * 0.01f; // 1.75f
    public static float GoldenMultiplier => ChroniaPointsGameModule.GlobalData.MaxGoldenStrawberries * 0.1f; // 2.5f
    public static float HeartGemMultiplier => ChroniaPointsGameModule.GlobalData.MaxHeartGems * 0.02f; // 0.48f
    public static float MoonBerryMultiplier => ChroniaPointsGameModule.GlobalData.MoonberryGet * 0.1f;
    public static float DashlessBerryMultiplier => ChroniaPointsGameModule.GlobalData.DashlessBerryGet * 0.1f;
    public static float CassetteMultiplier => ChroniaPointsGameModule.GlobalData.MaxCassettes * 0.015f; // 0.12f
    public static float _OverallBonus()
    {
        return StrawberryMultiplier + GoldenMultiplier + HeartGemMultiplier
            + MoonBerryMultiplier + DashlessBerryMultiplier + CassetteMultiplier;
    }
    public static float OverallBonus => _OverallBonus();

    public static double _OverallMultiplier()
    {
        return cheating? 1f : LevelMultiplier * (1f + OverallBonus);
    }
    public static double OverallBase => _OverallMultiplier();
    public static float OverallPow = 1f;
    public static double OverallMultiplier => 
        OverallPow == 1f ? OverallBase : Math.Pow(OverallBase, OverallPow);

    // 可选：格式化显示（如 "1234.5678"）
    public static string DisplayPoints
    {
        get
        {
            var integerPart = ChroniaPointsGameModule.GlobalData.TruePoints / SCALE;
            var fractionalPart = (ChroniaPointsGameModule.GlobalData.TruePoints % SCALE).ToString("D4"); // 补前导零到4位
            return $"{integerPart}.{fractionalPart}";
        }
    }
    
    // ========== 辅助：将任意数值转为 scaled BigInteger ==========
    private static BigInteger ToScaled(double value)
        => (BigInteger)Math.Round(value * (double)SCALE);

    private static BigInteger ToScaled(float value)
        => (BigInteger)Math.Round(value * (double)SCALE);

    private static BigInteger ToScaled(int value)
        => new BigInteger(value) * SCALE;

    private static BigInteger ToScaled(long value)
        => new BigInteger(value) * SCALE;

    // ========== 加法 ==========
    public static BigInteger AddPoints(this BigInteger original, double value)
        => original + ToScaled(value);

    public static BigInteger AddPoints(this BigInteger original, float value)
        => original + ToScaled(value);

    public static BigInteger AddPoints(this BigInteger original, int value)
        => original + ToScaled(value);

    public static BigInteger AddPoints(this BigInteger original, long value)
        => original + ToScaled(value);

    // ========== 减法 ==========
    public static BigInteger RemovePoints(this BigInteger original, double value)
        => original - ToScaled(value);

    public static BigInteger RemovePoints(this BigInteger original, float value)
        => original - ToScaled(value);

    public static BigInteger RemovePoints(this BigInteger original, int value)
        => original - ToScaled(value);

    public static BigInteger RemovePoints(this BigInteger original, long value)
        => original - ToScaled(value);

    // ========== 乘法（倍率）==========
    public static BigInteger MultiplyPoints(this BigInteger original, double multiplier)
    {
        if (multiplier < 0) throw new ArgumentException("Multiplier cannot be negative.");
        var factor = (BigInteger)Math.Round(multiplier * (double)SCALE);
        return (original * factor) / SCALE;
    }

    public static BigInteger MultiplyPoints(this BigInteger original, float multiplier)
    {
        if (multiplier < 0) throw new ArgumentException("Multiplier cannot be negative.");
        var factor = (BigInteger)Math.Round(multiplier * (double)SCALE);
        return (original * factor) / SCALE;
    }

    public static BigInteger MultiplyPoints(this BigInteger original, int multiplier)
        => original * multiplier; // 整数倍率无需缩放

    public static BigInteger MultiplyPoints(this BigInteger original, long multiplier)
        => original * multiplier;

    // ========== 除法（除以倍率）==========
    public static BigInteger DividePoints(this BigInteger original, double divisor)
    {
        if (divisor <= 0) throw new ArgumentException("Divisor must be positive.");
        var factor = (BigInteger)Math.Round(divisor * (double)SCALE);
        return (original * SCALE) / factor;
    }

    public static BigInteger DividePoints(this BigInteger original, float divisor)
    {
        if (divisor <= 0) throw new ArgumentException("Divisor must be positive.");
        var factor = (BigInteger)Math.Round(divisor * (double)SCALE);
        return (original * SCALE) / factor;
    }

    public static BigInteger DividePoints(this BigInteger original, int divisor)
    {
        if (divisor <= 0) throw new ArgumentException("Divisor must be positive.");
        return original / divisor;
    }

    public static BigInteger DividePoints(this BigInteger original, long divisor)
    {
        if (divisor <= 0) throw new ArgumentException("Divisor must be positive.");
        return original / divisor;
    }

    // ========== OUT 形式：加法 ==========
    public static void AddPoints(this BigInteger original, double value, out BigInteger result)
        => result = original.AddPoints(value);
    public static void AddPoints(this BigInteger original, float value, out BigInteger result)
        => result = original.AddPoints(value);
    public static void AddPoints(this BigInteger original, int value, out BigInteger result)
        => result = original.AddPoints(value);
    public static void AddPoints(this BigInteger original, long value, out BigInteger result)
        => result = original.AddPoints(value);

    // ========== OUT 形式：减法 ==========
    public static void RemovePoints(this BigInteger original, double value, out BigInteger result)
        => result = original.RemovePoints(value);
    public static void RemovePoints(this BigInteger original, float value, out BigInteger result)
        => result = original.RemovePoints(value);
    public static void RemovePoints(this BigInteger original, int value, out BigInteger result)
        => result = original.RemovePoints(value);
    public static void RemovePoints(this BigInteger original, long value, out BigInteger result)
        => result = original.RemovePoints(value);

    // ========== OUT 形式：乘法 ==========
    public static void MultiplyPoints(this BigInteger original, double multiplier, out BigInteger result)
        => result = original.MultiplyPoints(multiplier);
    public static void MultiplyPoints(this BigInteger original, float multiplier, out BigInteger result)
        => result = original.MultiplyPoints(multiplier);
    public static void MultiplyPoints(this BigInteger original, int multiplier, out BigInteger result)
        => result = original.MultiplyPoints(multiplier);
    public static void MultiplyPoints(this BigInteger original, long multiplier, out BigInteger result)
        => result = original.MultiplyPoints(multiplier);

    // ========== OUT 形式：除法 ==========
    public static void DividePoints(this BigInteger original, double divisor, out BigInteger result)
        => result = original.DividePoints(divisor);
    public static void DividePoints(this BigInteger original, float divisor, out BigInteger result)
        => result = original.DividePoints(divisor);
    public static void DividePoints(this BigInteger original, int divisor, out BigInteger result)
        => result = original.DividePoints(divisor);
    public static void DividePoints(this BigInteger original, long divisor, out BigInteger result)
        => result = original.DividePoints(divisor);
}
