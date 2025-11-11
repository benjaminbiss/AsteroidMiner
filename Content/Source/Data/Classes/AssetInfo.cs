using Godot;

public partial class AssetInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public float BaseDeploymentRate { get; set; }
    public float BaseHarvestAmount { get; set; }
    public string[] Prerequisites { get; set; }
}
