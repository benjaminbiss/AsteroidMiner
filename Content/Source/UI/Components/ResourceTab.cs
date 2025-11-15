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
    private NodePath creditAmountPath;
    [Export]
    private NodePath currentAmountPath;
    private Label currentAmount;
    [Export]
    private NodePath maxAmountPath;
    private Label maxAmount;
    [Export]
    private NodePath progressBarPath;
    private ProgressBar progressBar;


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
        if (creditAmountPath == null)
            return false;
        maxAmount = GetNodeOrNull<Label>(maxAmountPath);
        if (maxAmount == null)
            return false;
        progressBar = GetNodeOrNull<ProgressBar>(progressBarPath);
        if (progressBar == null)
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

        if (resourceInfo.Name == "Credits")
        {
            progressBar.Hide();
            currentAmount = GetNodeOrNull<Label>(creditAmountPath);
            currentAmount.Show();
        }
        else
        {
            GetNodeOrNull<Label>(creditAmountPath).Hide();
        }
    }

    public void UpdateResourceAmount(string resource, string param, double amount)
    {
        if (resource != resourceInfo.Name)
            return;

        switch (param)
        {
            case "Current":
                currentAmount.Text = amount.ToString("N0");
                progressBar.Value = amount;
                break;
            case "Max":
                maxAmount.Text = amount.ToString("N0");
                progressBar.MaxValue = amount;
                break;
            case "Rate":
                break;
        }
    }
}
