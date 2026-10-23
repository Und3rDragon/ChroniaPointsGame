using System.Numerics;
using ChroniaPointsGame.Cores;
using ChroniaPointsGame.Modules;
using ChroniaPointsGame.Utils;

namespace ChroniaPointsGame.Settings;

/// <summary>
/// 在游戏 HUD 上显示当前的 Chronia Points 与倍率。
/// 位置、字号、字距与对齐方式由 Mod 设置中的显示选项控制。
/// </summary>
[Tracked(true)]
public class Displayers : Entity
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
            self.Remove(Instance);

            Instance = null;
        }

        orig(self);
    }

    public Displayers(EntityData d, Vc2 o) : base(d.Position + o)
    {
        Tag |= Tags.HUD | Tags.Global;
    }

    public override void Render()
    {
        var displayer = ChroniaPointsGameModule.Settings.chroniaPointsDisplayer;
        if (!displayer.enabled) { return; }

        string target = GetDisplayTarget();
        if (string.IsNullOrEmpty(target)) { return; }

        DrawText(target, GetRenderPosition(displayer.displayPosition, new Vc2(displayer.X, displayer.Y)),
            ((int)displayer.aligning + 4).ToJustify(), displayer.scale * 0.1f, displayer.letterDistance);
    }

    /// <summary>
    /// 以 <paramref name="position"/> 为锚点绘制 HUD 文本。
    /// <paramref name="justify"/> 决定文本相对锚点的对齐方式，<paramref name="letterDistance"/> 为字间距。
    /// </summary>
    private void DrawText(string text, Vc2 position, Vc2 justify, float scale, float letterDistance)
    {
        EnsureLayout(text);

        Vc2 anchor = new Vc2(position.X, position.Y - cachedTextHeight * scale * justify.Y);
        Vc2 textScale = Vc2.One * scale;

        if (letterDistance == 0f)
        {
            //ActiveFont.Draw(text, anchor, new Vc2(justify.X, 0f), textScale, Color.White);
            ActiveFont.DrawOutline(text, anchor, new Vc2(justify.X, 0f), textScale, Color.White, 2f, Color.Black);
            return;
        }

        float cursor = anchor.X - (cachedTotalAdvance * scale + letterDistance * (text.Length - 1)) * justify.X;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] != ' ')
            {
                //ActiveFont.Draw(text[i], new Vc2(cursor, anchor.Y), Vc2.Zero, textScale, Color.White);
                ActiveFont.DrawOutline(text[i].ToString(), new Vc2(cursor, anchor.Y), Vc2.Zero, textScale, Color.White, 2f, Color.Black);
            }

            cursor += cachedAdvances[i] * scale + letterDistance;
        }
    }

    // ---- 文本布局：内容不变时复用上一次的测量结果 ----
    private string cachedLayoutText;
    private float[] cachedAdvances;
    private int cachedAdvanceCount;
    private float cachedTotalAdvance;
    private float cachedTextHeight;

    private void EnsureLayout(string text)
    {
        if (cachedLayoutText == text && cachedAdvanceCount == text.Length) { return; }

        if (cachedAdvances == null || cachedAdvances.Length < text.Length)
        {
            cachedAdvances = new float[text.Length];
        }

        float total = 0f;
        for (int i = 0; i < text.Length; i++)
        {
            float advance = ActiveFont.Measure(text[i]).X;
            cachedAdvances[i] = advance;
            total += advance;
        }

        cachedAdvanceCount = text.Length;
        cachedTotalAdvance = total;
        cachedTextHeight = ActiveFont.Measure(text).Y;
        cachedLayoutText = text;
    }

    // ---- 显示文本：积分与倍率都没变化时复用上一次拼好的字符串 ----
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

    /// <summary>
    /// 把显示位置换算成 HUD 坐标：跟随玩家时按玩家与相机的相对位置，固定在屏幕时直接取设置值。
    /// </summary>
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
