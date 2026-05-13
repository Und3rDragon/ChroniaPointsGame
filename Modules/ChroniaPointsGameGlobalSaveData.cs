using System;
using System.Numerics;
using ChroniaPointsGame.Cores;

namespace ChroniaPointsGame.Modules;

public class ChroniaPointsGameGlobalSaveData : ChroniaPointsGameModuleGlobalSaveData
{
    public HashSet<string> flags { get; set; }= new();
    public Dictionary<string, int> counters { get; set; }= new();
    public Dictionary<string, float> sliders { get; set; } = new();
    public Dictionary<string, string> permaKeys { get; set; } = new();
    
    public int Fun { get; set; } = 0;

    // Chronia Game SaveData
    [ChroniaGlobalSavePath("Points.yaml")]
    public BigInteger TruePoints { get; set; } = BigInteger.Zero;

    [ChroniaGlobalSavePath("Points.yaml")]
    public DateTime LastUpdateTimeUtc { get; set; } = DateTime.UtcNow;
    [ChroniaGlobalSavePath("Points.yaml")]
    public int MaxStrawberries { get; set; } = 0;
    [ChroniaGlobalSavePath("Points.yaml")]
    public int MaxGoldenStrawberries { get; set; } = 0;
    [ChroniaGlobalSavePath("Points.yaml")]
    public int MaxHeartGems { get; set; } = 0;
    [ChroniaGlobalSavePath("Points.yaml")]
    public int MoonberryGet { get; set; } = 0;
    [ChroniaGlobalSavePath("Points.yaml")]
    public int DashlessBerryGet { get; set; } = 0;
    [ChroniaGlobalSavePath("Points.yaml")]
    public int MaxCassettes { get; set; } = 0;
}