using Godot;

public partial class ResourceInfo : GodotObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public double BaseMaxAmount { get; set; }
    public double GenerationAmount { get; set; } // in sec
}
