using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Reflection;

public partial class GameMenu : Control
{
    [Signal]
    public delegate void UnlockedNewTabEventHandler(Node sender);
    [Signal]
    public delegate void TabUpgradedEventHandler(Node sender);

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
        foreach (var resource in gameCore.resourceInfos)
        {
            ResourceTab resourceTab = resourceTabScene.Instantiate<ResourceTab>();
            resourceBar.AddChild(resourceTab);
            resourceTabs.Add(resourceTab);
            resourceTab.SetResourceInfo(gameCore.resourceInfos[resource.Key]);

            if (!gameCore.gameData.resources.ContainsKey(resource.Key))
            {
                resourceTab.Hide();
            }
        }
    }
    private void PopulateAssetBar()
    {
        foreach (var asset in gameCore.assetInfos)
        {
            AssetTab assetTab = assetTabScene.Instantiate<AssetTab>();
            assetBar.AddChild(assetTab);
            assetTabs.Add(assetTab);
            assetTab.SetAssetInfo(gameCore.assetInfos[asset.Key]);
            assetTab.AssetButtonClicked += HandleAssetTabClicked;

            if (!gameCore.gameData.assets.ContainsKey(asset.Key))
            {
                assetTab.Hide();
            }
        }
    }

    private void PopulateResearchBars()
    {
        foreach (var research in gameCore.researchInfos)
        {
            ResearchTab researchTab = researchTabScene.Instantiate<ResearchTab>();
            researchTabs.Add(researchTab);
            researchTab.SetResearchInfo(gameCore.researchInfos[research.Key]);
            researchTab.ResearchButtonClicked += HandleResearchTabClicked;

            if (gameCore.gameData.researches.Contains<string>(research.Key))
            {
                ownedResearchBar.AddChild(researchTab);
            }
            else
            {
                availableResearchBar.AddChild(researchTab);
                if (gameCore.researchInfos[research.Key].Prerequisites != null)
                { 
                    foreach (string prereq in gameCore.researchInfos[research.Key].Prerequisites)
                    {
                        if (!gameCore.gameData.researches.Contains<string>(prereq))
                        {
                            researchTab.Hide();
                            break;
                        }
                        else if (!gameCore.gameData.upgrades.Contains<string>(prereq))
                        {
                            researchTab.Hide();
                            break;
                        }
                    }
                }
            }
        }
    }
    private void PopulateUpgradeBars()
    {
        foreach (var upgrade in gameCore.upgradeInfos)
        {
            UpgradeTab upgradeTab = upgradeTabScene.Instantiate<UpgradeTab>();
            if (gameCore.gameData.upgrades.Contains<string>(upgrade.Key))
            {
                ownedUpgradeBar.AddChild(upgradeTab);
            }
            else
            { 
                availableUpgradeBar.AddChild(upgradeTab);
                if (gameCore.upgradeInfos[upgrade.Key].Prerequisites != null)
                {
                    foreach (string prereq in gameCore.upgradeInfos[upgrade.Key].Prerequisites)
                    {
                        if (!gameCore.gameData.researches.Contains<string>(prereq))
                        {
                            upgradeTab.Hide();
                            break;
                        }
                        else if (!gameCore.gameData.upgrades.Contains<string>(prereq))
                        {
                            upgradeTab.Hide();
                            break;
                        }
                    }
                }
            }
            upgradeTabs.Add(upgradeTab);
            upgradeTab.SetUpgradeInfo(gameCore.upgradeInfos[upgrade.Key]);
            upgradeTab.UpgradeButtonClicked += HandleUpgradeTabClicked;
        }
    }

    private void HandleResearchTabClicked(Node sender)
    {
        ResearchTab researchTab = sender as ResearchTab;
        foreach (var cost in researchTab.researchInfo.ResourceCost)
        {
            if (cost.Value > gameCore.GetResourceAmount(cost.Key))
                return;
        }
        researchTab.RequestAccepted();
        availableResearchBar.RemoveChild(sender);
        ownedResearchBar.AddChild(sender);
        gameCore.AddResearch(researchTab.researchInfo.Name);
        EmitSignal(SignalName.UnlockedNewTab, sender);
        CheckPrerequisites();
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
        EmitSignal(SignalName.UnlockedNewTab, sender);
        CheckPrerequisites();
    }
    private void HandleAssetTabClicked(Node sender)
    {
        AssetTab assetTab = sender as AssetTab;
        foreach (var cost in assetTab.assetInfo.ResourceCost)
        {
            if (cost.Value > gameCore.GetResourceAmount(cost.Key))
                return;
        }
        assetTab.RequestAccepted();
        //gameCore.AddAsset();
        EmitSignal(SignalName.TabUpgraded, sender);
        CheckPrerequisites();
    }
    private void CheckPrerequisites()
    {
        string[] prereqs = gameCore.gameData.researches.Concat(gameCore.gameData.upgrades).ToArray();
        foreach (ResearchTab availableResearch in availableResearchBar.GetChildren())
        {
            bool canUnlock = true;
            foreach (string name in availableResearch.researchInfo.Prerequisites)
            {
                if (!prereqs.Contains<string>(name))
                {
                    canUnlock = false;
                }
            }
            if (canUnlock)
                availableResearch.Show();
        }
        foreach (UpgradeTab availableUpgrade in availableUpgradeBar.GetChildren())
        {
            bool canUnlock = true;
            foreach (string name in availableUpgrade.upgradeInfo.Prerequisites)
            {
                if (!prereqs.Contains<string>(name))
                {
                    canUnlock = false;
                }
            }
            if (canUnlock)
                availableUpgrade.Show();
        }
        foreach (AssetTab assetTab in assetTabs)
        {
            if (assetTab.Visible == true)
                continue;

            bool canUnlock = true;
            foreach (string name in assetTab.assetInfo.Prerequisites)
            {
                if (!prereqs.Contains<string>(name))
                {
                    canUnlock = false;
                }
            }
            if (canUnlock)
            {
                assetTab.Show();
                EmitSignal(SignalName.UnlockedNewTab, assetTab);
            }
        }
    }
}
