using Godot;
using Godot.Collections;
using System;

public partial class UpgradeTab : MarginContainer
{
    [Signal]
    public delegate void OnUpgradeButtonClickedEventHandler(Node sender);

    [Export]
    private NodePath buttonPath;
    private Button button;
    [Export]
    private NodePath upgradeLabelPath;
    private Label upgradeLabel;
    [Export]
    private NodePath upgradeTexturePath;
    private TextureRect upgradeTextureRect;
    [Export]
    private NodePath costLabelPath;
    private Label costLabel;
    [Export]
    private NodePath descriptionPath;
    private Label descriptionLabel;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("UpgradeTab | Initialization failed.");
            return;
        }

        button.Pressed += UpgradeTabButtonPressed;
    }
    private bool Initialize()
    {
        button = GetNodeOrNull<Button>(buttonPath);
        if (button == null)
            return false;
        upgradeLabel = GetNodeOrNull<Label>(upgradeLabelPath);
        if (upgradeLabel == null)
            return false;
        upgradeTextureRect = GetNodeOrNull<TextureRect>(upgradeTexturePath);
        if (upgradeTextureRect == null)
            return false;
        costLabel = GetNodeOrNull<Label>(costLabelPath);
        if (costLabel == null)
            return false;
        descriptionLabel = GetNodeOrNull<Label>(descriptionPath);
        if (descriptionLabel == null)
            return false;

        return true;
    }

    public void SetupUI(UpgradeInfo info)
    {
        upgradeLabel.Text = info.Name;
        if (info.IconPath != "")
            upgradeTextureRect.Texture = GD.Load<Texture2D>(info.IconPath);
        costLabel.Text = ParseCost(info.ResourceCost);
        descriptionLabel.Text = info.Description;

    }
    private void UpgradeTabButtonPressed()
    {
        EmitSignal(nameof(OnUpgradeButtonClicked), this);
    }
    public void RequestAccepted()
    {
        button.Disabled = true;
        costLabel.Hide();
    }
    private string ParseCost(Dictionary<string, double> cost)
    {
        string costString = "";
        if (cost == null)
            return "Free";
        foreach (var item in cost)
        {
            costString += $"{item.Key}: {item.Value.ToString("N0")} \n";
        }
        return costString.Trim();
    }
    public string GetUpgradeName()
    {
        return upgradeLabel.Text;
    }
}
