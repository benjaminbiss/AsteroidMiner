using Godot;
using Godot.Collections;

public partial class AssetManager : Node
{
    [Signal]
    public delegate void AssetUpgradedEventHandler(Node sender);
    //[Signal]
    //public delegate void AssetAddedEventHandler(Node sender);

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

        gameCore.AssetUpdated += UpdateAssetTab;
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
            assetTab.SetupUI(asset.Value);
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
        string name = assetTab.GetAssetName();
        foreach (var cost in gameCore.gameData.Assets[name].ResourceCost)
        {
            if (cost.Value > gameCore.GetResourceAmount(cost.Key))
                return;
        }
        gameCore.UpgradeAsset(name);
        EmitSignal(SignalName.AssetUpgraded, sender);
    }
    private void CheckPrerequisites()
    {
        foreach (var tab in assetTabs)
        {
            AssetTab assetTab = tab.Value;
            if (assetTab.Visible == false)
            {
                if (gameMenu.HasAllPrerequisites(gameCore.gameData.Assets[tab.Key].Prerequisites))
                {
                    assetTab.Show();
                    string name = assetTab.GetAssetName();
                    gameCore.UpgradeAsset(name);
                    //EmitSignal(SignalName.AssetAdded, assetTab);
                }
            }
        }
    }
    public void UpdateAssetTab(string asset)
    {
        foreach (var tab in assetTabs)
        {
            if (tab.Key == asset)
            {
                AssetTab assetTab = tab.Value;
                double rate = assetTab.CalculateRate(gameCore.gameData.Assets[asset].HarvestAmount, gameCore.gameData.Assets[asset].DeploymentSpeed);
                assetTab.UpdateAssetAmount(gameCore.gameData.Assets[asset].Level, rate, gameCore.gameData.Assets[asset].ResourceCost);
                break;
            }
        }
    }
    public AssetTab GetMiningLaserTab()
    {
        return assetTabs["Mining Laser"];
    }
}
