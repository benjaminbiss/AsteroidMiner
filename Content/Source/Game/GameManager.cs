using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public partial class GameManager : Node2D
{
    [Signal]
    public delegate void UpdateResourceEventHandler(string resource, string param);
    [Signal]
    public delegate void AddResourceEventHandler(string resource, string param, double amount);
    [Signal]
    public delegate void UpdateAssetEventHandler(string asset, string param);
    [Signal]
    public delegate void AddAssetEventHandler(string asset, string param, double amount);

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

    private GameCore gameCore;


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
    public void StartGame()
    {        
        SetupAsteroid();
        SetCameraZoom();

        asteroid.Visible = true;
        shipRoot.Visible = gameCore.gameData.upgrades.Contains<string>("Purchase Mining Vessel");
        asteroid.SetPosition(Vector2.Zero);
        shipRoot.SetPosition(Vector2.Zero);

        miningShip.UpdateShipInfo(100f, asteroid.radius + gameCore.defaultInfos["planetSize"] + gameCore.defaultInfos["shipDistanceFromSurface"]);
        miningShip.SetPosition(new Vector2(0f, -miningShip.shipDistanceFromCenter));

        UpdateResourceUI();
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Visible == false)
            return;

        GeneratePower(delta);
    }
    private void UpdateResourceUI()
    {
        foreach (string key in gameCore.gameData.resources.Keys)
        {
            foreach (var resourceValues in gameCore.gameData.resources[key])
            {
                EmitSignal(nameof(UpdateResource), key, resourceValues.Key);
            }
        }
    }

    private void SetupBindings()
    {
        Main main = GetParent<Main>();
        asteroid.NewAstroidCreated += main.UpdateAsteroidPoints;

        miningShip.ShipCollectedCredits += AddResources;
    }
    private void AddResources(string resource, string param, double amount)
    {
        EmitSignal(nameof(AddResource), resource, param, amount);
    }
    private void GeneratePower(double delta)
    {
        if (gameCore.gameData.resources.ContainsKey("Power") == false)
            return;

        double additiveRate = 0f;
        double multiplicativeRate = 1f;
        foreach (var mod in gameCore.gameData.resourceModifiers["Power"]["GenerationAmount"])
        { 
            if (mod.Key)
                additiveRate += mod.Value;
            else
                multiplicativeRate *= mod.Value;
        }
        UpdatePower(additiveRate * multiplicativeRate * delta);
    }
    private void UpdatePower(double power)
    {
        double current = gameCore.gameData.resources["Power"].Values.First();
        double max = gameCore.gameData.resources["Power"].Values.Last();
        EmitSignal(SignalName.UpdateResource, "Power", Mathf.Min(current += power, max), max);
    }
    private void SetupAsteroid()
    {
        if (gameCore.gameData.AsteroidPoints != null && gameCore.gameData.AsteroidPoints.Length > 0)
        {
            asteroid.SetupAsteroidShape(gameCore.gameData.AsteroidPoints);
        }
        else
            asteroid.SetupAsteroidShape(gameCore.defaultInfos["planetSize"]);
    }
    private void SetCameraZoom()
    {
        Vector2 gameSpace = new Vector2(gameCore.defaultInfos["planetSize"] + gameCore.defaultInfos["shipDistanceFromSurface"] + 100, gameCore.defaultInfos["planetSize"] + gameCore.defaultInfos["shipDistanceFromSurface"] + 100);
        camera2D.Zoom = Vector2.One / (gameSpace / 300);
        GD.Print("Camera Zoom: " + camera2D.Zoom);
    }
    public void HandleUnlockLogic(Node sender)
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
            AddResources(cost.Key, "Current", -1 * cost.Value);
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
            AddResources(cost.Key, "Current", -1 * cost.Value);
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
            AddResources(cost.Key, "Current", -1 * cost.Value);
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
