using Godot;

public partial class Asteroid : Node2D
{
    [Signal]
    public delegate void NewAstroidCreatedEventHandler(int[] points);

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

        EmitSignal(nameof(NewAstroidCreated), polygonShape.GetAsteroidPoints());
    }
    public void SetupAsteroidShape(int[] points)
    {
        Vector2[] polygon = new Vector2[points.Length / 2];
        for (int i = 0; i < points.Length / 2; i++)
        {
            polygon[i] = new Vector2(points[i * 2], points[i * 2 + 1]);
        }
        polygonShape.Polygon = polygon;
    }
}
