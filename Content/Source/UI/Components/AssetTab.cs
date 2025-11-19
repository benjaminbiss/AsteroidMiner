using Godot;
using System;

public partial class AssetTab : MarginContainer
{
    [Signal]
    public delegate void AssetButtonClickedEventHandler(Node sender);

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
    private NodePath progressBarPath;
    private ProgressBar progressBar;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("AssetTab | Initialization failed.");
            return;
        }
    }
    private bool Initialize()
    {
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
        progressBar = GetNodeOrNull<ProgressBar>(progressBarPath);
        if (progressBar == null)
            return false;

        return true;
    }
    public void SetupUI(AssetInfo info)
    {
        assetLabel.Text = info.Name;
        if (info.IconPath != "")
            assetTextureRect.Texture = GD.Load<Texture2D>(info.IconPath);
        levelLabel.Text = info.Level.ToString("N0");
        float rate = (float)((info.HarvestAmount * info.DeploymentSpeed) / 60);
        rateLabel.Text = $"{rate.ToString("N2")} sec";
        resourceLabel.Text = info.HarvestedResource.ToString();
    }

    public void UpdateAssetAmount(double amount)
    {
        //currentAmount.Text = amount.ToString("N0");
    }

    private void OnAssetTabButtonPressed()
    {
        EmitSignal(nameof(AssetButtonClicked), this);
    }
    public void RequestAccepted()
    {
        
    }
    public string GetAssetName()
    {
        return assetLabel.Text;
    }
}
