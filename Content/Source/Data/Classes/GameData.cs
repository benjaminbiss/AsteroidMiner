using System;
using Godot.Collections;

public class GameData
{
    // General Save Info
    public string SaveName { get; set; }
    public DateTime SaveDate { get; set; }
    public float PlayTime { get; set; }
    // Testing info
    public float PlaySpeed { get; set; }
    // Save Values
    public Array<int> AsteroidPoints { get; set; }
    public Dictionary<string, AssetInfo> Assets { get; set; }
    public Dictionary<string, ResearchInfo>  Researches { get; set; }
    public Dictionary<string, ResourceInfo> Resources { get; set; }
    public Dictionary<string, UpgradeInfo> Upgrades { get; set; }
    public Dictionary<string, int> Defaults { get; set; }
    public Array<string> Preresquisites { get; set; }
}
