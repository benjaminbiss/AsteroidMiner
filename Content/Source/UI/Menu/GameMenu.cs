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
    private NodePath upgradePath;
    private VBoxContainer upgradeBar;

    [Export]
    private PackedScene researchTabScene;
    [Export]
    private NodePath researchPath;
    private VBoxContainer researchBar;

    private GameCore gameCore;

    private System.Collections.Generic.Dictionary<string, AssetInfo> assets;
    public Array<AssetTab> assetTabs { get; private set; }
    private System.Collections.Generic.Dictionary<string, ResearchInfo> researches;
    public Array<ResearchTab> researchTabs { get; private set; }
    private System.Collections.Generic.Dictionary<string, ResourceInfo> resources;
    public Array<ResourceTab> resourceTabs { get; private set; }
    private System.Collections.Generic.Dictionary<string, UpgradeInfo> upgrades;
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
        upgradeBar = GetNodeOrNull<VBoxContainer>(upgradePath);
        if (upgradeTabScene == null)
            return false;
        researchBar = GetNodeOrNull<VBoxContainer>(researchPath);
        if (researchTabScene == null)
            return false;

        return true;
    }

    public void SetAssetBar(System.Collections.Generic.Dictionary<string, AssetInfo> newAssets)
    {
        assets = newAssets;
        assetTabs = new Array<AssetTab>();
        PopulateAssetBar();
    }
    public void SetResearchesBar(System.Collections.Generic.Dictionary<string, ResearchInfo> newResearches)
    {
        researches = newResearches;
        researchTabs = new Array<ResearchTab>();
        PopulateResearchBar();
    }
    public void SetResourceBar(System.Collections.Generic.Dictionary<string, ResourceInfo> newResources)
    {
        resources = newResources;
        resourceTabs = new Array<ResourceTab>();
        PopulateResourceBar();
    }
    public void SetUpgradesBar(System.Collections.Generic.Dictionary<string, UpgradeInfo> newUpgrades)
    {
        upgrades = newUpgrades;
        upgradeTabs = new Array<UpgradeTab>();
        PopulateUpgradeBar();
    }

    private void PopulateResourceBar()
    {
        foreach (string key in resources.Keys)
        {
            ResourceTab resourceTab = resourceTabScene.Instantiate<ResourceTab>();
            resourceBar.AddChild(resourceTab);
            resourceTabs.Add(resourceTab);
            resourceTab.SetResourceInfo(resources[key]);

            if (!gameCore.gameData.OwnedResources.ContainsKey(key))
            {
                resourceTab.Hide();
            }
        }
    }
    private void PopulateAssetBar()
    {
        foreach (string key in assets.Keys)
        {
            AssetTab assetTab = assetTabScene.Instantiate<AssetTab>();
            assetBar.AddChild(assetTab);
            assetTabs.Add(assetTab);
            assetTab.SetAssetInfo(assets[key]);

            if (!gameCore.gameData.OwnedAssets.ContainsKey(key))
            {
                assetTab.Hide();
            }
        }
    }

    private void PopulateResearchBar()
    {
        foreach (string key in researches.Keys)
        {
            ResearchTab researchTab = researchTabScene.Instantiate<ResearchTab>();
            researchBar.AddChild(researchTab);
            researchTabs.Add(researchTab);
            researchTab.SetResearchInfo(researches[key]);

            if (!gameCore.gameData.OwnedResearch.Contains<string>(key))
            {
                researchTab.Hide();
            }
        }
    }

    private void PopulateUpgradeBar()
    {
        foreach (string key in upgrades.Keys)
        {
            UpgradeTab upgradeTab = upgradeTabScene.Instantiate<UpgradeTab>();
            upgradeBar.AddChild(upgradeTab);
            upgradeTabs.Add(upgradeTab);
            upgradeTab.SetUpgradeInfo(upgrades[key]);

            if (!gameCore.gameData.OwnedUpgrades.Contains<string>(key))
            {
                upgradeTab.Hide();
            }
        }
    }
}
