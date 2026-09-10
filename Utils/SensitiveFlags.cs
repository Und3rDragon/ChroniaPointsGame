using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AsmResolver.DotNet.Memory;

namespace ChroniaPointsGame.Utils;

public static class SensitiveFlags
{
    public static Dictionary<string, Sensitivity> Flags = new()
    {
        { "ChroniaPointsGame_PlayerCollidingBGTiles", Sensitivity.None },
        { "ChroniaPointsGame_PlayerTouchingTriggers", Sensitivity.None },
        { "ChroniaPointsGame_PlayerCollidingEntitiesAbove", Sensitivity.None },
        { "ChroniaPointsGame_PlayerCollidingEntitiesBelow", Sensitivity.None },
        { "ChroniaPointsGame_PlayerCollidingEntitiesWithSameDepth", Sensitivity.None },
    };

    // 预编译正则：一次性编译缓存，热路径直接 IsMatch，避免每次调用重新解析编译
    private static readonly List<(Regex Regex, Sensitivity Sensitivity)> Regexs = new()
    {
        (new Regex(@"^ChroniaPointsGame_Input_.*", RegexOptions.Compiled), Sensitivity.None),
        (new Regex(@"^ChroniaPointsGame_ConnectedRefill_.*_triggered$", RegexOptions.Compiled), Sensitivity.None),
        (new Regex(@"^ChroniaPointsGame_ConnectedRefill_.*_consumed$", RegexOptions.Compiled), Sensitivity.None),
        (new Regex(@"^ChroniaPointsGame_ConnectedRefill_.*_queue$", RegexOptions.Compiled), Sensitivity.None),
        (new Regex(@"^ChroniaPointsGame_ConnectedRefill_.*_collect$", RegexOptions.Compiled), Sensitivity.None),
        (new Regex(@"^ChroniaPointsGame_Stopclock_.*", RegexOptions.Compiled), Sensitivity.None),
        (new Regex(@"^ChroniaPointsGame_Language_.*", RegexOptions.Compiled), Sensitivity.AllowNoRegister),
    };

    // 所有敏感 flag 均以此为前缀，用于对普通 flag 快速短路，跳过全部正则匹配
    private const string SensitivePrefix = "ChroniaPointsGame_";

    public static bool SensitiveFlagged(this string name)
    {
        if (Flags.ContainsKey(name)) { return true; }
        if (!name.StartsWith(SensitivePrefix)) { return false; }

        foreach (var (regex, _) in Regexs)
        {
            if (regex.IsMatch(name)) { return true; }
        }

        return false;
    }

    public static Sensitivity GetSensitivity(this string name)
    {
        if (Flags.TryGetValue(name, out var sensitivity)) { return sensitivity; }
        if (!name.StartsWith(SensitivePrefix)) { return Sensitivity.None; }

        foreach (var (regex, sens) in Regexs)
        {
            if (regex.IsMatch(name)) { return sens; }
        }

        return Sensitivity.None;
    }

    /// <summary>
    /// ChroniaHelper Sensitive Flag level
    /// </summary>
    /// 
    [Flags]
    public enum Sensitivity
    {
        None = 0,
        AllowNoRegister = 1 << 0, // Pow(2,n) == 1 << n, bit形式
        AllowNoSetFlag = 1 << 1,
    }
}
