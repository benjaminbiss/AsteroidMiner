using Godot;

public partial class Asteroid : Node2D
{
    [Export]
    private NodePath polygonShapePath;
    private PolygonShape polygonShape;

    [Export]
    public float radius { get; set; } = 100f;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("Asteroid | Initialization failed.");
            return;
        }        
    }
    private bool Initialize()
    {
        polygonShape = GetNodeOrNull<PolygonShape>(polygonShapePath);
        if (polygonShape == null)
            return false;
        
        return true;
    }

    public void SetupAsteroidShape(float newRadius)
    {
        radius = newRadius;
        polygonShape.DrawAsteroid(radius);
    }
}
