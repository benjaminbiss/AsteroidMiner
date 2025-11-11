using Godot;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
    [Signal]
    public delegate void ResourcesUpdatedEventHandler(string resource, float current, float max);

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

    private Main main;
    public RunningData runningData { get; set; }
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

        LoadGameData();
        SetupBindings();

        asteroid.Visible = false;
        shipRoot.Visible = false;
    }
    private bool Initialize()
    {
        main = GetParent<Main>();
        if (main == null)
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
    private void LoadGameData()
    {
        assetsDictionary = DataUtil.Instance.GetDefaultAssets();
        gameDefaults = DataUtil.Instance.GetGameDefaults();
        researchDictionary = DataUtil.Instance.GetDefaultResearch();
        resourcesDictionary = DataUtil.Instance.GetDefaultResources();
        upgradesDictionary = DataUtil.Instance.GetDefaultUpgrades();
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

        runningData = new RunningData();
        runningData.credits_CurrentAmount = (int)FetchGameData().OwnedResources.GetValueOrDefault("Credits", 0f);
        runningData.credits_MaxAmount = resourcesDictionary["Credits"].BaseMaxAmount;
        EmitSignal(nameof(ResourcesUpdated), "Credits", runningData.credits_CurrentAmount, runningData.credits_MaxAmount);
        runningData.power_CurrentAmount = FetchGameData().OwnedResources.GetValueOrDefault("Power", 0f);
        runningData.power_MaxAmount = resourcesDictionary["Power"].BaseMaxAmount;
        EmitSignal(nameof(ResourcesUpdated), "Power", runningData.power_CurrentAmount, runningData.power_MaxAmount);
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Visible == false)
            return;

        GeneratePower(delta);
    }
    public GameData FetchGameData()
    {
        return main.gameData;
    }
    private void SetupBindings()
    {
        miningShip.ShipCollectedCredits += UpdateCredits;
        asteroid.NewAstroidCreated += main.UpdateAsteroidPoints;
    }
    private void UpdateCredits(int credits)
    {
        Mathf.Min(runningData.credits_CurrentAmount += credits, runningData.credits_MaxAmount);
        EmitSignal(nameof(ResourcesUpdated), "Credits", runningData.credits_CurrentAmount, runningData.credits_MaxAmount);
    }
    private void GeneratePower(double delta)
    {
        if (FetchGameData().OwnedResources.ContainsKey("Power") == false)
            return;

        float generationRate = runningData.power_AdditiveGenerationRate * runningData.power_MultiplicativeGenerationRate;
        UpdatePower(generationRate * (float)delta);
    }
    private void UpdatePower(float power)
    {
        Mathf.Min(runningData.power_CurrentAmount += power, runningData.power_MaxAmount);
        EmitSignal(nameof(ResourcesUpdated), "Power", runningData.power_CurrentAmount, runningData.power_MaxAmount);
    }
    private void SetupAsteroid()
    {
        if (FetchGameData().AsteroidPoints != null && FetchGameData().AsteroidPoints.Length > 0)
        {
            asteroid.SetupAsteroidShape(FetchGameData().AsteroidPoints);
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
