using Godot;
using Godot.Collections;
using System;

public partial class AssetManager : Node
{
    [Signal]
    public delegate void TabUpgradedEventHandler(Node sender);

    private GameCore gameCore;
    private GameMenu gameMenu;

    [Export]
    private PackedScene assetTabScene;
    [Export]
    private NodePath assetPath;
    private VBoxContainer assetBar;

    private Dictionary<string, AssetTab> assetTabs = new Dictionary<string, AssetTab>();

    // Initialization
    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("AssetManager | Initialization failed.");
            return;
        }

        gameCore.PrerequisitesUpdated += CheckPrerequisites;

    }
    private bool Initialize()
    {
        gameCore = GameCore.Instance;
        if (gameCore == null)
            return false;
        if (assetTabScene == null)
            return false;
        assetBar = GetNodeOrNull<VBoxContainer>(assetPath);
        if (assetBar == null)
            return false;

        return true;
    }
    public void Setup(GameMenu menu)
    { 
        gameMenu = menu;
        PopulateAssetBar();
    }

    // Runtime
    private void PopulateAssetBar()
    {
        if (gameCore == null || gameCore.gameData == null)
            return;

        foreach (var asset in gameCore.gameData.Assets)
        {
            AssetTab assetTab = assetTabScene.Instantiate<AssetTab>();
            assetBar.AddChild(assetTab);
            assetTabs.Add(asset.Key, assetTab);
            assetTab.SetAssetInfo(asset.Value);
            assetTab.AssetButtonClicked += HandleAssetTabClicked;

            if (!gameMenu.HasAllPrerequisites(asset.Value.Prerequisites))
            {
                assetTab.Hide();
            }
        }
    }
    public void HandleAssetTabClicked(Node sender)
    {
        AssetTab assetTab = sender as AssetTab;
        foreach (var cost in assetTab.assetInfo.ResourceCost)
        {
            if (cost.Value > gameCore.GetResourceAmount(cost.Key))
                return;
        }
        assetTab.RequestAccepted();
        EmitSignal(SignalName.TabUpgraded, sender);
    }
    private void CheckPrerequisites()
    {
        foreach (AssetTab assetTab in assetTabs.Values)
        {
            if (gameMenu.HasAllPrerequisites(assetTab.assetInfo.Prerequisites))
            {
                assetTab.Show();
            }
        }
    }
    public void UpdateAssetTab(string asset)
    {

    }

}
