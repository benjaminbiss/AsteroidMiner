using Godot;

public partial class Main : Node2D
{
    [Export]
    private float planetSize = 100f;
    [Export]
    private NodePath asteroidPath;
    private Asteroid asteroid;
    [Export]
    private NodePath shipRootPath;
    private AutoRotate shipRoot;
    private MiningShip miningShip;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("Main | Initialization failed.");
            return;
        }

        asteroid.SetPosition(Vector2.Zero);
        asteroid.SetupAsteroidShape(planetSize);
        shipRoot.SetPosition(Vector2.Zero);
        miningShip.SetPosition(new Vector2(0f, -(asteroid.radius + planetSize + 50f)));
    }

    private bool Initialize()
    {
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
}