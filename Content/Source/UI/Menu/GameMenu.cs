using Godot;
using Godot.Collections;

public partial class GameMenu : Control
{
    [Export]
    private PackedScene resourceTabScene;
    [Export]
    private NodePath resourceBarPath;
    private HBoxContainer resourceBar;

    [Export]
    private Array<ResourceInfo> resources;
    private Array<ResourceTab> resourceTabs;

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
        bool result = true;

        resourceBar = GetNodeOrNull<HBoxContainer>(resourceBarPath);
        result = resourceBar != null;

        return result;
    }

    private void PopulateResourceBar()
    {
        foreach (ResourceInfo resource in resources)
        {
            ResourceTab resourceTab = resourceTabScene.Instantiate<ResourceTab>();
            resourceTabs.Add(resourceTab);

            resourceTab.SetResourceInfo(resource);
            resourceBar.AddChild(resourceTab);
        }
    }
}
