using Godot;
using Godot.Collections;

public partial class UpgradeInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }

    // keys to upgrades and researches
    public Array<string> Prerequisites { get; set; }

    // name : amount
    public Dictionary<string, int> ResourceCost { get; set; }

    // name : paramater : isMultiplicative : effectValue
    public Dictionary<string, Dictionary<string, Dictionary<bool, float>>> UpgradeEffects { get; set; }
}
