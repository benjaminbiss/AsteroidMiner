using Godot;

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

        UpdateUI();
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
        UpdateUI();
    }

    private void UpdateUI()
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

    public void UpdateResourceAmount(string resource, double currentAmount, double maxAmount)
    {
        if (resource != resourceInfo.Name)
            return;

        this.currentAmount.Text = currentAmount.ToString();

        if (resourceInfo.Name == "Credits")
            return;

        this.maxAmount.Text = maxAmount.ToString();
        progressBar.Value = currentAmount;
        progressBar.MaxValue = maxAmount;
    }
}
