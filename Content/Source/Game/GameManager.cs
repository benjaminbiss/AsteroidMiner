using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node2D
{
    [Signal]
    public delegate void ResourcesUpdatedEventHandler(string resource, double current, double max);

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

    public Dictionary<string, AssetInfo> assetsDictionary { get; set; }
    public Dictionary<string, int> gameDefaults { get; set; }
    public Dictionary<string, ResearchInfo> researchDictionary { get; set; }
    public Dictionary<string, ResourceInfo> resourcesDictionary { get; set; }
    public Dictionary<string, UpgradeInfo> upgradesDictionary { get; set; }


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
        shipRoot.Visible = true;
        asteroid.SetPosition(Vector2.Zero);
        shipRoot.SetPosition(Vector2.Zero);

        miningShip.UpdateShipInfo(100f, asteroid.radius + gameDefaults["planetSize"] + gameDefaults["shipDistanceFromSurface"]);
        miningShip.SetPosition(new Vector2(0f, -miningShip.shipDistanceFromCenter));

        SetRunningData();
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Visible == false)
            return;

        GeneratePower(delta);
    }
    private void SetRunningData()
    {
        foreach (string key in gameCore.gameData.resources.Keys)
        {
            EmitSignal(nameof(ResourcesUpdated), key, gameCore.gameData.resources[key].Values.First(), gameCore.gameData.resources[key].Values.Last());
        }
    }

    private void SetupBindings()
    {
        Main main = GetParent<Main>();
        asteroid.NewAstroidCreated += main.UpdateAsteroidPoints;

        miningShip.ShipCollectedCredits += UpdateCredits;
    }
    private void UpdateCredits(double credits)
    {
        double current = gameCore.gameData.resources["Credits"].Values.First();
        double max = gameCore.gameData.resources["Credits"].Values.Last();
        EmitSignal(nameof(ResourcesUpdated), "Credits", Mathf.Min(current += credits, max), max);
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
        EmitSignal(nameof(ResourcesUpdated), "Power", Mathf.Min(current += power, max), max);
    }
    private void SetupAsteroid()
    {
        if (gameCore.gameData.AsteroidPoints != null && gameCore.gameData.AsteroidPoints.Length > 0)
        {
            asteroid.SetupAsteroidShape(gameCore.gameData.AsteroidPoints);
        }
        else
            asteroid.SetupAsteroidShape(gameDefaults["planetSize"]);
    }
    private void SetCameraZoom()
    {
        Vector2 gameSpace = new Vector2(gameDefaults["planetSize"] + gameDefaults["shipDistanceFromSurface"] + 100, gameDefaults["planetSize"] + gameDefaults["shipDistanceFromSurface"] + 100);
        camera2D.Zoom = Vector2.One / (gameSpace / 300);
        GD.Print("Camera Zoom: " + camera2D.Zoom);
    }
}
