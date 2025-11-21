using Godot;
using Godot.Collections;

public partial class AssetTab : MarginContainer
{
    [Signal]
    public delegate void AssetButtonClickedEventHandler(Node sender);

    [Export]
    private NodePath buttonPath;
    private Button button;
    [Export]
    private NodePath assetLabelPath;
    private Label assetLabel;
    [Export]
    private NodePath assetTexturePath;
    private TextureRect assetTextureRect;
    [Export]
    private NodePath levelLabelPath;
    private Label levelLabel;
    [Export]
    private NodePath resourceLabelPath;
    private Label resourceLabel;
    [Export]
    private NodePath rateLabelPath;
    private Label rateLabel;
    [Export]
    private NodePath costLabelPath;
    private Label costLabel;
    [Export]
    private NodePath levelProgBarPath;
    private ProgressBar levelProgBar;
    [Export]
    private NodePath deployProgBarPath;
    private ProgressBar deployProgBar;

    private bool isCreditGenerator = false;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("AssetTab | Initialization failed.");
            return;
        }

        button.Pressed += OnAssetTabButtonPressed;
    }
    private bool Initialize()
    {
        button = GetNodeOrNull<Button>(buttonPath);
        if (button == null)
            return false;
        assetLabel = GetNodeOrNull<Label>(assetLabelPath);
        if (assetLabel == null)
            return false;
        assetTextureRect = GetNodeOrNull<TextureRect>(assetTexturePath);
        if (assetTextureRect == null)
            return false;
        resourceLabel = GetNodeOrNull<Label>(resourceLabelPath);
        if (resourceLabel == null)
            return false;
        levelLabel = GetNodeOrNull<Label>(levelLabelPath);
        if (levelLabel == null)
            return false;
        rateLabel = GetNodeOrNull<Label>(rateLabelPath);
        if (rateLabel == null)
            return false;
        costLabel = GetNodeOrNull<Label>(costLabelPath);
        if (costLabel == null)
            return false;
        levelProgBar = GetNodeOrNull<ProgressBar>(levelProgBarPath);
        if (levelProgBar == null)
            return false;
        deployProgBar = GetNodeOrNull<ProgressBar>(deployProgBarPath);
        if (deployProgBar == null)
            return false;

        return true;
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (IsVisibleInTree() && isCreditGenerator)
        {
            deployProgBar.Value = deployProgBar.Value >= deployProgBar.MaxValue ? 0 : deployProgBar.Value += delta;
        }
    }
    public void SetupUI(AssetInfo info)
    {
        assetLabel.Text = info.Name;
        if (info.IconPath != "")
            assetTextureRect.Texture = GD.Load<Texture2D>(info.IconPath);
        resourceLabel.Text = info.HarvestedResource.ToString();
        levelLabel.Text = info.Level.ToString("N0");        
        rateLabel.Text = $"{CalculateRate(info.HarvestAmount, info.DeploymentSpeed).ToString("N2")} sec";
        costLabel.Text = ParseCost(info.ResourceCost);
        levelProgBar.MaxValue = 10;
        if (info.HarvestedResource == "Credits")
            isCreditGenerator = true;
    }
    public double CalculateRate(double amount, double speed)
    {
        return (amount * speed) / 60;
    }

    public void UpdateAssetAmount(double level, double rate, Dictionary<string, double> cost)
    {
        levelLabel.Text = level.ToString("N0");
        rateLabel.Text = rate.ToString("N2");
        costLabel.Text = ParseCost(cost);
        levelProgBar.Value = level % 10;
    }
    private string ParseCost(Dictionary<string, double> cost)
    {
        string costString = "";
        foreach (var item in cost)
        {
            costString += $"{item.Key}: {item.Value.ToString("N0")} \n";
        }
        return costString.Trim();
    }
    private void OnAssetTabButtonPressed()
    {
        EmitSignal(nameof(AssetButtonClicked), this);
    }
    public void UpdateDeployBar(double value, double max)
    {
        deployProgBar.Value = value;
        deployProgBar.MaxValue = max;
    }
    public string GetAssetName()
    {
        return assetLabel.Text;
    }
}
