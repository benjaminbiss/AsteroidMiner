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

        GameCore.Instance.ResourcesUpdated += UpdateResourceAmount;
        GameCore.Instance.PrerequisitesUpdated += CheckPrerequisites;
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
            resourceTab.SetupUI(resource.Value);

            if (!gameMenu.HasAllPrerequisites(resource.Value.Prerequisites))
            {
                resourceTab.Hide();
            }
        }
    }
    private void UpdateResourceAmount(string resource)
    {
        foreach (var tab in resourceTabs)
        {
            if (tab.Key == resource)
            {                
                ResourceTab resTab = tab.Value;
                resTab.UpdateResourceAmount(gameCore.gameData.Resources[resource].Current);
                break;
            }
        }
    }    
    private void CheckPrerequisites()
    {
        foreach (var tab in resourceTabs)
        { 
            ResourceTab resTab = tab.Value;
            if (resTab.Visible == false)
            {
                if (gameMenu.HasAllPrerequisites(gameCore.gameData.Resources[tab.Key].Prerequisites))                    
                    resTab.Show();
            }
        }
    }
}
