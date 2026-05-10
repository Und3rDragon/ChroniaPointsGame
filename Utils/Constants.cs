global using CColor = ChroniaPointsGame.Utils.ColorUtils.ChroniaColor;
global using ChroniaColor = ChroniaPointsGame.Utils.ColorUtils.ChroniaColor;
global using Cons = ChroniaPointsGame.Utils.Constants;
global using HSL = ChroniaPointsGame.Utils.ColorUtils.HSLColor;
global using HSLColor = ChroniaPointsGame.Utils.ColorUtils.HSLColor;
global using HSV = ChroniaPointsGame.Utils.ColorUtils.HSVColor;
global using HSVColor = ChroniaPointsGame.Utils.ColorUtils.HSVColor;
global using MaP = ChroniaPointsGame.Cores.MapProcessor; // usual
global using Md = ChroniaPointsGame.ChroniaPointsGameModule; // usual
global using Sav = ChroniaPointsGame.Modules.ChroniaPointsGameSaveData; // usual
global using Ses = ChroniaPointsGame.Modules.ChroniaPointsGameSession; // usual
global using Sts = ChroniaPointsGame.Modules.ChroniaPointsGameSettings; // usual
global using Vc2 = Microsoft.Xna.Framework.Vector2;
global using Vc3 = Microsoft.Xna.Framework.Vector3;
global using Sens = ChroniaPointsGame.Utils.SensitiveFlags.Sensitivity;
global using Prm = ChroniaPointsGame.Cores.Graphical.GraphicalParams;
global using GSav = ChroniaPointsGame.Modules.ChroniaPointsGameGlobalSaveData; // usual
global using CPGame = ChroniaPointsGame.ChroniaGame.Utils.ChroniaPointsUtils;
global using PUt = ChroniaPointsGame.Utils.PlayerUtils;

using System.Reflection;

namespace ChroniaPointsGame.Utils;

public static class Constants
{
    public const float piOverEight = MathHelper.PiOver4 / 2f;
    public const float eightPi = 4 * MathHelper.TwoPi;

    public const string BgTileListDynamicDataName = "BgTileList";

    public const float TeraBlockLiftBoostMultipler = 4f;
    
    public const string EntityStringId = "ChroniaPointsGame/PlayerIndicatorZone";
    public const string EntityStringId2 = "ChroniaPointsGame/PlayerIndicatorZoneCustom";

    public const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
    
    public const string DisplayFontsReference = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+-*/.<>()[]{}'\"?!\\:; =,_^";

    public const string CommandStopclockID = "ChroniaPointsGame_Debug_CommandStopclock";

    public const float HDScale = 6f;
}