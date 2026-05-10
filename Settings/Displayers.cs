using ChroniaPointsGame.Modules;
using ChroniaPointsGame.Cores;
using ChroniaPointsGame.Cores.Graphical;
using ChroniaPointsGame.Utils;

namespace ChroniaPointsGame.Settings;

[Tracked(true)]
public class Displayers : HDRenderEntity
{
    [LoadHook]
    public static void Load()
    {
        On.Celeste.Level.Begin += OnLevelBegin;
        On.Celeste.Level.End += OnLevelEnd;
    }
    [UnloadHook]
    public static void Unload()
    {
        On.Celeste.Level.Begin -= OnLevelBegin;
        On.Celeste.Level.End -= OnLevelEnd;
    }

    public static Displayers Instance = null;
    public static void OnLevelBegin(On.Celeste.Level.orig_Begin orig, Level self)
    {
        orig(self);

        self.Add(Instance = new Displayers(new EntityData(), Vc2.Zero));
    }

    public static void OnLevelEnd(On.Celeste.Level.orig_End orig, Level self)
    {
        Instance.Buffer?.Dispose();
        self.Remove(Instance);

        orig(self);
    }

    public Displayers(EntityData d, Vc2 o) : base(d, o)
    {
        Tag |= Tags.Global;
    }

    protected override void HDRender()
    {
        if (ChroniaPointsGameModule.Settings.chroniaPointsDisplayer.enabled)
        {
            var displayer = ChroniaPointsGameModule.Settings.chroniaPointsDisplayer;
            var displayUI = chroniaPoints_UI;
            
            string num = CPGame.ParsedPoints.ToString();
            if(num.Length >= 18)
            {
                int n = num.Length / 3;
                if(num.Length - 3* n == 0) { n = n - 1; }

                string sub1 = num.Substring(0, num.Length - 3 * n);
                string sub2 = num.Substring(num.Length - 3 * n, 2);

                num = $"{sub1}.{sub2}E+{3 * n}";
            }
            
            string target = $"{num} Chronia Points";
            if(CPGame.OverallBase != 1)
            {
                target = target + $" (x{CPGame.OverallBase:0.00}";
            }
            if(CPGame.OverallPow != 1)
            {
                target = target + $"P{CPGame.OverallPow}";
            }
            target = target + ")";

            displayUI.origin = ((int)displayer.aligning + 4).ToJustify();
            displayUI.distance = displayer.letterDistance;
            displayUI.scale = displayer.scale * 0.1f;

            displayUI.Render(target, (c) =>
            {
                return generalReference.Contains(c) ? generalReference.IndexOf(c) : generalReference.IndexOf(" ");
            }, GetRenderPosition(displayer.displayPosition,
                new Vc2(displayer.X, displayer.Y)));
        }
    }

    public string generalReference = Cons.DisplayFontsReference;

    public SerialImageRaw chroniaPoints_UI = new SerialImageRaw(GFX.Game.GetAtlasSubtextures("ChroniaPointsGame/DisplayFonts/font"))
    {
        segmentOrigin = Vc2.Zero,
    };
    
    public Vc2 GetRenderPosition(ChroniaPointsGameSettings.DisplayPosition pos, Vc2 setup)
    {
        if (pos == ChroniaPointsGameSettings.DisplayPosition.PlayerBased)
        {
            Vc2 p = new Vc2((int)(PUt.player?.Center.X ?? 0), (int)(PUt.player?.Center.Y ?? 0)) + setup;
            return (p - MaP.cameraPos) * Cons.HDScale;
        }
        else if (pos == ChroniaPointsGameSettings.DisplayPosition.StaticScreen)
        {
            return setup * Cons.HDScale;
        }

        return Vc2.Zero;
    }
}
