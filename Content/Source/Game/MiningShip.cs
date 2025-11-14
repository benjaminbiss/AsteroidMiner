using Godot;

public partial class MiningShip : Node2D
{
    [Signal]
    public delegate void ShipCollectedCreditsEventHandler(double credits, double max);

    [Export]
    private NodePath shipSpritePath;
    private Sprite2D shipSprite;
    private AutoRotate shipRoot;

    public float shipSpeed { get; private set; }
    public float shipDistanceFromCenter { get; private set; }

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("MiningShip | Initialization failed.");
            return;
        }
    }
    private bool Initialize()
    {
        shipSprite = GetNodeOrNull<Sprite2D>(shipSpritePath);
        if (shipSprite == null)
            return false;
        shipRoot = GetParent<AutoRotate>();
        if (shipRoot == null)
            return false;

        return true;
    }

    public void UpdateShipInfo(float speed, float distance)
    {
        shipSpeed = speed;
        shipDistanceFromCenter = distance;

        shipRoot.rotationSpeed = shipSpeed;
        shipRoot.rotationRadius = shipDistanceFromCenter;
    }

    private void AddCredits(double credits)
    {
        EmitSignal(nameof(ShipCollectedCredits), credits, 0);
    }
}
