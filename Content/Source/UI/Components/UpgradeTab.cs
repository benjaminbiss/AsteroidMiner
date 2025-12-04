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

    public bool IsInfiniteUpgrade { get; private set; }
    public string UpgradeName { get; private set; }
    private int CurrentLevel;
    public Dictionary<string, double> BaseResourceCost { get; private set; }
    private double CostMultiplier;

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
        IsInfiniteUpgrade = false;

        UpgradeName = info.Name;
        upgradeLabel.Text = UpgradeName;
        if (info.IconPath != "")
            upgradeTextureRect.Texture = GD.Load<Texture2D>(info.IconPath);
        costLabel.Text = ParseCost(info.ResourceCost);
        descriptionLabel.Text = info.Description;
    }
    public void SetupUI(InfiniteUpgradeInfo info)
    {
        IsInfiniteUpgrade = true;
        UpgradeName = info.Name;
        CurrentLevel = info.CurrentLevel;
        BaseResourceCost = info.BaseResourceCost;
        CostMultiplier = info.CostMultiplier;

        upgradeLabel.Text = UpgradeName + CurrentLevel;
        if (info.IconPath != "")
            upgradeTextureRect.Texture = GD.Load<Texture2D>(info.IconPath);
        costLabel.Text = ParseCost(GetCurrentCost());
        descriptionLabel.Text = info.Description;
    }
    public void UpdateInfiniteUpgradeUI()
    {
        CurrentLevel += 1;
        upgradeLabel.Text = UpgradeName + CurrentLevel;
        costLabel.Text = ParseCost(GetCurrentCost());
    }
    private void UpgradeTabButtonPressed()
    {
        EmitSignal(nameof(OnUpgradeButtonClicked), this);
    }
    public void RequestAccepted()
    {
        if (IsInfiniteUpgrade)
        {
            UpdateInfiniteUpgradeUI();
            return;
        }
        button.Disabled = true;
        costLabel.Hide();
    }
    private string ParseCost(Dictionary<string, double> cost)
    {
        string costString = "";
        if (cost == null)
            return "Free";
        foreach (var resource in cost)
        {
            costString += $"{resource.Key}: {resource.Value.ToString("N0")} \n";
        }
        return costString.Trim();
    }
    public Dictionary<string, double> GetCurrentCost()
    {
        Dictionary<string, double> currentCost = new Dictionary<string, double>();
        foreach (var cost in BaseResourceCost)
        {
            double finalCost = cost.Value;
            if (CurrentLevel >= 1)
                finalCost *= CostMultiplier * CurrentLevel;
            currentCost[cost.Key] = finalCost;
        }
        return currentCost;
    }
    public Dictionary<string, double> GetPreviousCost()
    {
        Dictionary<string, double> nextCost = new Dictionary<string, double>();
        foreach (var cost in BaseResourceCost)
        {
            double finalCost = cost.Value;
            if (CurrentLevel >= 1)
                finalCost *= CostMultiplier * (CurrentLevel - 1);
            nextCost[cost.Key] = finalCost;
        }
        return nextCost;
    }
    public Dictionary<string, double> GetNextCost()
    {
        Dictionary<string, double> nextCost = new Dictionary<string, double>();
        foreach (var cost in BaseResourceCost)
        {
            double finalCost = cost.Value;
            finalCost *= CostMultiplier * (CurrentLevel + 1);
            nextCost[cost.Key] = finalCost;
        }
        return nextCost;
    }
}
