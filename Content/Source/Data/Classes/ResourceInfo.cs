using Godot;

public partial class ResourceInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public int CurrentAmount { get; set; }
    public int MaxAmount { get; set; }
}
