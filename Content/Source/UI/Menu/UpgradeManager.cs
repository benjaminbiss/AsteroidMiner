using Godot;
using Godot.Collections;

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
            AddUpgradeTab(upgrade);
        }
        foreach (var upgrade in gameCore.gameData.InfiniteUpgrades)
        {
            AddInfiniteUpgradeTab(upgrade);
        }
    }
    private void AddUpgradeTab(System.Collections.Generic.KeyValuePair<string, UpgradeInfo> upgrade)
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
        upgradeTab.SetupUI(upgrade.Value);
        upgradeTab.OnUpgradeButtonClicked += HandleUpgradeTabClicked;
    }
    private void AddInfiniteUpgradeTab(System.Collections.Generic.KeyValuePair<string, InfiniteUpgradeInfo> upgrade)
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
        upgradeTab.SetupUI(upgrade.Value);
        upgradeTab.OnUpgradeButtonClicked += HandleUpgradeTabClicked;
    }
    private void HandleUpgradeTabClicked(Node sender)
    {
        UpgradeTab upgradeTab = sender as UpgradeTab;
        string name = upgradeTab.UpgradeName;
        if (!upgradeTab.IsInfiniteUpgrade)
        {
            if (gameCore.gameData.Upgrades[name].ResourceCost != null)
            {
                foreach (var cost in gameCore.gameData.Upgrades[name].ResourceCost)
                {
                    if (cost.Value > gameCore.GetResourceAmount(cost.Key))
                        return;
                }
            }
            availableUpgradeBar.RemoveChild(sender);
            ownedUpgradeBar.AddChild(sender);
            gameCore.AddUpgrade(name);
        }
        else
        {
            foreach (var cost in upgradeTab.GetCurrentCost())
            {
                if (cost.Value > gameCore.GetResourceAmount(cost.Key))
                    return;
            }
            gameCore.AddInfiniteUpgrade(name);
        }
        upgradeTab.RequestAccepted();
        EmitSignal(SignalName.UnlockedNewUpgrade, sender);
        CheckPrerequisites();
    }
    private void CheckPrerequisites()
    {
        foreach (var tab in upgradeTabs)
        {
            UpgradeTab upgradeTab = tab.Value;
            if (upgradeTab.Visible == false)
            {
                if (tab.Value.IsInfiniteUpgrade)
                {
                    if (gameMenu.HasAllPrerequisites(gameCore.gameData.InfiniteUpgrades[tab.Key].Prerequisites))
                        upgradeTab.Show();
                }
                else
                {
                    if (gameMenu.HasAllPrerequisites(gameCore.gameData.Upgrades[tab.Key].Prerequisites))
                        upgradeTab.Show();
                }
            }
        }
    }
}
