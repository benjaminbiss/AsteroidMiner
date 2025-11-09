using Godot;

public partial class AutoRotate : Node2D
{
    [Export]
    private float rotationSpeedDegrees = 10f;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Rotation += Mathf.DegToRad(rotationSpeedDegrees) * (float)delta;
    }
}
