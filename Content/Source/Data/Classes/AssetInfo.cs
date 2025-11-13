using Godot;

public partial class AssetInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public double DeploymentSpeed { get; set; } // in sec
    public double HarvestAmount { get; set; }
    public string[] Prerequisites { get; set; }
}
