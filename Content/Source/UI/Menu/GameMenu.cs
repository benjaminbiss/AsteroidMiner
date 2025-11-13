using Godot;
using Godot.Collections;
using System.Linq;

public partial class GameMenu : Control
{
    [Export]
    private PackedScene resourceTabScene;
    [Export]
    private NodePath resourceBarPath;
    private VBoxContainer resourceBar;

    [Export]
    private PackedScene assetTabScene;
    [Export]
    private NodePath assetPath;
    private VBoxContainer assetBar;

    [Export]
    private PackedScene upgradeTabScene;
    [Export]
    private NodePath availableUpgradePath;
    private VBoxContainer availableUpgradeBar;
    [Export]
    private NodePath ownedUpgradePath;
    private VBoxContainer ownedUpgradeBar;

    [Export]
    private PackedScene researchTabScene;
    [Export]
    private NodePath availableResearchPath;
    private VBoxContainer availableResearchBar;
    [Export]
    private NodePath ownedResearchPath;
    private VBoxContainer ownedResearchBar;

    private GameCore gameCore;

    public Array<AssetTab> assetTabs { get; private set; }
    public Array<ResearchTab> researchTabs { get; private set; }
    public Array<ResourceTab> resourceTabs { get; private set; }
    public Array<UpgradeTab> upgradeTabs { get; private set; }

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("GameMenu | Initialization failed.");
            return;
        }
    }
    private bool Initialize()
    {
        gameCore = GameCore.Instance;
        if (gameCore == null)
            return false;
        if (resourceTabScene == null)
            return false;
        resourceBar = GetNodeOrNull<VBoxContainer>(resourceBarPath);
        if (resourceBar == null)
            return false;
        if (assetTabScene == null)
            return false;
        assetBar = GetNodeOrNull<VBoxContainer>(assetPath);
        if (assetBar == null)
            return false;
        availableUpgradeBar = GetNodeOrNull<VBoxContainer>(availableUpgradePath);
        if (upgradeTabScene == null)
            return false;
        ownedUpgradeBar = GetNodeOrNull<VBoxContainer>(ownedUpgradePath);
        if (ownedUpgradeBar == null)
            return false;
        availableResearchBar = GetNodeOrNull<VBoxContainer>(availableResearchPath);
        if (researchTabScene == null)
            return false;
        ownedResearchBar = GetNodeOrNull<VBoxContainer>(ownedResearchPath);
        if (ownedResearchBar == null)
            return false;

        return true;
    }

    public void SetAssetBar(System.Collections.Generic.Dictionary<string, AssetInfo> newAssets)
    {
        assetTabs = new Array<AssetTab>();
        PopulateAssetBar();
    }
    public void SetResearchesBar(System.Collections.Generic.Dictionary<string, ResearchInfo> newResearches)
    {
        researchTabs = new Array<ResearchTab>();
        PopulateResearchBars();
    }
    public void SetResourceBar(System.Collections.Generic.Dictionary<string, ResourceInfo> newResources)
    {
        resourceTabs = new Array<ResourceTab>();
        PopulateResourceBar();
    }
    public void SetUpgradesBar(System.Collections.Generic.Dictionary<string, UpgradeInfo> newUpgrades)
    {
        upgradeTabs = new Array<UpgradeTab>();
        PopulateUpgradeBars();
    }

    private void PopulateResourceBar()
    {
        foreach (string key in gameCore.gameData.resources.Keys)
        {
            ResourceTab resourceTab = resourceTabScene.Instantiate<ResourceTab>();
            resourceBar.AddChild(resourceTab);
            resourceTabs.Add(resourceTab);
            resourceTab.SetResourceInfo(gameCore.resourceInfos[key]);

            if (!gameCore.gameData.resources.ContainsKey(key))
            {
                resourceTab.Hide();
            }
        }
    }
    private void PopulateAssetBar()
    {
        foreach (string key in gameCore.gameData.assets.Keys)
        {
            AssetTab assetTab = assetTabScene.Instantiate<AssetTab>();
            assetBar.AddChild(assetTab);
            assetTabs.Add(assetTab);
            assetTab.SetAssetInfo(gameCore.assetInfos[key]);

            if (!gameCore.gameData.assets.ContainsKey(key))
            {
                assetTab.Hide();
            }
        }
    }

    private void PopulateResearchBars()
    {
        foreach (string key in gameCore.gameData.researches)
        {
            ResearchTab researchTab = researchTabScene.Instantiate<ResearchTab>();
            if (gameCore.gameData.researches.Contains<string>(key))
            {
                ownedResearchBar.AddChild(researchTab);
            }
            else
            {
                availableResearchBar.AddChild(researchTab);
            }
            researchTabs.Add(researchTab);
            researchTab.SetResearchInfo(gameCore.researchInfos[key]);
            researchTab.ResearchButtonClicked += HandleResearchTabClicked;
        }
    }

    private void PopulateUpgradeBars()
    {
        foreach (string key in gameCore.gameData.upgrades)
        {
            UpgradeTab upgradeTab = upgradeTabScene.Instantiate<UpgradeTab>();
            if (gameCore.gameData.upgrades.Contains<string>(key))
            {
                ownedUpgradeBar.AddChild(upgradeTab);
            }
            else
            { 
                availableUpgradeBar.AddChild(upgradeTab);
            }
            upgradeTabs.Add(upgradeTab);
            upgradeTab.SetUpgradeInfo(gameCore.upgradeInfos[key]);
            upgradeTab.UpgradeButtonClicked += HandleUpgradeTabClicked;
        }
    }

    private void HandleResearchTabClicked(ResearchTab sender)
    {
        availableResearchBar.RemoveChild(sender);
        ownedResearchBar.AddChild(sender);
        sender.button.Disabled = true;
    }
    private void HandleUpgradeTabClicked(UpgradeTab sender)
    {
        availableUpgradeBar.RemoveChild(sender);
        ownedUpgradeBar.AddChild(sender);
        sender.button.Disabled = true;
    }
}
