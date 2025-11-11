using Godot;

public partial class AssetInfo : GodotObject
{
    public string Name;
    public string Description;
    public string IconPath;
    public float BaseDeploymentRate;
    public float BaseHarvestAmount;
    public string[] Prerequisites;
}
