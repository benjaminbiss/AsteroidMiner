using Godot;

public partial class ResourceInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public int BaseMaxAmount { get; set; }
    public float BaseGenerationRate { get; set; }
}
