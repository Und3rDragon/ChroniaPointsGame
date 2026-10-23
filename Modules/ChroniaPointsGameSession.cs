using ChroniaPointsGame.Cores;
using YamlDotNet.Serialization;

namespace ChroniaPointsGame.Modules;

public class ChroniaPointsGameSession : EverestModuleSession
{
    // Chronia Points Game
    public HashSet<string> DiscoveredRooms = new();

    [Note("For Chronia Flag-Counter-Slider System")]
    public HashSet<string> flagsPerRoom = new();
    [Note("For Chronia Flag-Counter-Slider System")]
    public HashSet<string> flagsPerDeath = new();
    [Note("For Chronia Flag-Counter-Slider System")]
    public Dictionary<string, int> countersPerRoom = new();
    [Note("For Chronia Flag-Counter-Slider System")]
    public Dictionary<string, int> countersPerDeath = new();
    [Note("For Chronia Flag-Counter-Slider System")]
    public Dictionary<string, float> slidersPerRoom = new();
    [Note("For Chronia Flag-Counter-Slider System")]
    public Dictionary<string, float> slidersPerDeath = new();
    [Note("For Chronia Flag-Counter-Slider System")]
    public Dictionary<string, string> keystrings = new();
    [Note("For Chronia Flag-Counter-Slider System")]
    public Dictionary<string, ChroniaColor> chroniaColors = new();
}
