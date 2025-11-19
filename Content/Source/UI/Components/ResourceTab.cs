using Godot;
using Godot.Collections;

public partial class ResourceTab : MarginContainer
{
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

    public void SetupUI(ResourceInfo info)
    {
        resourceLabel.Text = info.Name;
        if (info.IconPath != "")
            resourceTextureRect.Texture = GD.Load<Texture2D>(info.IconPath);
        currentAmount.Text = info.Current.ToString("N2");
    }

    public void UpdateResourceAmount(double amount)
    {
        currentAmount.Text = amount.ToString("N2");
    }
}
