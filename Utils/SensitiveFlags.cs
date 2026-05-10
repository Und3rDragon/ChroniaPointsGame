using System.Text.RegularExpressions;

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

    public static Dictionary<string, Sensitivity> Regexs = new()
    {
        { @"^ChroniaPointsGame_Input_.*", Sensitivity.None },
        { @"^ChroniaPointsGame_ConnectedRefill_.*_triggered$", Sensitivity.None },
        { @"^ChroniaPointsGame_ConnectedRefill_.*_consumed$" , Sensitivity.None },
        { @"^ChroniaPointsGame_ConnectedRefill_.*_queue$", Sensitivity.None },
        { @"^ChroniaPointsGame_ConnectedRefill_.*_collect$" , Sensitivity.None },
        { @"^ChroniaPointsGame_Stopclock_.*", Sensitivity.None },
        { @"^ChroniaPointsGame_Language_.*", Sensitivity.AllowNoRegister }
    };
    
    public static bool SensitiveFlagged(this string name, RegexOptions regexExpansion = RegexOptions.None)
    {
        if (Flags.Keys.Contains(name)) { return true; }
        
        foreach(var regex in Regexs.Keys)
        {
            if(Regex.Match(name, regex, regexExpansion).Success)
            {
                return true;
            }
        }
        
        return false;
    }
    
    public static Sensitivity GetSensitivity(this string name, RegexOptions regexExpansion = RegexOptions.None)
    {
        if (Flags.Keys.Contains(name)) { return Flags[name]; }

        foreach (var regex in Regexs.Keys)
        {
            if (Regex.Match(name, regex, regexExpansion).Success)
            {
                return Regexs[regex];
            }
        }

        return Sensitivity.None;
    }

    /// <summary>
    /// ChroniaPointsGame Sensitive Flag level
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
