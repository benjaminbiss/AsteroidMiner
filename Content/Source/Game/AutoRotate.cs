using Godot;

public partial class AutoRotate : Node2D
{
    public float rotationSpeed { get; set; }
    public float rotationRadius { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        if (rotationRadius <= 0)
            return;
        Rotation += GetRotationDegrees(rotationSpeed, rotationRadius) * (float)delta;
    }
    private float GetRotationDegrees(float distanceTraveled, float radius)
    {
        return (distanceTraveled / (2 * Mathf.Pi * radius));
    }

}
