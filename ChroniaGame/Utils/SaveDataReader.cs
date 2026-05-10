using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ChroniaPointsGame.Utils;

public class SaveSummary
{
    public SaveSummary(int slot = -1, string name = "", int berries = 0, int goldens = 0,
        int cassettes = 0, int hearts = 0, bool moon = false, bool dashless = false)
    {
        Slot = slot;
        Name = name;
        TotalStrawberries = berries;
        GoldenStrawberries = goldens;
        Cassettes = cassettes;
        CrystalHearts = hearts;
        HasFarewellMoonBerry = moon;
        HasPrologueNoDashGolden = dashless;

        StrawberryList = new();
        GoldenList = new();
        CassetteList = new();
        HeartList = new();
    }
    public int Slot { get; set; }
    public string Name { get; set; }
    public int TotalStrawberries { get; set; } 
    public int GoldenStrawberries { get; set; }
    public int Cassettes { get; set; }
    public int CrystalHearts { get; set; }
    public bool HasFarewellMoonBerry { get; set; }
    public bool HasPrologueNoDashGolden { get; set; } 

    public List<string> StrawberryList { get; set; }
    public List<string> GoldenList { get; set; }
    public List<string> CassetteList { get; set; }
    public List<string> HeartList { get; set; }
}

public static class SaveDataReader
{
    public static List<string> maxStrawberries = new();
    public static List<string> maxGoldens = new();
    public static List<string> maxCassettes = new();
    public static List<string> maxHearts = new();
    public static List<SaveSummary> AnalyzeAllLocalSaves()
    {
        var summaries = new List<SaveSummary>();
        string userPath = Everest.PathGame; 

        if (!Directory.Exists(userPath))
        {
            Logger.Log("MyMod", "User save directory does not exist.");
            return summaries;
        }

        string[] celesteFiles = Directory.GetFiles(Path.Combine(userPath, "Saves"), "*.celeste");

        foreach (string filePath in celesteFiles)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (!int.TryParse(fileName, out int slot))
                    continue;

                XDocument doc = XDocument.Load(filePath);
                XElement root = doc.Root;

                if (root?.Name != "SaveData")
                    continue; // 不是有效的存档文件

                var summary = new SaveSummary
                {
                    Slot = slot,
                    Name = root.Element("Name")?.Value ?? "Unknown"
                };

                // --- 提取核心数据 ---
                var areasElement = root.Element("Areas");
                if (areasElement != null)
                {
                    var areaElements = areasElement.Elements("AreaStats");

                    // 只处理 SID 以 "Celeste/" 开头的原版章节
                    var celesteAreas = areaElements.Where(a => a.Attribute("SID")?.Value.StartsWith("Celeste/") == true).ToList();

                    int totalStrawberryCount = 0;
                    summary.GoldenStrawberries = 0;
                    summary.Cassettes = 0;
                    summary.CrystalHearts = 0;
                    summary.HasFarewellMoonBerry = false;
                    summary.HasPrologueNoDashGolden = false;
                    
                    for(int i = 0; i < celesteAreas.Count(); i++)
                    {
                        var areaElement = celesteAreas.ElementAt(i);
                        int areaId = areaElement.Attribute("ID").Value.ParseInt(-1);

                        // 计算金草莓：每个 SingleRunCompleted=true 的 AreaModeStats 算一个金草莓
                        var modesElement = areaElement.Element("Modes");
                        if (modesElement != null)
                        {
                            
                            for(int j = 0; j < modesElement.Elements("AreaModeStats").Count(); j++)
                            {
                                var modeElement = modesElement.Elements("AreaModeStats").ElementAt(j);
                                
                                if ((bool?)modeElement.Attribute("SingleRunCompleted") == true)
                                {
                                    summary.GoldenStrawberries++;
                                    maxGoldens.Enter($"{areaId}_{j}");
                                    summary.GoldenList.Enter($"{areaId}_{j}");
                                }

                                // 计算水晶之心
                                if ((bool?)modeElement.Attribute("HeartGem") == true)
                                { 
                                    summary.CrystalHearts++;
                                    maxHearts.Enter($"{areaId}_{j}");
                                    summary.HeartList.Enter($"{areaId}_{j}");
                                }

                                // 统计普通草莓（EntityID）
                                var strawberriesElement = modeElement.Element("Strawberries");
                                if (strawberriesElement != null)
                                {
                                    totalStrawberryCount += strawberriesElement.Elements("EntityID").Count();

                                    foreach (var strawberryElement in strawberriesElement.Elements("EntityID"))
                                    {
                                        if (areaId == 1 && strawberryElement.Attribute("Key").Value == "end:4")
                                        {
                                            summary.HasPrologueNoDashGolden = true;
                                        }
                                        if (areaId == 10 && strawberryElement.Attribute("Key").Value == "j-19:9")
                                        {
                                            summary.HasFarewellMoonBerry = true;
                                        }

                                        maxStrawberries.Enter($"{areaId}_{strawberryElement.Attribute("Key").Value}");
                                        summary.StrawberryList.Enter($"{areaId}_{strawberryElement.Attribute("Key").Value}");
                                    }
                                }
                            }
                        }

                        // 计算磁带
                        if ((bool?)areaElement.Attribute("Cassette") == true)
                        {
                            summary.Cassettes++;
                            maxCassettes.Enter(areaId.ToString());
                            summary.CassetteList.Enter(areaId.ToString());
                        }
                    }

                    // --- 计算 TotalStrawberries ---
                    // 总草莓数 = 所有草莓条目 - 金草莓 - 月梅（如果有） - 无冲刺金草莓（如果有）
                    summary.TotalStrawberries = totalStrawberryCount;
                    if (summary.HasPrologueNoDashGolden) summary.TotalStrawberries--;
                    if (summary.HasFarewellMoonBerry) summary.TotalStrawberries--;
                    summary.TotalStrawberries -= summary.GoldenStrawberries; // 每个金草莓也算一个 EntityID

                    // 确保不为负
                    summary.TotalStrawberries = Math.Max(0, summary.TotalStrawberries);
                }

                summaries.Add(summary);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "MyMod", $"Failed to analyze save file '{filePath}': {ex.Message}");
            }
        }

        return summaries;
    }
}