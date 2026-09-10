using System.Numerics;
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
        if (Instance.IsNotNull())
        {
            Instance.Buffer?.Dispose();
            self.Remove(Instance);

            Instance = null;
        }

        orig(self);
    }

    public Displayers(EntityData d, Vc2 o) : base(d, o)
    {
        Tag |= Tags.Global;
    }

    protected override void HDRender()
    {
        if (!ChroniaPointsGameModule.Settings.chroniaPointsDisplayer.enabled) { return; }

        var displayer = ChroniaPointsGameModule.Settings.chroniaPointsDisplayer;
        var displayUI = chroniaPoints_UI;

        displayUI.origin = ((int)displayer.aligning + 4).ToJustify();
        displayUI.distance = displayer.letterDistance;
        displayUI.scale = displayer.scale * 0.1f;

        displayUI.Render(GetDisplayTarget(), GeneralSelector, GetRenderPosition(displayer.displayPosition,
            new Vc2(displayer.X, displayer.Y)));
    }

    // ---- 显示文本缓存：积分与倍率都没变化时复用上一次拼好的字符串 ----
    private BigInteger cachedPoints;
    private double cachedOverallBase;
    private float cachedOverallPow;
    private bool hasCachedTarget = false;
    private string cachedTarget;

    private string GetDisplayTarget()
    {
        BigInteger points = CPGame.ParsedPoints;
        double overallBase = CPGame.OverallBase;
        float overallPow = CPGame.OverallPow;

        if (hasCachedTarget
            && points == cachedPoints
            && overallBase == cachedOverallBase
            && overallPow == cachedOverallPow)
        {
            return cachedTarget;
        }

        cachedPoints = points;
        cachedOverallBase = overallBase;
        cachedOverallPow = overallPow;
        hasCachedTarget = true;
        cachedTarget = BuildDisplayTarget(points, overallBase, overallPow);

        return cachedTarget;
    }

    private static string BuildDisplayTarget(BigInteger points, double overallBase, float overallPow)
    {
        string num = points.ToString();
        if(num.Length >= 18)
        {
            int n = num.Length / 3;
            if(num.Length - 3* n == 0) { n = n - 1; }

            string sub1 = num.Substring(0, num.Length - 3 * n);
            string sub2 = num.Substring(num.Length - 3 * n, 2);

            num = $"{sub1}.{sub2}E+{3 * n}";
        }

        string target = $"{num} Chronia Points";
        if(overallBase != 1)
        {
            target = target + $" (x{overallBase:0.00}";
        }
        if(overallPow != 1)
        {
            target = target + $"P{overallPow}";
        }
        target = target + ")";

        return target;
    }

    // ---- 静态字符映射与 selector：固定引用 + O(1) 查表，避免每字符两次线性扫描 ----
    private static readonly int[] GeneralCharIndex = BuildGeneralCharIndex();
    private static readonly Func<char, int> GeneralSelector = static c =>
    {
        int idx = GeneralCharIndex[c];
        return idx >= 0 ? idx : GeneralCharIndex[' '];
    };

    private static int[] BuildGeneralCharIndex()
    {
        var arr = new int[char.MaxValue + 1];
        Array.Fill(arr, -1);
        string reference = Cons.DisplayFontsReference;
        for (int i = 0; i < reference.Length; i++)
        {
            if (arr[reference[i]] == -1) { arr[reference[i]] = i; }
        }
        // 保证空格映射存在
        arr[' '] = reference.IndexOf(' ');

        return arr;
    }

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
