using Godot;

public partial class AssetInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }

    // spawn p/s
    public float DeploymentRate { get; set; }
    // total number active
    public int CurrentlyDeployed { get; set; }
}
