using Godot;
using Godot.Collections;
using System;

public partial class ResourceManager : Node
{
    private GameCore gameCore;
    private GameMenu gameMenu;

    [Export]
    private PackedScene resourceTabScene;
    [Export]
    private NodePath resourceBarPath;
    private VBoxContainer resourceBar;
    
    private Dictionary<string, ResourceTab> resourceTabs = new Dictionary<string, ResourceTab>();

    // Initialization
    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("ResourceManager | Initialization failed.");
            return;
        }
    }
    private bool Initialize()
    {
        gameCore = GameCore.Instance;
        if (gameCore == null)
            return false;
        if (resourceTabScene == null)
            return false;
        resourceBar = GetNodeOrNull<VBoxContainer>(resourceBarPath);
        if (resourceBar == null)
            return false;

        return true;
    }
    public void Setup(GameMenu menu)
    {
        gameMenu = menu;
        PopulateResourceBar();
    }

    // Runtime
    private void PopulateResourceBar()
    {
        if (gameCore == null || gameCore.gameData == null)
            return;

        foreach (var resource in gameCore.gameData.Resources)
        {
            ResourceTab resourceTab = resourceTabScene.Instantiate<ResourceTab>();
            resourceBar.AddChild(resourceTab);
            resourceTabs.Add(resource.Key, resourceTab);
            resourceTab.SetResourceInfo(resource.Value);

            if (!gameMenu.HasAllPrerequisites(resource.Value.Prerequisites))
            {
                resourceTab.Hide();
            }
        }

        //foreach (var resource in gameCore.resourceInfos)
        //{
        //    ResourceTab resourceTab = resourceTabScene.Instantiate<ResourceTab>();
        //    resourceBar.AddChild(resourceTab);
        //    resourceTabs.Add(resourceTab);
        //    resourceTab.SetResourceInfo(gameCore.resourceInfos[resource.Key]);

        //    if (!gameCore.gameData.resources.ContainsKey(resource.Key))
        //    {
        //        resourceTab.Hide();
        //    }
        //}
    }
    private void UpdateResourceTab(string resource)
    {
    }
}
