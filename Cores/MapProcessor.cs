using ChroniaPointsGame.Utils;
using ChroniaPointsGame.Utils.ChroniaSystem;
using Microsoft.Xna.Framework.Input;
using MonoMod.Utils;

namespace ChroniaPointsGame.Cores;

public static class MapProcessor
{
    [LoadHook]
    public static void Load()
    {
        On.Celeste.SaveData.Start += OnSaveDataStart;
        On.Celeste.Level.LoadLevel += OnLevelLoadLevel;
    }

    [UnloadHook]
    public static void Unload()
    {
        On.Celeste.SaveData.Start -= OnSaveDataStart;
        On.Celeste.Level.LoadLevel -= OnLevelLoadLevel;
    }

    // Variables

    public static AreaKey areakey;
    public static MapData mapdata;
    public static int saveSlotIndex;
    public static Level level = null;

    public static Vector2 camOffset = Vector2.Zero;

    // Session
    
    public static Dictionary<string, Session.Slider> sliders = new();

    // Hooks

    public static Tuple<int, SaveData> currentSaveData = null;
    private static void OnSaveDataStart(On.Celeste.SaveData.orig_Start orig, SaveData self, int index)
    {
        orig(self, index);

        currentSaveData = new (index, self);
    }

    public static int totalMaxID = -1;
    private static void OnLevelLoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level level, Player.IntroTypes intro, bool isFromLoader)
    {
        orig(level,  intro, isFromLoader);
        
        MaP.level = level;
        camOffset = level.CameraOffset;
        mapdata = level.Session.MapData;
        areakey = level.Session.MapData.Area;

        // Apply Session Dynamic Data
        object _slider = new DynamicData(level.Session).Get("_Sliders");
        sliders = (Dictionary<string, Session.Slider>)_slider;
    }
    // Shortcuts and Properties

    public static Scene scene => Monocle.Engine.Scene;
    public static Level sceneAsLevel => scene is Level ? scene as Level : null;
    public static MouseState mouseState => Mouse.GetState();
    public static KeyboardState keyboardState => Keyboard.GetState();
    public static GamePadState gamePadState => GamePad.GetState(PlayerIndex.One);

    public static Vc2 cameraPos => level?.Camera.Position ?? Vc2.Zero;
    public static Vc2 cameraCenter => cameraPos + new Vc2(160f, 90f);
    public static Vc2 levelPos => new Vc2(level?.Bounds.Left ?? 0, level?.Bounds.Top ?? 0);
    public static Vc2 InParallax(this Vc2 position, Vc2 parallax)
    {
        return cameraCenter + (position - cameraCenter) * parallax;
    }
    public static Vc2 InGlobalParallax(this Vc2 position, Vc2 parallax)
    {
        return position.InParallax(parallax) + levelPos;
    }
    public static Vc2 InParallax(this Vc2 position, Vc2 parallax, Vc2 staticScreen)
    {
        Vc2 inparallax = position.InParallax(parallax);
        float X = parallax.X == 0 ? cameraPos.X + staticScreen.X : inparallax.X;
        float Y = parallax.Y == 0 ? cameraPos.Y + staticScreen.Y : inparallax.Y;
        return new Vc2(X, Y);
    }
    public static Vc2 InGlobalParallax(this Vc2 position, Vc2 parallax, Vc2 staticScreen)
    {
        Vc2 inglobalparallax = position.InGlobalParallax(parallax);
        float X = parallax.X == 0 ? cameraPos.X + staticScreen.X : inglobalparallax.X;
        float Y = parallax.Y == 0 ? cameraPos.Y + staticScreen.Y : inglobalparallax.Y;
        return new Vc2(X, Y);
    }
    public static Vc2 CameraPos(this Level level) => level?.Camera.Position ?? Vc2.Zero;
    public static Vc2 CameraCenter(this Level level) => CameraPos(level) + new Vc2(160f, 90f);
    public static Vc2 LevelPos(this Level level) => new Vc2(level?.Bounds.Left ?? 0, level?.Bounds.Top ?? 0);
    public static Vc2 CameraPos(this Scene scene) => (scene as Level)?.Camera.Position ?? Vc2.Zero;
    public static Vc2 CameraCenter(this Scene scene) => CameraPos(scene as Level) + new Vc2(160f, 90f);
    public static Vc2 LevelPos(this Scene scene) => new Vc2((scene as Level)?.Bounds.Left ?? 0, (scene as Level)?.Bounds.Top ?? 0);
    
}
