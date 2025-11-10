using Godot;
using System.Text.Json;
using FileAccess = Godot.FileAccess;

public partial class DataUtil : Node
{
    public static DataUtil Instance { get; private set; }

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
            return new GameData(); // return default data

        FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
        string json = file.GetAsText();
        file.Close();

        return JsonSerializer.Deserialize<GameData>(json);
    }
}