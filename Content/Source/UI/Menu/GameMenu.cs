using Godot;
using Godot.Collections;

public partial class GameMenu : Control
{
    [Export]
    private PackedScene resourceTabScene;
    [Export]
    private NodePath resourceBarPath;
    private VBoxContainer resourceBar;

    [Export]
    private PackedScene assetTabScene;
    [Export]
    private NodePath assetPath;
    private VBoxContainer asseteBar;

    private System.Collections.Generic.Dictionary<string, ResourceInfo> resources;
    public Array<ResourceTab> resourceTabs { get; private set; }

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("GameMenu | Initialization failed.");
            return;
        }
    }
    private bool Initialize()
    {
        if (resourceTabScene == null)
            return false;
        resourceBar = GetNodeOrNull<VBoxContainer>(resourceBarPath);
        if (resourceBar == null)
            return false;
        if (assetTabScene == null)
            return false;
        asseteBar = GetNodeOrNull<VBoxContainer>(assetPath);
        if (asseteBar == null)
            return false;

        return true;
    }

    public void SetResources(System.Collections.Generic.Dictionary<string, ResourceInfo> resources)
    {
        this.resources = resources;
        resourceTabs = new Array<ResourceTab>();
        PopulateResourceBar();
    }
    public void SetAssets(System.Collections.Generic.Dictionary<string, AssetInfo> assets)
    {
        // Implementation for populating asset bar can be added here
    }
    public void SetResearches(System.Collections.Generic.Dictionary<string, ResearchInfo> researches)
    {
        // Implementation for populating research bar can be added here
    }
    public void SetUpgrades(System.Collections.Generic.Dictionary<string, UpgradeInfo> upgrades)
    {
        // Implementation for populating upgrade bar can be added here
    }

    private void PopulateResourceBar()
    {
        foreach (string key in resources.Keys)
        {
            ResourceTab resourceTab = resourceTabScene.Instantiate<ResourceTab>();
            resourceBar.AddChild(resourceTab);

            resourceTabs.Add(resourceTab);
            resourceTab.SetResourceInfo(resources[key]);

            if (resources[key].BaseMaxAmount == 0)
            {
                resourceTab.Hide();
            }
        }
    }
}
