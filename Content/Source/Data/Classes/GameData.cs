using Godot;
using Godot.Collections;
using System;

public partial class GameData : Node
{
    // General Save Info
    public string SaveName { get; set; }
    public DateTime SaveDate { get; set; }
    public float PlayTime { get; set; }
    // Testing info
    public float PlaySpeed { get; set; }
    // Save Values
    public Dictionary<string, AssetInfo> Assets { get; set; }
    public Dictionary<string, ResearchInfo> Research { get; set; }
    public Dictionary<string, ResourceInfo> Resources { get; set; }
    public Dictionary<string, UpgradeInfo> Upgrades { get; set; }
}
