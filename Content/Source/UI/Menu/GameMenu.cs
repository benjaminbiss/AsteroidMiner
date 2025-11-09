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

    private Resources GlobalResourceInstance;
    private Array<ResourceInfo> resources;
    private Array<ResourceTab> resourceTabs = new Array<ResourceTab>();

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("GameMenu | Initialization failed.");
            return;
        }

        resources = GlobalResourceInstance.GetAllResourceInfos();
        PopulateResourceBar();
    }
    private bool Initialize()
    {
        GlobalResourceInstance = Resources.Instance;
        if (GlobalResourceInstance == null)
        {
            GD.PrintErr("GameMenu | Global Resources Singleton instance not found.");
            return false;
        }

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
