using Godot;

[GlobalClass]
public partial class ResourceInfo : Resource
{
    [Export]
    public string Name { get; set; }
    [Export]
    public Texture2D Icon { get; set; }
    [Export]
    public int CurrentAmount { get; set; }
    [Export]
    public int MaxAmount { get; set; }
}
