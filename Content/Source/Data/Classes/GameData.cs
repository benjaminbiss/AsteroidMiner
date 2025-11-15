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
    public Dictionary<string, Dictionary<string, double>> assets { get; set; }
    public Dictionary<string, Dictionary<string, Dictionary<bool, double>>> assetModifiers { get; set; }
    public string[] researches { get; set; }
    public Dictionary<string, Dictionary<string, double>> resources { get; set; }
    public Dictionary<string, Dictionary<string, Dictionary<bool, double>>> resourceModifiers { get; set; }
    public string[] upgrades { get; set; }
}
