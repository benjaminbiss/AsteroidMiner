using Godot;
using Godot.Collections;
using System.Xml.Linq;

public partial class GameManager : Node2D
{
    [Signal]
    public delegate void UpdateResourceEventHandler(string resource);
    [Signal]
    public delegate void UpdateAssetEventHandler(string asset);
    [Signal]
    public delegate void UpdateResearchEventHandler(string research, string resource, double amount);
    [Signal]
    public delegate void ResearchUnlockedEventHandler(string research);

    private GameCore gameCore; 
    private string activeResearch = "";

    [Export]
    private NodePath Camera2D;
    private Camera2D camera2D;

    [Export]
    private NodePath asteroidPath;
    private Asteroid asteroid;
    [Export]
    private NodePath shipRootPath;
    private AutoRotate shipRoot;
    private MiningVessel miningShip;


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
        miningShip = shipRoot.GetNodeOrNull<MiningVessel>("MiningVessel");
        if (miningShip == null)
            return false;

        return true;
    }
    private void SetupBindings()
    {
        Main main = GetParent<Main>();
        asteroid.NewAstroidCreated += UpdateAsteroidPoints;

        miningShip.CollectedCredits += AddResources;
    }

    // Runtime
    public override void _Process(double delta)
    {
        CalculateAllPowerGenerators(delta);
        WorkingOnResearch();
    }
    private void CalculateAllPowerGenerators(double delta)
    {
        double amount = 0;
        foreach (var asset in gameCore.gameData.Assets)
        {
            AssetInfo assetInfo = asset.Value;
            if (assetInfo.Level <= 0 || assetInfo.HarvestedResource == "Credits")
                continue;

            amount += delta * ((assetInfo.DeploymentSpeed * assetInfo.HarvestAmount) / 60d);
        }
        if (amount > 0)
            AddResources("Power", amount);
    }
    public void StartGame()
    {        
        SetupAsteroid();
        SetCameraZoom();

        asteroid.Visible = true;
        shipRoot.Visible = gameCore.gameData.Prerequisites.Contains("Purchase Mining Vessel");
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
    }
    public void HandleTabClickedEvent(Node sender)
    {        
        ResearchTab researchTab = sender as ResearchTab;
        if (researchTab != null)
        {
            HandleResearchSelection(researchTab);
            return;
        }
        UpgradeTab upgradeTab = sender as UpgradeTab;
        if (upgradeTab != null)
        {
            HandleUpgradeUnlocks(upgradeTab);
            return;
        }
    }
    private void HandleResearchSelection(ResearchTab researchTab)
    {
        activeResearch = researchTab.GetResearchName();        
    }
    public void HandleResearchDeselection()
    {
        activeResearch = "";
    }
    private void WorkingOnResearch()
    {
        if (activeResearch == "")
            return;

        bool unlocked = true;
        if (gameCore.gameData.Researches[activeResearch].ResourceCost != null)
        {
            foreach (var cost in gameCore.gameData.Researches[activeResearch].ResourceCost)
            {
                if (cost.Value > gameCore.GetResourceAmount(cost.Key))
                {
                    double amount = gameCore.GetResourceAmount(cost.Key);
                    gameCore.gameData.Researches[activeResearch].ResourceCost[cost.Key] -= amount;
                    RemoveResources(cost.Key, gameCore.GetResourceAmount(cost.Key));
                    EmitSignal(SignalName.UpdateResearch, activeResearch, cost.Key, amount);
                    unlocked = false;
                }
                else
                {
                    gameCore.ChargeCost(cost.Key, -cost.Value);
                    EmitSignal(SignalName.UpdateResearch, activeResearch, cost.Key, 0);
                }
            }
        }
        if (unlocked)
        {
            EmitSignal(SignalName.ResearchUnlocked, activeResearch);
            activeResearch = "";
        }
    }
    private void HandleUpgradeUnlocks(UpgradeTab upgradeTab)
    {
        string upgradeName = upgradeTab.GetUpgradeName();
        gameCore.ChargeResourceCosts(gameCore.gameData.Upgrades[upgradeName].ResourceCost);

        switch (upgradeName)
        {
            case "Purchase Mining Vessel":
                shipRoot.Show();
                break;
            default:
                break;
        }
    }
}
