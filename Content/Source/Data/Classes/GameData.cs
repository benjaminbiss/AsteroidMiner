using System;
using System.Collections.Generic;

public class GameData
{
    // General Save Info
    public string SaveName { get; set; }
    public DateTime SaveDate { get; set; }
    public float PlayTime { get; set; }
    // Testing info
    public float PlaySpeed { get; set; }
    // Save Values
    public int[] AsteroidPoints { get; set; }
    public Dictionary<string, Dictionary<string, float>> OwnedAssets { get; set; }
    public string[] OwnedResearch { get; set; }
    public Dictionary<string, int> OwnedResources { get; set; }
    public string[] OwnedUpgrades { get; set; }
}
