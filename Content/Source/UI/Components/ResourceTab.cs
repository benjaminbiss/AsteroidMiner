using Godot;

public partial class ResourceTab : MarginContainer
{
    [Export]
    private NodePath resourceLabelPath;
    private Label resourceLabel;
    private TextureRect resourceTextureRect;
    private ResourceInfo resourceInfo;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("ResourceTab | Initialization failed.");
            return;
        }
    }
    private bool Initialize()
    {
        bool result = true;

        resourceLabel = GetNodeOrNull<Label>(resourceLabelPath);
        result = resourceLabel != null;
        resourceTextureRect = GetNodeOrNull<TextureRect>("ResourceIcon");
        result = resourceTextureRect != null;

        return result;
    }

    public void SetResourceInfo(ResourceInfo info)
    {
        resourceInfo = info;
        resourceLabel.Text = resourceInfo.Name;
        resourceTextureRect.Texture = resourceInfo.Icon;
    }
}
