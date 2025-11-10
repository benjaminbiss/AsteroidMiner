using Godot;

public partial class AssetInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public float DeploymentRate { get; set; }
    public int CurrentlyDeployed { get; set; }
}
