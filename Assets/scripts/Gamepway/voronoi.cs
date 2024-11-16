using System.Collections.Generic;
using UnityEngine;

public struct Line
{
    public float a;
    public float b;
    public float c;
}
public static class Voronoi
{
    public static Line FindLineThroughTwoPoints(Vector2 A, Vector2 B)
    {
        Line line;
        line.a = B.y - A.y;
        line.b = A.x - B.x;
        line.c = line.a * A.x + line.b * A.y;
        return line; 
    }
    public static Line EquidistantLineBetweenTwoPoints(Vector2 A, Vector2 B)
    {
        Vector2 D = B - A;
        Vector2 O = 0.5f * D + A; 
        Line line;
        line.a = D.x;
        line.b = D.y;
        line.c = line.a * O.x + line.b * O.y;
        return line;
    }

    public static Vector2 FindIntersectionFromLines(Line line1, Line line2)
    {
        Vector2 intersection;
        intersection.x = -(line1.b * line2.c - line2.b * line1.c) / (line1.a * line2.b - line2.a * line1.b);
        intersection.y = -(line1.c * line2.a - line2.c * line1.a) / (line1.a * line2.b - line2.a * line1.b);
        return intersection;
    }

    public static bool ArePointsOnOneSide(Vector2 pointA, Line line, Vector2 pointB)
    {
        float FindPerpProjection(Vector2 point)
        {
            return line.a * point.x + line.b * point.y - line.c;
        }

        float projectionToPoint = FindPerpProjection(pointA);
        float projectionToIntersection = FindPerpProjection(pointB);
        if (projectionToIntersection * projectionToPoint < 0)
        {
            return false;
        }
        return true;
    }
    public static List<Vector2> GetAreaVertices(Vector2 point, Line[] lines, int ignoredLineIndex)
    {
        var intersections = new List<Vector2>();
        for (int line1Index = 0; line1Index < lines.Length; line1Index++)
        {
            if (ignoredLineIndex == line1Index)
            {
                continue;
            }

            for (int line2Index = 0; line2Index < lines.Length; line2Index++)
            {
                if (line2Index == line1Index || line2Index == ignoredLineIndex)
                {
                    continue;
                }

                Line line1 = lines[line1Index];
                Line line2 = lines[line2Index];
                Vector2 intersection = FindIntersectionFromLines(line1, line2);

                bool IsAllBeforeLine()
                {
                    for (int otherLineIndex = 0; otherLineIndex < lines.Length; otherLineIndex++)
                    {
                        if (otherLineIndex == line1Index || otherLineIndex == line2Index || otherLineIndex == ignoredLineIndex)
                        {
                            continue;
                        }
                        Line otherLine = lines[otherLineIndex];
                        bool m = ArePointsOnOneSide(point,otherLine,intersection);
                        if (m == false)
                        {
                            return m;
                        }
                    }
                    return true;
                }

                if (IsAllBeforeLine())
                {
                    intersections.Add(intersection); 
                }
            }
        }
        return intersections;
    }

    public static Line[] GetBoundsLines(Bounds bounds)
    {
        var ret = new Line[4];
        for (int k = 0; k < 4; k++) // adding lines for mesh's walls so intersections are within mesh bounds
        {
            var size = bounds.size;
            Vector2 offset = bounds.center;
            Vector2 halfsize = size / 2;
            Line line;
            switch (k)
            {
                case 0: // left
                {
                    line.a = 1;
                    line.b = 0;
                    line.c = offset.x - halfsize.x;
                    break;
                }
                case 1: // top
                {
                    line.a = 0;
                    line.b = 1;
                    line.c = offset.y - halfsize.y;
                    break;
                }
                case 2: // right
                {
                    line.a = 1;
                    line.b = 0;
                    line.c = offset.x + halfsize.x;
                    break;
                }
                case 3: // bottom
                {
                    line.a = 0;
                    line.b = 1;
                    line.c = offset.y + halfsize.y;
                    break;
                }
                default:
                {
                    System.Diagnostics.Debug.Fail("Unreachable");
                    return ret;
                }
            }
            ret[k] = line;
        }
        return ret;
    }
}