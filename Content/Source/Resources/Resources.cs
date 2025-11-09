using Godot;
using Godot.Collections;
using System;

public partial class Resources : Node
{
    public static Resources Instance { get; private set; }

    public ResourceInfo CreditsTabInfo { get; private set; }
    public ResourceInfo PowerTabInfo { get; private set; }

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

    public Array<ResourceInfo> GetAllResourceInfos()
    {
        Array<ResourceInfo> resourceInfos = new Array<ResourceInfo>
        {
            CreditsTabInfo,
            PowerTabInfo
        };
        return resourceInfos;
    }

    private void LoadResourceInfos()
    {
        CreditsTabInfo = new ResourceInfo
        {
            Name = "Credits",
            Icon = GD.Load<Texture2D>("res://icon.svg"),
            CurrentAmount = 0,
            MaxAmount = 10
        };
        PowerTabInfo = new ResourceInfo
        {
            Name = "Power",
            Icon = GD.Load<Texture2D>("res://icon.svg"),
            CurrentAmount = 0,
            MaxAmount = 3
        };
    }
}
