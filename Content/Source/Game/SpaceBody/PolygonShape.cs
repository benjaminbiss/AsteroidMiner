using Godot;
using System;

public partial class PolygonShape : Polygon2D
{
    [Export] public int PointCount = 32;
    private float Radius = 100f;

    public void DrawAsteroid(float radius)
    {
        Radius = radius;
        GenerateCirclePolygon(PointCount, Radius);
    }

    private void GenerateCirclePolygon(int points, float radius)
    {
        Vector2[] polygon = new Vector2[points];

        for (int i = 0; i < points; i++)
        {
            float angle = (float)i / points * Mathf.Tau; // Tau = 2π
            Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            polygon[i] = point;
        }

        Polygon = polygon;
    }
}
