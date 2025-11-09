using Godot;

public partial class Resources : Node
{
    public static Resources Instance { get; private set; }

    public static ResourceInfo CreditsTabInfo { get; private set; }
    public static ResourceInfo PowerTabInfo { get; private set; }

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.PrintErr("Resources | Multiple instances detected. There should only be one instance of Resources in the scene tree.");
            QueueFree();
            return;
        }
        Instance = this;
        LoadResourceInfos();
    }

    private void LoadResourceInfos()
    {
        CreditsTabInfo = new ResourceInfo
        {
            Name = "Credits",
            Icon = GD.Load<Texture2D>("res://icon.svg"),
            CurrentAmount = 0,
            MaxAmount = 1000
        };
        PowerTabInfo = new ResourceInfo
        {
            ResourceName = "Power",
            Icon = GD.Load<Texture2D>("res://icon.svg"),
            CurrentAmount = 0,
            MaxAmount = 500
        };
    }
}
