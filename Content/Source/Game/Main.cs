using Godot;
using System.Collections.Generic;

public partial class Main : Node2D
{
    [Export]
    private NodePath menuManagerPath;
    private MenuManager menuManager;
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

    [Export]
    private float planetSize = 100f;
    [Export]
    private float shipDistanceFromSurface = 75f;

    private bool bIsGameLaunched = false;
    private float currentPlayTime = 0f;
    private float autoSaveInterval = 10f;

    private DataUtil dataUtilInstance;
    private GameData gameData;
    public Dictionary<string, AssetInfo> assetsDictionary { get; set; }
    public Dictionary<string, ResearchInfo> researchDictionary { get; set; }
    public Dictionary<string, ResourceInfo> resourcesDictionary { get; set; }
    public Dictionary<string, UpgradeInfo> upgradesDictionary { get; set; }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!bIsGameLaunched)
            return;

        currentPlayTime += (float)delta;
        if (currentPlayTime >= autoSaveInterval)
        {
            gameData.PlayTime += currentPlayTime;
            SaveGame();
            currentPlayTime = 0f;
        }
    }

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("Main | Initialization failed.");
            return;
        }

        LoadGameData();
        SetupGameMenu();
        SetCameraZoom();

        menuManager.GameStarted += LaunchGame;
        miningShip.ShipCollectedCredits += UpdateCredits;
        asteroid.NewAstroidCreated += UpdateAsteroidPoints;

        asteroid.Visible = false;
        shipRoot.Visible = false;
    }

    private bool Initialize()
    {
        dataUtilInstance = DataUtil.Instance;
        if (dataUtilInstance == null)
        {
            GD.PrintErr("GameMenu | Global Resources Singleton instance not found.");
            return false;
        }
        menuManager = GetNodeOrNull<MenuManager>(menuManagerPath);
        if (menuManager == null)
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
        gameData = dataUtilInstance.LoadGame();
        assetsDictionary = dataUtilInstance.GetDefaultAssets();
        researchDictionary = dataUtilInstance.GetDefaultResearch();
        resourcesDictionary = dataUtilInstance.GetDefaultResources();
        upgradesDictionary = dataUtilInstance.GetDefaultUpgrades();

        if (gameData == null)
        {
            GD.PrintErr("Main | Failed to load game data.");
            return;
        }
    }

    private void LaunchGame()
    {
        bIsGameLaunched = true;

        asteroid.Visible = true;
        shipRoot.Visible = true;
        asteroid.SetPosition(Vector2.Zero);
        shipRoot.SetPosition(Vector2.Zero);

        SetupAsteroid();

        miningShip.UpdateShipInfo(100f, asteroid.radius + planetSize + shipDistanceFromSurface);
        miningShip.SetPosition(new Vector2(0f, -miningShip.shipDistanceFromCenter));
    }

    private void SetCameraZoom()
    {
        Vector2 gameSpace = new Vector2(planetSize + shipDistanceFromSurface + 100, planetSize + shipDistanceFromSurface + 100);
        camera2D.Zoom = Vector2.One / (gameSpace / 300);
        GD.Print("Camera Zoom: " + camera2D.Zoom);
    }

    private void SaveGame()
    {
        dataUtilInstance.SaveGame(gameData);
    }

    private void UpdateAsteroidPoints(int[] points)
    {
        gameData.AsteroidPoints = points;
    }
    private void UpdateOwnedAssets(Dictionary<string, Dictionary<string, float>> ownedAssets)
    {
        gameData.OwnedAssets = ownedAssets;
    }
    private void UpdateOwnedResearch(string[] ownedResearch)
    {
        gameData.OwnedResearch = ownedResearch;
    }
    private void UpdateOwnedResources(Dictionary<string, int> ownedResources)
    {
        gameData.OwnedResources = ownedResources;
    }
    private void UpdateOwnedUpgrades(string[] ownedUpgrades)
    {
        gameData.OwnedUpgrades = ownedUpgrades;
    }

    private void UpdateCredits(int credits)
    {
        gameData.OwnedResources["Credits"] += credits;
    }

    private void SetupAsteroid()
    {
        if (gameData.AsteroidPoints != null && gameData.AsteroidPoints.Length > 0)
        {
            asteroid.SetupAsteroidShape(gameData.AsteroidPoints);
        }
        else
            asteroid.SetupAsteroidShape(planetSize);
    }

    private void SetupGameMenu()
    {
        GameMenu gameMenu = menuManager.GetNodeOrNull<GameMenu>("GameMenu");
        if (gameMenu == null)
        {
            GD.PrintErr("Main | GameMenu not found in MenuManager.");
            return;
        }
        gameMenu.SetResources(resourcesDictionary);
    }
}