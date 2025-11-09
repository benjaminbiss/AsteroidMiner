using Godot;
using Godot.Collections;

public partial class GameMenu : Control
{
    [Export]
    private PackedScene resourceTabScene;
    [Export]
    private NodePath resourceBarPath;
    private VBoxContainer resourceBar;

    private Array<ResourceInfo> resources;
    private Array<ResourceTab> resourceTabs = new Array<ResourceTab>();

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("GameMenu | Initialization failed.");
            return;
        }

        PopulateResourceBar();
    }
    private bool Initialize()
    {
        // Get Resources from Resource global

        resourceBar = GetNodeOrNull<VBoxContainer>(resourceBarPath);
        if (resourceBar == null)
            return false;

        return true;
    }

    private void PopulateResourceBar()
    {
        foreach (ResourceInfo resource in resources)
        {
            ResourceTab resourceTab = resourceTabScene.Instantiate<ResourceTab>();
            resourceBar.AddChild(resourceTab);
            
            resourceTabs.Add(resourceTab);
            resourceTab.SetResourceInfo(resource);
        }
    }
}
