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
    private float miningShipDistance = 100;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("Main | Initialization failed.");
            return;
        }

        SetCameraZoom();

        menuManager.GameStarted += LaunchGame;
        asteroid.Visible = false;
        shipRoot.Visible = false;
    }

    private bool Initialize()
    {
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

    private void LaunchGame()
    {
        asteroid.Visible = true;
        shipRoot.Visible = true;

        asteroid.SetPosition(Vector2.Zero);
        asteroid.SetupAsteroidShape(planetSize);
        shipRoot.SetPosition(Vector2.Zero);
        miningShip.SetPosition(new Vector2(0f, -(asteroid.radius + planetSize + miningShipDistance)));
    }

    private void SetCameraZoom()
    {
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        float aspectRatio = screenSize.X / screenSize.Y;
        Vector2 gameSpace = new Vector2(planetSize + miningShipDistance, (planetSize + miningShipDistance) * aspectRatio);
        camera2D.Zoom = gameSpace / screenSize;
        GD.Print("Camera Zoom: " + camera2D.Zoom);
    }
}