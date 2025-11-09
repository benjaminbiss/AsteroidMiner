using Godot;

public partial class MiningShip : Node2D
{
    [Export]
    private NodePath shipSpritePath;
    private Sprite2D shipSprite;

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

        return true;
    }
}
