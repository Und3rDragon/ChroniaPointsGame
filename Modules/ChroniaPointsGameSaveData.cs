using ChroniaPointsGame.Cores;

namespace ChroniaPointsGame.Modules;

public class ChroniaPointsGameSaveData : EverestModuleSaveData
{
    // Flag Timer Trigger
    public Dictionary<string, float> FlagTimerS = new();

    [Note("For Chronia Flag-Counter-Slider System")]
    public HashSet<string> flags = new();
    [Note("For Chronia Flag-Counter-Slider System")]
    public Dictionary<string, int> counters = new();
    [Note("For Chronia Flag-Counter-Slider System")]
    public Dictionary<string, float> sliders = new();
    [Note("For Chronia Flag-Counter-Slider System")]
    public Dictionary<string, string> keystrings = new();
    [Note("For Chronia Flag-Counter-Slider System")]
    public Dictionary<string, ChroniaColor> chroniaColors = new();
}
