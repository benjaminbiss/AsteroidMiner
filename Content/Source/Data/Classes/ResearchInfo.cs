using Godot;
using Godot.Collections;

public partial class ResearchInfo : GodotObject
{
    public string Name;
    public string Description;
    public string IconPath;

    // keys to upgrades and researches
    public Array<string> Prerequisites;

    // name : amount
    public Dictionary<string, int> ResourceCost;

    // name : paramater : isMultiplicative : effectValue
    public Dictionary<string, Dictionary<string, Dictionary<bool, float>>> UpgradeEffects;
}
