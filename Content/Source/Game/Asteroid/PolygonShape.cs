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

    public int[] GetAsteroidPoints()
    {
        int[] points = new int[Polygon.Length * 2];
        for (int i = 0; i < Polygon.Length; i++)
        {
            points[i * 2] = (int)Polygon[i].X;
            points[i * 2 + 1] = (int)Polygon[i].Y;
        }
        return points;
    }
    public int FindNearestAnglePoint(Vector2 position)
    {
        float shipAngle = Mathf.Atan2(position.Y, position.X);
        if (shipAngle < 0)
            shipAngle += Mathf.Tau;

        int nearestIndex = Mathf.RoundToInt(shipAngle / Mathf.Tau * PointCount) % PointCount;
        return nearestIndex;
    }
    public Vector2[] MovePointTowardsCenter(int index, float distance)
    {
        var newPolygon = (Vector2[])Polygon.Clone();
        float angle = (float)index / PointCount * Mathf.Tau; // Tau = 2π
        Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (Radius - distance);
        newPolygon[index] = point;
        return newPolygon;
    }
}