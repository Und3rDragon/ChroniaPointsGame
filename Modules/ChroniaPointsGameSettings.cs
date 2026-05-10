using System.Diagnostics;
using System.Reflection.Emit;
using YamlDotNet.Serialization;

namespace ChroniaPointsGame.Modules;

public class ChroniaPointsGameSettings : EverestModuleSettings
{
    public enum DisplayPosition { PlayerBased, StaticScreen }
    public enum Aligning { Left, Middle, Right }

    public class CommonDisplayer
    {
        public bool enabled { get; set; } = false;
        public DisplayPosition displayPosition { get; set; } = DisplayPosition.StaticScreen;
        [SettingRange(-1000, 1000, true)]
        public int X { get; set; } = 160;
        [SettingRange(-1000, 1000, true)]
        public int Y { get; set; } = 168;
        [SettingRange(-32, 32)]
        public int letterDistance { get; set; } = 0;
        [SettingRange(0, 1000, true)]
        public int scale { get; set; } = 60;
        public Aligning aligning { get; set; } = Aligning.Middle;
    }

    [SettingSubMenu]
    public class ChroniaPointsDisplayer : CommonDisplayer
    {
        
    }

    public ChroniaPointsDisplayer chroniaPointsDisplayer { get; set; } = new();
}
