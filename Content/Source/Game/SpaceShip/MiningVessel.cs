using Godot;

public partial class MiningVessel : Node2D
{
    [Signal]
    public delegate void DealDamageToAsteroidEventHandler(int index, int radius, double amount);

    [Export]
    private NodePath shipSpritePath;
    private Sprite2D shipSprite;
    private AutoRotate shipRoot;
    
    public Reactor reactor { get; private set; }
    public MiningLaser miningLaser { get; private set; }


    public float shipSpeed { get; private set; }
    public float shipDistanceFromCenter { get; private set; }

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("MiningShip | Initialization failed.");
            return;
        }

        SetupBindings();
    }
    private bool Initialize()
    {
        shipSprite = GetNodeOrNull<Sprite2D>(shipSpritePath);
        if (shipSprite == null)
            return false;
        shipRoot = GetParent<AutoRotate>();
        if (shipRoot == null)
            return false;
        reactor = GetNodeOrNull<Reactor>("Reactor");
        if (reactor == null)
            return false;
        miningLaser = GetNodeOrNull<MiningLaser>("MiningLaser");
        if (miningLaser == null)
            return false;

        return true;
    }
    private void SetupBindings()
    {
        // Reactor
        GameCore.Instance.AssetUpdated += reactor.assetSprite.Activate;
        // Mining Laser
        GameCore.Instance.AssetUpdated += miningLaser.assetSprite.Activate;
        miningLaser.LaserFired += DamageAsteroid;
    }

    public void UpdateShipInfo(float speed, float distance)
    {
        shipSpeed = speed;
        shipDistanceFromCenter = distance;

        shipRoot.rotationSpeed = shipSpeed;
        shipRoot.rotationRadius = shipDistanceFromCenter;
    }
    public void DamageAsteroid(int index, int radius, double amount)
    {
        EmitSignal(SignalName.DealDamageToAsteroid, index, radius, amount);
    }
}
