using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Asteroid : Node2D
{
    [Signal]
    public delegate void NewAstroidCreatedEventHandler(Array<int> points);

    [Export]
    private NodePath polygonShapePath;
    private PolygonShape polygonShape;

    [Export]
    public float radius { get; set; } = 100f;

    private bool isDirty = false;
    private Vector2[] newPolygon;

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
    public void SetupAsteroidShape(Array<int> points)
    {
        Vector2[] polygon = new Vector2[points.Count / 2];
        for (int i = 0; i < points.Count / 2; i++)
        {
            polygon[i] = new Vector2(points[i * 2], points[i * 2 + 1]);
        }
        polygonShape.Polygon = polygon;
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (isDirty)
        {
            isDirty = false;
            polygonShape.Polygon = newPolygon;            
        }
    }
    private void UpdateAsteroidShape(Array<int> points)
    {
        Vector2[] polygon = new Vector2[points.Count / 2];
        for (int i = 0; i < points.Count / 2; i++)
        {
            polygon[i] = new Vector2(points[i * 2], points[i * 2 + 1]);
        }
        polygonShape.Polygon = polygon;
    }
    public void DealDamage(int index, int radius, double amount)
    {
        newPolygon = polygonShape.MovePointTowardsCenter(index, (float)amount);
        isDirty = true;
    }
    public int GetIndexBelowPosition(Vector2 position)
    {
        return polygonShape.FindNearestAnglePoint(position);
    }
    public Vector2 GetPointAtIndex(int index)
    {
        return polygonShape.Polygon[index];
    }
}
