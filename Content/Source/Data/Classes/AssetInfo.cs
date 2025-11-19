using Godot;
using Godot.Collections;

public partial class AssetInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public int Level { get; set; }
    public double DeploymentSpeed { get; set; }
    public string HarvestedResource { get; set; }
    public double HarvestAmount { get; set; }
    public Array<string> Prerequisites { get; set; }
    // name : amount
    public Dictionary<string, double> ResourceCost { get; set; }
    // parameter : isAdditive : effectValue
    public Dictionary<string, Dictionary<bool, double>> Modifiers { get; set; }

}
