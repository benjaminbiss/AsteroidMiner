using Godot;

public partial class Asteroid : Node2D
{
    [Export]
    private float radius = 300f;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("Asteroid | Initialization failed.");
            return;
        }

        SetLocation();
    }
    private bool Initialize()
    {
        return true;
    }
    private void SetLocation()
    {
        Position = Vector2.Zero;
    }
}
