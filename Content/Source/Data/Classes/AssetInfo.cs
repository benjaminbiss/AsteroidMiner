using Godot;
using Godot.Collections;

public partial class AssetInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public double DeploymentSpeed { get; set; } // in sec
    public double HarvestAmount { get; set; }
    public Array<string> Prerequisites { get; set; }
    // name : amount
    public Dictionary<string, double> ResourceCost { get; set; }
    // paramater : isAdditive : effectValue
    public Dictionary<string, Dictionary<bool, double>> Modifiers { get; set; }

}
