using ChroniaPointsGame.ChroniaGame.Utils;
using ChroniaPointsGame.Cores;
using ChroniaPointsGame.Utils;

namespace ChroniaPointsGame.ChroniaGame;

public class PointsSystem
{
    [LoadHook]
    public static void Load()
    {
        On.Monocle.Scene.Update += SceneUpdate;
        On.Celeste.SaveData.RegisterCompletion += OnLevelComplete;
        On.Celeste.SaveData.AddStrawberry_AreaKey_EntityID_bool += BerryAdding;
        On.Celeste.SaveData.RegisterHeartGem += CollectingHeartGem;
        On.Celeste.SaveData.RegisterCassette += RegisterCassette;
        On.Celeste.Level.LoadLevel += LevelLoad;
        On.Celeste.SaveData.TryDelete += DeleteFile;
    }
    [UnloadHook]
    public static void Unload()
    {
        On.Monocle.Scene.Update -= SceneUpdate;
        On.Celeste.SaveData.RegisterCompletion -= OnLevelComplete;
        On.Celeste.SaveData.AddStrawberry_AreaKey_EntityID_bool -= BerryAdding;
        On.Celeste.SaveData.RegisterHeartGem -= CollectingHeartGem;
        On.Celeste.SaveData.RegisterCassette -= RegisterCassette;
        On.Celeste.Level.LoadLevel -= LevelLoad;
        On.Celeste.SaveData.TryDelete -= DeleteFile;
    }
    
    public static void RegisterCassette(On.Celeste.SaveData.orig_RegisterCassette orig, SaveData save, AreaKey area)
    {
        orig(save, area);
        
        if(save.FileSlot == -1) { return; }

        bool upgrade = CPGame.AnyCollectableBonus;
        bool registered = save.Areas[area.ID].Cassette;
        bool freq = CPGame.LastCassetteUpdated < 10;
        
        bool collectArg = upgrade && !registered && freq;
        
        if (area.ID > 10)
        {
            if (collectArg)
            {
                ChroniaPointsGameModule.GlobalData.TruePoints.AddPoints(CPGame.AnyCassetteBonus);
                CPGame.LastCassetteUpdated++;
            }
            
            return;
        }

        ChroniaPointsGameModule.summaries.Create(save.FileSlot, new(slot: save.FileSlot), new());

        bool contains = ChroniaPointsGameModule.summaries[save.FileSlot].CassetteList.Contains(area.ID.ToString());

        if (!contains)
        {
            ChroniaPointsGameModule.GlobalData.MaxCassettes++;
            ChroniaPointsGameModule.summaries[save.FileSlot].CassetteList.Enter(area.ID.ToString());
        }
        else
        {
            if (collectArg)
            {
                ChroniaPointsGameModule.GlobalData.TruePoints.AddPoints(CPGame.AnyCassetteBonus);
                CPGame.LastCassetteUpdated++;
            }
        }
    }
    
    public static bool DeleteFile(On.Celeste.SaveData.orig_TryDelete orig, int fileSlot)
    {
        bool _orig = orig(fileSlot);

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

        ChroniaPointsGameModule.summaries = SaveDataReader.AnalyzeAllLocalSaves();
        ChroniaPointsGameModule.summaries.EachDo((item) =>
        {
            berries += item.TotalStrawberries;
            goldens += item.GoldenStrawberries;
            cassettes += item.Cassettes;
            gems += item.CrystalHearts;
            get9a += item.HasFarewellMoonBerry ? 1 : 0;
            get1a += item.HasPrologueNoDashGolden ? 1 : 0;
        });

        ChroniaPointsGameModule.GlobalData.MaxStrawberries = berries;
        ChroniaPointsGameModule.GlobalData.MaxGoldenStrawberries = goldens;
        ChroniaPointsGameModule.GlobalData.MaxCassettes = cassettes;
        ChroniaPointsGameModule.GlobalData.MaxHeartGems = gems;
        ChroniaPointsGameModule.GlobalData.MoonberryGet = get9a;
        ChroniaPointsGameModule.GlobalData.DashlessBerryGet = get1a;

        return _orig;
    }

    public static void LevelLoad(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes intro, bool loader)
    {
        orig(self, intro, loader);

        ChroniaPointsGameModule.Session.DiscoveredRooms.Add(self.Session.LevelData.Name);
    }
    
    public static void CollectingHeartGem(On.Celeste.SaveData.orig_RegisterHeartGem orig, SaveData save, AreaKey area)
    {
        orig(save, area);

        if (save.FileSlot == -1) { return; }

        bool upgrade = CPGame.AnyCollectableBonus;
        bool registered = save.Areas[area.ID].Modes[(int)area.Mode].HeartGem;
        bool freq = CPGame.LastHeartUpdated < 10;

        bool collectArg = upgrade && !registered && freq;
        
        if (area.ID > 10)
        {
            if (collectArg)
            {
                ChroniaPointsGameModule.GlobalData.TruePoints.AddPoints(CPGame.AnyHeartBonus);
                CPGame.LastHeartUpdated++;
            }
            
            return;
        }

        // Old - Analyze whole game data

        //bool contains = SaveDataReader.maxHearts.Contains($"{area.ID}_{(int)area.Mode}");

        //if (!contains)
        //{
        //    Md.GlobalData.MaxHeartGems++;

        //    SaveDataReader.maxHearts.Enter($"{area.ID}_{(int)area.Mode}");
        //}

        ChroniaPointsGameModule.summaries.Create(save.FileSlot, new(slot: save.FileSlot), new());
        
        bool contains = ChroniaPointsGameModule.summaries[save.FileSlot].HeartList.Contains($"{area.ID}_{(int)area.Mode}");

        if (!contains)
        {
            ChroniaPointsGameModule.GlobalData.MaxHeartGems++;
            ChroniaPointsGameModule.summaries[save.FileSlot].HeartList.Enter($"{area.ID}_{(int)area.Mode}");
        }
        else
        {
            if (collectArg)
            {
                ChroniaPointsGameModule.GlobalData.TruePoints.AddPoints(CPGame.AnyHeartBonus);
                CPGame.LastHeartUpdated++;
            }
        }
    }
    
    public static void BerryAdding(On.Celeste.SaveData.orig_AddStrawberry_AreaKey_EntityID_bool orig, SaveData save, AreaKey area, EntityID berry, bool golden)
    {
        orig(save, area, berry, golden);

        if (save.FileSlot == -1) { return; }

        bool upgrade = CPGame.AnyCollectableBonus;
        bool freq1 = CPGame.LastStrawberryUpdated < 100;
        bool freq2 = CPGame.LastGoldenUpdated < 100;

        bool collectArg = upgrade && ((!golden && freq1) || (golden && freq2));
        
        if (area.ID > 10)
        {
            if (collectArg)
            {
                ChroniaPointsGameModule.GlobalData.TruePoints.AddPoints(golden ? CPGame.AnyGoldenBerryBonus : CPGame.AnyStrawberryBonus);
                if (golden)
                {
                    CPGame.LastGoldenUpdated++;
                }
                else
                {
                    CPGame.LastStrawberryUpdated++;
                }
            }
            
            return;
        }

        // Old - Analyze whole game data

        //bool contains = SaveDataReader.maxStrawberries.Contains($"{area.ID}_{berry.Key}");
        //bool newGold = !SaveDataReader.maxGoldens.Contains($"{area.ID}_{(int)area.Mode}") && golden;

        //if (!contains && newGold)
        //{
        //    Md.GlobalData.MaxGoldenStrawberries++;

        //    SaveDataReader.maxGoldens.Enter($"{area.ID}_{(int)area.Mode}");
        //}
        //else if (!contains)
        //{
        //    if (area.ID == 1 && berry.Key == "end:4") { Md.GlobalData.DashlessBerryGet = 1; }
        //    else if (area.ID == 10 && berry.Key == "j-19:9") { Md.GlobalData.MoonberryGet = 1; }
        //    else
        //    {
        //        Md.GlobalData.MaxStrawberries++;
        //        SaveDataReader.maxStrawberries.Enter($"{area.ID}_{berry.Key}");
        //    }
        //}

        ChroniaPointsGameModule.summaries.Create(save.FileSlot, new(slot: save.FileSlot), new());
        bool contains = ChroniaPointsGameModule.summaries[save.FileSlot].StrawberryList.Contains($"{area.ID}_{berry.Key}");
        bool newGold = !ChroniaPointsGameModule.summaries[save.FileSlot].GoldenList.Contains($"{area.ID}_{(int)area.Mode}") && golden;
        
        if(!contains && newGold)
        {
            ChroniaPointsGameModule.GlobalData.MaxGoldenStrawberries++;
            ChroniaPointsGameModule.summaries[save.FileSlot].GoldenList.Enter($"{area.ID}_{(int)area.Mode}");
        }
        else if (!contains)
        {
            if (area.ID == 1 && berry.Key == "end:4") { ChroniaPointsGameModule.GlobalData.DashlessBerryGet = 1; return; }
            if (area.ID == 10 && berry.Key == "j-19:9") { ChroniaPointsGameModule.GlobalData.MoonberryGet = 1; return; }
            
            ChroniaPointsGameModule.GlobalData.MaxStrawberries++;
            ChroniaPointsGameModule.summaries[save.FileSlot].StrawberryList.Enter($"{area.ID}_{berry.Key}");
        }
        else
        {
            if (collectArg)
            {
                ChroniaPointsGameModule.GlobalData.TruePoints.AddPoints(golden ? CPGame.AnyGoldenBerryBonus : CPGame.AnyStrawberryBonus);
                if (golden)
                {
                    CPGame.LastGoldenUpdated++;
                }
                else
                {
                    CPGame.LastStrawberryUpdated++;
                }
            }
        }
    }

    // ===== 通关奖励 =====
    public static void OnLevelComplete(On.Celeste.SaveData.orig_RegisterCompletion orig, SaveData save, Session session)
    {
        orig(save, session);

        if (save.FileSlot == -1) { return; }

        int rooms = ChroniaPointsGameModule.Session.DiscoveredRooms.Count;
        
        if (session.GrabbedGolden)
        {
            ChroniaPointsGameModule.GlobalData.TruePoints = ChroniaPointsGameModule.GlobalData.TruePoints.AddPoints(rooms * 0.4 * CPGame.OverallMultiplier);
            ChroniaPointsGameModule.GlobalData.LastUpdateTimeUtc = DateTime.UtcNow;
            ChroniaPointsGameModule.GlobalData.SaveAll();

            Logger.Log("ChroniaPointsGame", $"Golden Get! Points added {rooms * 0.4} before running multiplier of {CPGame.OverallMultiplier} → {ChroniaPointsGameModule.GlobalData.TruePoints}");
        }
        else if (session.StartedFromBeginning)
        {
            ChroniaPointsGameModule.GlobalData.TruePoints = ChroniaPointsGameModule.GlobalData.TruePoints.AddPoints(rooms * 0.2 * CPGame.OverallMultiplier);
            ChroniaPointsGameModule.GlobalData.LastUpdateTimeUtc = DateTime.UtcNow;
            ChroniaPointsGameModule.GlobalData.SaveAll();

            Logger.Log("ChroniaPointsGame", $"Level full run! Points added {rooms * 0.2} before running multiplier of {CPGame.OverallMultiplier} → {ChroniaPointsGameModule.GlobalData.TruePoints}");
        }
        else
        {
            ChroniaPointsGameModule.GlobalData.TruePoints = ChroniaPointsGameModule.GlobalData.TruePoints.AddPoints(rooms * 0.1 * CPGame.OverallMultiplier);
            ChroniaPointsGameModule.GlobalData.LastUpdateTimeUtc = DateTime.UtcNow;
            ChroniaPointsGameModule.GlobalData.SaveAll();

            Logger.Log("ChroniaPointsGame", $"Level complete! Points added {rooms * 0.1} before running multiplier of {CPGame.OverallMultiplier} → {ChroniaPointsGameModule.GlobalData.TruePoints}");
        }
    }

    // ===== 通用刷新 =====
    public static void SceneUpdate(On.Monocle.Scene.orig_Update orig, Scene scene)
    {
        orig(scene);

        if (scene is not Overworld && scene is not Level) { return; }

        var now = DateTime.UtcNow;

        if ((now - CPGame.LastCollectableUpdate).TotalSeconds > 10)
        {
            CPGame.LastCollectableUpdate = now;
            CPGame.LastStrawberryUpdated = 0;
            CPGame.LastGoldenUpdated = 0;
            CPGame.LastCassetteUpdated = 0;
            CPGame.LastHeartUpdated = 0;
        }

        var delta = (now - ChroniaPointsGameModule.GlobalData.LastUpdateTimeUtc).TotalSeconds;

        if (delta <= 0 || delta > 10)
        {
            ChroniaPointsGameModule.GlobalData.LastUpdateTimeUtc = now;
            return;
        }

        CPGame.LevelMultiplier = scene is Level ? 1.2 : 1.0;
        double rawGain = CPGame.OverallMultiplier * delta;

        if (rawGain > 0)
        {
            ChroniaPointsGameModule.GlobalData.TruePoints = ChroniaPointsGameModule.GlobalData.TruePoints.AddPoints(rawGain);

            ChroniaPointsGameModule.GlobalData.LastUpdateTimeUtc = now;

            // 自动保存（每30秒）
            if ((now - ChroniaPointsGameModule.lastAutoSaveTime).TotalSeconds >= 30)
            {
                ChroniaPointsGameModule.GlobalData.SaveAll();
                ChroniaPointsGameModule.lastAutoSaveTime = now;
            }
        }
    }
}
