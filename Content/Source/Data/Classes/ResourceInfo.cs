using Godot;
using Godot.Collections;

public partial class ResourceInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public double Current { get; set; }
    // keys to upgrades and researches
    public Array<string> Prerequisites { get; set; }
}
