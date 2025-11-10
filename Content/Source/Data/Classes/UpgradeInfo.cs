using Godot;
using Godot.Collections;

public partial class UpgradeInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public Dictionary<string, int> ResourceCost { get; set; }
}
