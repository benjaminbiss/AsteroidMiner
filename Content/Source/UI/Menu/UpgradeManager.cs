using Godot;
using Godot.Collections;
using System;

public partial class UpgradeManager : Node
{
    [Signal]
    public delegate void UnlockedNewUpgradeEventHandler(Node sender);

    private GameCore gameCore;
    private GameMenu gameMenu;

    [Export]
    private PackedScene upgradeTabScene;
    [Export]
    private NodePath availableUpgradePath;
    private VBoxContainer availableUpgradeBar;
    [Export]
    private NodePath ownedUpgradePath;
    private VBoxContainer ownedUpgradeBar;
    
    private Dictionary<string, UpgradeTab> upgradeTabs = new Dictionary<string, UpgradeTab>();

    // Initialization
    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("UpgradeManager | Initialization failed.");
            return;
        }

        gameCore.PrerequisitesUpdated += CheckPrerequisites;
    }
    private bool Initialize()
    {
        gameCore = GameCore.Instance;
        if (gameCore == null)
            return false;
        availableUpgradeBar = GetNodeOrNull<VBoxContainer>(availableUpgradePath);
        if (upgradeTabScene == null)
            return false;
        ownedUpgradeBar = GetNodeOrNull<VBoxContainer>(ownedUpgradePath);
        if (ownedUpgradeBar == null)
            return false;

        return true;
    }
    public void Setup(GameMenu menu)
    {
        gameMenu = menu;
        PopulateUpgradeBars();
    }

    // Runtime
    private void PopulateUpgradeBars()
    {
        if (gameCore == null || gameCore.gameData == null)
            return;

        foreach (var upgrade in gameCore.gameData.Upgrades)
        {
            UpgradeTab upgradeTab = upgradeTabScene.Instantiate<UpgradeTab>();
            if (gameMenu.HasAllPrerequisites(upgrade.Value.Prerequisites))
            {
                ownedUpgradeBar.AddChild(upgradeTab);
            }
            else
            {
                availableUpgradeBar.AddChild(upgradeTab);
                if (!gameMenu.HasAnyPrerequisites(upgrade.Value.Prerequisites))
                {
                    upgradeTab.Hide();
                }
            }
            upgradeTabs.Add(upgrade.Key, upgradeTab);
            upgradeTab.SetUpgradeInfo(upgrade.Value);
            upgradeTab.UpgradeButtonClicked += HandleUpgradeTabClicked;
        }
    }
    private void HandleUpgradeTabClicked(Node sender)
    {
        UpgradeTab upgradeTab = sender as UpgradeTab;
        foreach (var cost in upgradeTab.upgradeInfo.ResourceCost)
        {
            if (cost.Value > gameCore.GetResourceAmount(cost.Key))
                return;
        }
        upgradeTab.RequestAccepted();
        availableUpgradeBar.RemoveChild(sender);
        ownedUpgradeBar.AddChild(sender);
        gameCore.AddUpgrade(upgradeTab.upgradeInfo.Name);
        EmitSignal(SignalName.UnlockedNewUpgrade, sender);
        CheckPrerequisites();
    }
    private void CheckPrerequisites()
    {
        foreach (UpgradeTab upgradeTab in upgradeTabs.Values)
        {
            if (gameMenu.HasAllPrerequisites(upgradeTab.upgradeInfo.Prerequisites))
            {
                upgradeTab.Show();
            }
        }
    }
    private void UpdateUpgradeTab(string upgrade)
    {
    }
}
