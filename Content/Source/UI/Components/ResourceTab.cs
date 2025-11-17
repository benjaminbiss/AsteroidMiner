using Godot;
using Godot.Collections;

public partial class ResourceTab : MarginContainer
{
    private ResourceInfo resourceInfo;    
    
    [Export]
    private NodePath resourceLabelPath;
    private Label resourceLabel;
    [Export]
    private NodePath resourceTexturePath;
    private TextureRect resourceTextureRect;
    [Export]
    private NodePath currentAmountPath;
    private Label currentAmount;



    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("ResourceTab | Initialization failed.");
            return;
        }

        GameCore.Instance.ResourcesUpdated += UpdateResourceAmount;
    }
    private bool Initialize()
    {
        resourceLabel = GetNodeOrNull<Label>(resourceLabelPath);
        if (resourceLabel == null)
            return false;
        resourceTextureRect = GetNodeOrNull<TextureRect>(resourceTexturePath);
        if (resourceTextureRect == null)
            return false;
        currentAmount = GetNodeOrNull<Label>(currentAmountPath);
        if (currentAmount == null)
            return false;

        return true;
    }

    public void SetResourceInfo(ResourceInfo info)
    {
        resourceInfo = info;
        SetupUI();
    }

    private void SetupUI()
    {
        if (resourceInfo == null)
            return;
        
        resourceLabel.Text = resourceInfo.Name;
        if (resourceInfo.IconPath != "")
            resourceTextureRect.Texture = GD.Load<Texture2D>(resourceInfo.IconPath);
    }

    public void UpdateResourceAmount(string resource)
    {
        if (resource != resourceInfo.Name)
            return;

        currentAmount.Text = resourceInfo.Current.ToString("N0");
    }
}
