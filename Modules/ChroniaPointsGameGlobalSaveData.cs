using System;
using System.Numerics;
using ChroniaPointsGame.Cores;

namespace ChroniaPointsGame.Modules;

public class ChroniaPointsGameGlobalSaveData : ChroniaPointsGameModuleGlobalSaveData
{
    [ChroniaGlobalSavePath]
    public HashSet<string> flags = new();
    [ChroniaGlobalSavePath]
    public Dictionary<string, int> counters = new();
    [ChroniaGlobalSavePath]
    public Dictionary<string, float> sliders = new();
    [ChroniaGlobalSavePath]
    public Dictionary<string, string> permaKeys = new();
    
    [ChroniaGlobalSavePath]
    public int Fun { get; set; } = 0;

    // Chronia Game SaveData
    [ChroniaGlobalSavePath("ChroniaPointsGame/Points.xml")]
    public BigInteger TruePoints { get; set; } = BigInteger.Zero;

    [ChroniaGlobalSavePath("ChroniaPointsGame/Points.xml")]
    public DateTime LastUpdateTimeUtc { get; set; } = DateTime.UtcNow;
    [ChroniaGlobalSavePath("ChroniaPointsGame/Points.xml")]
    public int MaxStrawberries { get; set; } = 0;
    [ChroniaGlobalSavePath("ChroniaPointsGame/Points.xml")]
    public int MaxGoldenStrawberries { get; set; } = 0;
    [ChroniaGlobalSavePath("ChroniaPointsGame/Points.xml")]
    public int MaxHeartGems { get; set; } = 0;
    [ChroniaGlobalSavePath("ChroniaPointsGame/Points.xml")]
    public int MoonberryGet { get; set; } = 0;
    [ChroniaGlobalSavePath("ChroniaPointsGame/Points.xml")]
    public int DashlessBerryGet { get; set; } = 0;
    [ChroniaGlobalSavePath("ChroniaPointsGame/Points.xml")]
    public int MaxCassettes { get; set; } = 0;
}