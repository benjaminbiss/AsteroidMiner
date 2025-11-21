using Godot;
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
            Assets = GetAssetsJSON(),
            Researches = GetResearchJSON(),
            Resources = GetResourcesJSON(),
            Upgrades = GetUpgradesJSON(),
            Defaults = GetGameDefaultsJSON(),
            Prerequisites = []
        };
        return defaultData;
    }
   
    public Godot.Collections.Dictionary<string, AssetInfo> GetAssetsJSON()
    {
        if (!FileAccess.FileExists(ASSET_DATA_FILE))
            return null;
        FileAccess file = FileAccess.Open(ASSET_DATA_FILE, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();
        System.Collections.Generic.Dictionary<string, AssetInfo> data = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, AssetInfo>>(json);
        Godot.Collections.Dictionary<string, AssetInfo> returnData = new Godot.Collections.Dictionary<string, AssetInfo>();
        foreach (var pair in data)
        {
            returnData[pair.Key] = pair.Value;
        }
        return returnData;
    }
    private Godot.Collections.Dictionary<string, int> GetGameDefaultsJSON()
    {
        if (!FileAccess.FileExists(DEFAULTS_DATA_FILE))
            return null;
        FileAccess file = FileAccess.Open(DEFAULTS_DATA_FILE, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();
        System.Collections.Generic.Dictionary<string, int> data = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, int>>(json);
        Godot.Collections.Dictionary<string, int> returnData = new Godot.Collections.Dictionary<string, int>();
        foreach (var pair in data)
        {
            returnData[pair.Key] = pair.Value;
        }
        return returnData;
    }
    private Godot.Collections.Dictionary<string, ResourceInfo> GetResourcesJSON()
    {
        if (!FileAccess.FileExists(RESOURCE_DATA_FILE))
            return null;

        FileAccess file = FileAccess.Open(RESOURCE_DATA_FILE, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();
        System.Collections.Generic.Dictionary<string, ResourceInfo> data = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, ResourceInfo>>(json);
        Godot.Collections.Dictionary<string, ResourceInfo> returnData = new Godot.Collections.Dictionary<string, ResourceInfo>();
        foreach (var pair in data)
        {
            returnData[pair.Key] = pair.Value;
        }
        return returnData;
    }
    private Godot.Collections.Dictionary<string, ResearchInfo> GetResearchJSON()
    {
        if (!FileAccess.FileExists(RESEARCH_DATA_FILE))
            return null;
        FileAccess file = FileAccess.Open(RESEARCH_DATA_FILE, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();
        System.Collections.Generic.Dictionary<string, ResearchInfo> data = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, ResearchInfo>>(json);
        Godot.Collections.Dictionary<string, ResearchInfo> returnData = new Godot.Collections.Dictionary<string, ResearchInfo>();
        foreach (var pair in data)
        {
            returnData[pair.Key] = pair.Value;
        }
        return returnData;
    }
    private Godot.Collections.Dictionary<string, UpgradeInfo> GetUpgradesJSON()
    {
        if (!FileAccess.FileExists(UPGRADE_DATA_FILE))
            return null;
        FileAccess file = FileAccess.Open(UPGRADE_DATA_FILE, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();
        System.Collections.Generic.Dictionary<string, UpgradeInfo> data = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, UpgradeInfo>>(json);
        Godot.Collections.Dictionary<string, UpgradeInfo> returnData = new Godot.Collections.Dictionary<string, UpgradeInfo>();
        foreach (var pair in data)
        {
            returnData[pair.Key] = pair.Value;
        }
        return returnData;
    }
}