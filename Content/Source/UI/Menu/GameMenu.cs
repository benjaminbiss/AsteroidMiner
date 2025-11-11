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
    private Array<ResourceTab> resourceTabs;

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

    private void PopulateResourceBar()
    {
        foreach (string key in resources.Keys)
        {
            ResourceTab resourceTab = resourceTabScene.Instantiate<ResourceTab>();
            resourceBar.AddChild(resourceTab);

            resourceTabs.Add(resourceTab);
            resourceTab.SetResourceInfo(resources[key]);
        }
    }
}
