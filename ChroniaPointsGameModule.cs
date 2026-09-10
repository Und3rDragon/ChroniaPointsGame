using Celeste.Mod.Helpers;
using ChroniaPointsGame.ChroniaGame.Utils;
using ChroniaPointsGame.Cores;
using ChroniaPointsGame.Modules;
using ChroniaPointsGame.Utils;
using System.Numerics;

namespace ChroniaPointsGame;

public class ChroniaPointsGameModule : EverestModule
{

    public const string Name = "ChroniaPointsGame";

    public static ChroniaPointsGameModule Instance { get; private set; }

    public override Type SettingsType => typeof(ChroniaPointsGameSettings);

    public static ChroniaPointsGameSettings Settings => (ChroniaPointsGameSettings)ChroniaPointsGameModule.Instance._Settings;

    public override Type SessionType => typeof(ChroniaPointsGameSession);

    public static ChroniaPointsGameSession Session => (ChroniaPointsGameSession)ChroniaPointsGameModule.Instance._Session;

    public override Type SaveDataType => typeof(ChroniaPointsGameSaveData);

    public static ChroniaPointsGameSaveData SaveData => (ChroniaPointsGameSaveData)ChroniaPointsGameModule.Instance._SaveData;

    public static ChroniaPointsGameGlobalSaveData GlobalData { get; private set; }
    
    public string ModDirectory
    {
        get => Path.Combine(Path.GetDirectoryName(FakeAssembly.GetFakeEntryAssembly().Location), $"Mods\\{ChroniaPointsGameModule.Name}");
    }

    public ChroniaPointsGameModule()
    {
        Instance = this;
    }

    public static bool InstanceReady => Session != null && SaveData != null;

    public static DateTime lastAutoSaveTime = DateTime.UtcNow;
    
    public static bool CheckDependency(string modName, string minimumVersion)
    {
        EverestModuleMetadata meta = new()
        {
            Name = modName,
            Version = new Version(minimumVersion)
        };

        return Everest.Loader.DependencyLoaded(meta);
    }

    public static bool CheckDependency(string modName, string minimumVersion,
        out EverestModule module)
    {
        EverestModuleMetadata meta = new()
        {
            Name = modName,
            Version = new Version(minimumVersion)
        };

        return Everest.Loader.TryGetDependency(meta, out module);
    }

    public override void Load()
    {
        Log.Info("Welcome to Chronia Points Game!");
        ChroniaPointsGameModule.Instance = this;

        GlobalData = new ChroniaPointsGameGlobalSaveData();
        GlobalData.LoadAll(); // 读取全局存档数据
        
        On.Celeste.Celeste.OnExiting += OnGameExiting;
        
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        LoadingManager.Load();

        // ChroniaGame
        InitializeChroniaPointsGame();
        
        CalculateOfflineIncome(); // 结算离线收益

        GlobalData.Fun++;
        Log.Info($"You've been here for {GlobalData.Fun} times, eh?");
    }

    public override void Unload()
    {
        Log.Error("Unload");
        
        LoadingManager.Unload();

        On.Celeste.Celeste.OnExiting -= OnGameExiting;
    }

    private static bool IsFromHelpers(ModAsset asset)
    {
        if (GlobalData.IsNull()) { return false; }

        return false;
    }

    private static void OnGameExiting(On.Celeste.Celeste.orig_OnExiting orig, Celeste.Celeste self, object sender, EventArgs args)
    {
        GlobalData.SaveAll();
        
        orig(self, sender, args);
    }

    // 游戏异常退出时执行紧急存档
    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            Logger.Log(LogLevel.Error, "ChroniaPointsGame", "Emergency save due to crash...");
            GlobalData?.SaveAll();
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, "ChroniaPointsGame", $"Emergency save failed: {ex}");
        }
    }

    public static List<SaveSummary> summaries = new();
    private void InitializeChroniaPointsGame()
    {
        int berries = 0, goldens = 0, cassettes = 0, gems = 0;
        int get9a = 0, get1a = 0;

        // Old - Analyze whole game data
        
        //SaveDataReader.AnalyzeAllLocalSaves().EachDo((item) =>
        //{
        //    berries = int.Max(berries, item.TotalStrawberries);
        //    goldens = int.Max(item.GoldenStrawberries, goldens);
        //    cassettes = int.Max(cassettes, item.Cassettes);
        //    gems = int.Max(gems, item.CrystalHearts);
        //    get9a.TryPositive(item.HasFarewellMoonBerry);
        //    get1a.TryPositive(item.HasPrologueNoDashGolden);
        //});

        summaries = SaveDataReader.AnalyzeAllLocalSaves();
        summaries.EachDo((item) =>
        {
            berries += item.TotalStrawberries;
            goldens += item.GoldenStrawberries;
            cassettes += item.Cassettes;
            gems += item.CrystalHearts;
            get9a += item.HasFarewellMoonBerry ? 1 : 0;
            get1a += item.HasPrologueNoDashGolden ? 1 : 0;

            //Log.Info(item.Slot);
            //Log.Info(item.TotalStrawberries);
            //Log.Info(item.GoldenStrawberries);
            //Log.Info(item.Cassettes);
            //Log.Info(item.CrystalHearts);
            //Log.Info(item.HasFarewellMoonBerry);
            //Log.Info(item.HasPrologueNoDashGolden);
        });

        GlobalData.MaxStrawberries = berries;
        GlobalData.MaxGoldenStrawberries = goldens;
        GlobalData.MaxCassettes = cassettes;
        GlobalData.MaxHeartGems = gems;
        GlobalData.MoonberryGet = get9a;
        GlobalData.DashlessBerryGet = get1a;

        //Log.Info(Md.GlobalData.MaxStrawberries, ChroniaPointsUtils.StrawberryMultiplier);
        //Log.Info(Md.GlobalData.MaxGoldenStrawberries, ChroniaPointsUtils.GoldenMultiplier);
        //Log.Info(Md.GlobalData.MaxCassettes, ChroniaPointsUtils.CassetteMultiplier);
        //Log.Info(Md.GlobalData.MaxHeartGems, ChroniaPointsUtils.HeartGemMultiplier);
        //Log.Info(Md.GlobalData.MoonberryGet, ChroniaPointsUtils.MoonBerryMultiplier);
        //Log.Info(Md.GlobalData.DashlessBerryGet, ChroniaPointsUtils.DashlessBerryMultiplier);
        //Log.Warn(ChroniaPointsUtils.OverallMultiplier);
        //Log.Warn(Log.divider);
    }
    
    // Chronia Game
    // ===== 离线收益结算 =====
    private void CalculateOfflineIncome()
    {
        var now = DateTime.UtcNow;
        var offlineSeconds = (now - GlobalData.LastUpdateTimeUtc).TotalSeconds;

        // 最多只补 7 天
        offlineSeconds = Math.Max(0, Math.Min(offlineSeconds, 7 * 24 * 3600));

        if (offlineSeconds > 0)
        {
            GlobalData.TruePoints = GlobalData.TruePoints.AddPoints(offlineSeconds * CPGame.OfflineMultiplier);
            Logger.Log("ChroniaPointsGame", $"Added offline income: {(BigInteger)(offlineSeconds * CPGame.OfflineMultiplier)} points over {offlineSeconds:F1}s");
        }

        GlobalData.LastUpdateTimeUtc = now;
        GlobalData.SaveAll(); // 立即保存，防止重复累加
    }
}
