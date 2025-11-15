using Godot;
using System.Collections.Generic;
using System.Text.Json;
using FileAccess = Godot.FileAccess;

public partial class DataUtil : Node
{
    const string ASSET_DATA_FILE = "res://Content/Source/Data/JSON/Assets.json";
    const string DEFAULTS_DATA_FILE = "res://Content/Source/Data/JSON/Defaults.json";
    const string RESEARCH_DATA_FILE = "res://Content/Source/Data/JSON/Research.json";
    const string RESOURCE_DATA_FILE = "res://Content/Source/Data/JSON/Resources.json";
    const string UPGRADE_DATA_FILE = "res://Content/Source/Data/JSON/Upgrades.json";

    public static DataUtil Instance { get; private set; }

    public GameData gameData;
    private string savePath;

    public override void _Ready()
    {
        Instance = this;
        savePath = ProjectSettings.GlobalizePath("user://savegame.json");
    }

    public void SaveGame(GameData data)
    {
        string json = JsonSerializer.Serialize(data);
        FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);
        file.StoreString(json);
        file.Close();
    }

    public GameData LoadGame()
    {
        if (!FileAccess.FileExists(savePath))
            return CreateDefaultGameData(); // return default data

        FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();

        return JsonSerializer.Deserialize<GameData>(json);
    }

    private GameData CreateDefaultGameData()
    {
        GameData defaultData = new GameData
        {
            SaveName = "New Save",
            SaveDate = System.DateTime.Now,
            PlayTime = 0f,
            PlaySpeed = 1f,
            AsteroidPoints = [],
            assets = new Dictionary<string, Dictionary<string, double>>(),
            researches = [],
            resources = new Dictionary<string, Dictionary<string, double>> 
            {
                { "Credits", new Dictionary<string, double>
                    {
                        { "Current", 100000000000000d }
                    }
                }
            },
            upgrades = []
        }; 
        return defaultData;
    }

    public Dictionary<string, AssetInfo> GetDefaultAssets()
    {
        if (!FileAccess.FileExists(ASSET_DATA_FILE))
            return null;
        FileAccess file = FileAccess.Open(ASSET_DATA_FILE, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();
        return JsonSerializer.Deserialize<Dictionary<string, AssetInfo>>(json);
    }
    public Dictionary<string, int> GetGameDefaults()
    {
        if (!FileAccess.FileExists(DEFAULTS_DATA_FILE))
            return null;
        FileAccess file = FileAccess.Open(DEFAULTS_DATA_FILE, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();
        return JsonSerializer.Deserialize<Dictionary<string, int>>(json);
    }
    public Dictionary<string, ResourceInfo> GetDefaultResources()
    {
        if (!FileAccess.FileExists(RESOURCE_DATA_FILE))
            return null;

        FileAccess file = FileAccess.Open(RESOURCE_DATA_FILE, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();

        return JsonSerializer.Deserialize<Dictionary<string, ResourceInfo>>(json);
    }
    public Dictionary<string, ResearchInfo> GetDefaultResearch()
    {
        if (!FileAccess.FileExists(RESEARCH_DATA_FILE))
            return null;
        FileAccess file = FileAccess.Open(RESEARCH_DATA_FILE, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();
        return JsonSerializer.Deserialize<Dictionary<string, ResearchInfo>>(json);
    }
    public Dictionary<string, UpgradeInfo> GetDefaultUpgrades()
    {
        if (!FileAccess.FileExists(UPGRADE_DATA_FILE))
            return null;
        FileAccess file = FileAccess.Open(UPGRADE_DATA_FILE, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();
        return JsonSerializer.Deserialize<Dictionary<string, UpgradeInfo>>(json);
    }
}