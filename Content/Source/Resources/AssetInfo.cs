using Godot;

[GlobalClass]
public partial class AssetInfo : Resource
{
    [Export]
    public string Name { get; set; }
    [Export]
    public Texture2D Icon { get; set; }
    [Export]
    public float DeploymentRate { get; set; }
}
