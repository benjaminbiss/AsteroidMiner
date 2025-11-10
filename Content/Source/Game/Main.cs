using Godot;

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

    private DataUtil dataUtilInstance;
    private GameData gameData;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("Main | Initialization failed.");
            return;
        }

        LoadGameData();
        SetCameraZoom();

        menuManager.GameStarted += LaunchGame;
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
        if (gameData == null)
        {
            GD.PrintErr("Main | Failed to load game data.");
            return;
        }
    }

    private void LaunchGame()
    {
        asteroid.Visible = true;
        shipRoot.Visible = true;
        asteroid.SetPosition(Vector2.Zero);
        shipRoot.SetPosition(Vector2.Zero);

        asteroid.SetupAsteroidShape(planetSize);
        miningShip.UpdateShipInfo(100f, asteroid.radius + planetSize + shipDistanceFromSurface);
        miningShip.SetPosition(new Vector2(0f, -miningShip.shipDistanceFromCenter));
    }

    private void SetCameraZoom()
    {
        Vector2 gameSpace = new Vector2(planetSize + shipDistanceFromSurface + 100, planetSize + shipDistanceFromSurface + 100);
        camera2D.Zoom =  Vector2.One / (gameSpace / 300);
        GD.Print("Camera Zoom: " + camera2D.Zoom);
    }
}