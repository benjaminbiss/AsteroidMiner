using Godot;
using Godot.Collections;

public partial class InfiniteUpgradeInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }

    // keys to upgrades and researches
    public Array<string> Prerequisites { get; set; }

    // name : amount
    public Dictionary<string, double> BaseResourceCost { get; set; }

    // cost multiplier per level : ^n
    public double CostMultiplier { get; set; }

    // current level of the upgrade
    public int CurrentLevel { get; set; }

    // name : parameter : isMultiplicative : effectValue
    public Dictionary<string, Dictionary<string, Dictionary<bool, double>>> AssetModifiers { get; set; }
}
