using Godot;
using Godot.Collections;

public partial class GameManager : Node2D
{
    [Signal]
    public delegate void UpdateResourceEventHandler(string resource);
    [Signal]
    public delegate void UpdateAssetEventHandler(string asset);

    private GameCore gameCore;

    [Export]
    private NodePath Camera2D;
    private Camera2D camera2D;

    [Export]
    private NodePath asteroidPath;
    private Asteroid asteroid;
    [Export]
    private NodePath shipRootPath;
    private AutoRotate shipRoot;
    private MiningShip miningShip;


    // Initialization
    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("GameManager | Initialization failed.");
            return;
        }

        SetupBindings();

        asteroid.Visible = false;
        shipRoot.Visible = false;
    }
    private bool Initialize()
    {
        gameCore = GameCore.Instance;
        if (gameCore == null)
            return false;
        camera2D = GetNodeOrNull<Camera2D>(Camera2D);
        if (camera2D == null)
            return false;
        asteroid = GetNodeOrNull<Asteroid>(asteroidPath);
        if (asteroid == null)
            return false;
        shipRoot = GetNodeOrNull<AutoRotate>(shipRootPath);
        if (shipRoot == null)
            return false;
        miningShip = shipRoot.GetNodeOrNull<MiningShip>("MiningShip");
        if (miningShip == null)
            return false;

        return true;
    }
    private void SetupBindings()
    {
        Main main = GetParent<Main>();
        asteroid.NewAstroidCreated += UpdateAsteroidPoints;

        miningShip.ShipCollectedCredits += AddResources;
    }

    // Runtime
    public void StartGame()
    {        
        SetupAsteroid();
        SetCameraZoom();

        asteroid.Visible = true;
        shipRoot.Visible = gameCore.gameData.Upgrades.ContainsKey("Purchase Mining Vessel");
        asteroid.SetPosition(Vector2.Zero);
        shipRoot.SetPosition(Vector2.Zero);

        miningShip.UpdateShipInfo(100f, asteroid.radius + gameCore.gameData.Defaults["planetSize"] + gameCore.gameData.Defaults["shipDistanceFromSurface"]);
        miningShip.SetPosition(new Vector2(0f, -miningShip.shipDistanceFromCenter));
    }
    private void AddResources(string resource, double amount)
    {
        gameCore.AddResource(resource, amount);
    }
    private void RemoveResources(string resource, double amount)
    {
        gameCore.AddResource(resource, -amount);
    }
    private void SetupAsteroid()
    {
        if (gameCore.gameData.AsteroidPoints != null && gameCore.gameData.AsteroidPoints.Count > 0)
        {
            asteroid.SetupAsteroidShape(gameCore.gameData.AsteroidPoints);
        }
        else
            asteroid.SetupAsteroidShape(gameCore.gameData.Defaults["planetSize"]);
    }
    public void UpdateAsteroidPoints(Array<int> points)
    {
        gameCore.UpdateAsteroid(points);
        gameCore.gameData.AsteroidPoints = points;
    }
    private void SetCameraZoom()
    {
        Dictionary<string, int> defaults = gameCore.gameData.Defaults;
        Vector2 gameSpace = new Vector2(defaults["planetSize"] + defaults["shipDistanceFromSurface"] + 100, defaults["planetSize"] + defaults["shipDistanceFromSurface"] + 100);
        camera2D.Zoom = Vector2.One / (gameSpace / 300);
        GD.Print("Camera Zoom: " + camera2D.Zoom);
    }
    public void HandleTabClicked(Node sender)
    {
        ResearchTab researchTab = sender as ResearchTab;
        if (researchTab != null)
        {
            HandleResearchUnlocks(researchTab);
            return;
        }
        UpgradeTab upgradeTab = sender as UpgradeTab;
        if (upgradeTab != null)
        {
            HandleUpgradeUnlocks(upgradeTab);
            return;
        }
        AssetTab assetTab = sender as AssetTab;
        if (assetTab != null)
        {
            HandleAssetUnlocks(assetTab);
            return;
        }
    }
    private void HandleResearchUnlocks(ResearchTab researchTab)
    {
        foreach(var cost in researchTab.researchInfo.ResourceCost)
        {
            RemoveResources(cost.Key, cost.Value);
        }
        switch (researchTab.researchInfo.Name)
        {
            case "Hangar":
                break;
            default:
                break;
        }
    }
    private void HandleUpgradeUnlocks(UpgradeTab upgradeTab)
    {
        foreach (var cost in upgradeTab.upgradeInfo.ResourceCost)
        {
            RemoveResources(cost.Key, cost.Value);
        }
        switch (upgradeTab.upgradeInfo.Name)
        {
            case "Purchase Mining Vessel":
                shipRoot.Show();
                break;
            default:
                break;
        }
    }
    private void HandleAssetUnlocks(AssetTab assetTab)
    {
        foreach (var cost in assetTab.assetInfo.ResourceCost)
        {
            RemoveResources(cost.Key, cost.Value);
        }
        switch (assetTab.assetInfo.Name)
        {
            case "Mining Laser":
                break;
            case "Mining Ship":
                break;
            default:
                break;
        }
    }
}
