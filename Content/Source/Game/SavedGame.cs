using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SavedGame : Resource
{
    [Export]
    public string SaveName { get; set; }
    [Export]
    public string SaveDate { get; set; }

    [Export]
    public Dictionary<string, Variant> SaveData { get; set; }
}
