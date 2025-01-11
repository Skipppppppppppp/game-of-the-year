using UnityEngine;
using NUnit.Framework;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine.Assertions.Must;
// using System.Numerics; уди нахуй

namespace Assets.scripts.GamepwayTests
{
    public class GamepwayTests
    {
        [Test]
        public void FoundPointsCorrectly_yx()
        {
            // Arrange
            Line line;
            line.a = -1;
            line.b = 1;
            line.c = 0;
            Vector2 x;
            x.x = 0;
            x.y = 10;
            Vector2 z;
            z.x = 10;
            z.y = 0;

            // Act
            var r = Voronoi.ArePointsOnOneSide(x, line, z);

            // Assert
            Assert.AreEqual(false, r);
        }
        [Test]
        public void FoundPointsCorrectly_xx()
        {
            // Arrange
            Line line;
            line.a = -1;
            line.b = 1;
            line.c = 0;
            Vector2 x;
            x.x = 20;
            x.y = 0;
            Vector2 z;
            z.x = 10;
            z.y = 0;

            // Act
            var r = Voronoi.ArePointsOnOneSide(x, line, z);

            // Assert
            Assert.AreEqual(true, r);
        }
        [Test]
        public void FoundIfPointsAreOnOneSide_yy()
        {
            // Arrange
            Line line;
            line.a = -1;
            line.b = 1;
            line.c = 0;
            Vector2 x;
            x.x = 0;
            x.y = 10;
            Vector2 z;
            z.x = 0;
            z.y = 10;

            // Act
            var r = Voronoi.ArePointsOnOneSide(x, line, z);

            // Assert
            Assert.AreEqual(true, r);
        }
        [Test]
        public void FoundIfPointsAreOnOneSide_fucker()
        {
            // Arrange
            Line line;
            line.a = 1;
            line.b = 9;
            line.c = 35;
            Vector2 x;
            x.x = -2;
            x.y = 3;
            Vector2 z;
            z.x = -2.66f;
            z.y = 5.66f;

            // Act
            var r = Voronoi.ArePointsOnOneSide(x, line, z);

            // Assert
            Assert.AreEqual(false, r);
        }

        [Test]public void FoundIfPointsAreOnOneSide_donker()
        {
            // Arrange
            Line line;
            line.a = -3;
            line.b = -3;
            line.c = -9;
            Vector2 x;
            x.x = -2;
            x.y = 3;
            Vector2 z;
            z.x = -10;
            z.y = 5;

            // Act
            var r = Voronoi.ArePointsOnOneSide(x, line, z);

            // Assert
            Assert.AreEqual(true, r);
        }

        [Test]
        public void LineEquationFoundCorrectly_OnAxes()
        {
            // Arrange
            Vector2 x;
            x.x = 0;
            x.y = -1;
            Vector2 y;
            y.x = 1;
            y.y = 0;
            
            // Act
            Line u = Voronoi.EquidistantLineBetweenTwoPoints(x,y);

            // Assert
            Assert.AreEqual(1, u.a);
            Assert.AreEqual(1, u.b);
            Assert.AreEqual(0, u.c);
        }
        [Test]
        public void LineequationFoundCorrectly_OffAxes()
        {
            // Arrange
            Vector2 x;
            x.x = 0;
            x.y = 5;
            Vector2 y;
            y.x = -2;
            y.y = 0;
            
            // Act
            Line u = Voronoi.EquidistantLineBetweenTwoPoints(x,y);

            // Assert
            Assert.AreEqual(-2, u.a);
            Assert.AreEqual(-5, u.b);
            Assert.AreEqual(-10.5, u.c);
        }
        [Test]

        public void FoundLinesIntersectionCorrectly_NotOnOrigin()
        {
            // Arrange
            Line line1;
            line1.a = -1;
            line1.b = 2;
            line1.c = 0;

            Line line2;
            line2.a = 1;
            line2.b = 1;
            line2.c = 3;

            void DoThings(Line line1, Line line2)
            {
                Vector2 u = Voronoi.FindIntersectionFromLines(line1,line2).Value;
                Assert.AreEqual(2, u.x);
                Assert.AreEqual(1, u.y);
            }
            
            DoThings(line1, line2);
            DoThings(line2,line1);
        }
        [Test]
        public void FoundAreaVerticesCorrectly()
        {
            // Arrange
            var points = new Vector2[5];
            points[0] = new Vector2 (-1,4);
            points[1] = new Vector2 (-10,5);
            points[2] = new Vector2 (2,1);
            points[3] = new Vector2 (-4,3);
            points[4] = new Vector2 (-3,5);

            Vector2 centerPoint = new Vector2 (-2, 3);

            var lines = new Line[5];
            lines[0] = new Line();
            lines[1] = Voronoi.FindLineThroughTwoPoints(points[0],points[1]);
            lines[2] = Voronoi.FindLineThroughTwoPoints(points[1],points[2]);
            lines[3] = Voronoi.FindLineThroughTwoPoints(points[0],points[2]);
            lines[4] = Voronoi.FindLineThroughTwoPoints(points[3],points[4]);

            // Act
            var u = Voronoi.GetAreaVertices(centerPoint,lines,0);

            // Assert
            AssertContains(u, new(x: -4, y: 3));
            AssertContains(u, new(x: -1, y: 4));
            AssertContains(u, new(x: -3.37f, y: 4.26f));
            AssertContains(u, new(x: 2, y: 1));
            Assert.AreEqual(u.Count, 2*4);
        }
        [Test]
        public void SkipsCorrectIndex()
        {
            // Arrange
            var points = new Vector2[5];
            points[0] = new Vector2 (-1,4);
            points[1] = new Vector2 (-10,5);
            points[2] = new Vector2 (2,1);
            points[3] = new Vector2 (-4,3);
            points[4] = new Vector2 (-3,5);

            Vector2 centerPoint = new Vector2 (-2, 3);

            var lines = new Line[5];
            lines[0] = Voronoi.FindLineThroughTwoPoints(points[0],points[1]);
            lines[1] = Voronoi.FindLineThroughTwoPoints(points[1],points[2]);
            lines[2] = new Line();
            lines[3] = Voronoi.FindLineThroughTwoPoints(points[0],points[2]);
            lines[4] = Voronoi.FindLineThroughTwoPoints(points[3],points[4]);

            // Act
            var u = Voronoi.GetAreaVertices(centerPoint,lines,2);

            // Assert
            AssertContains(u, new(x: -4, y: 3));
            AssertContains(u, new(x: -1, y: 4));
            AssertContains(u, new(x: -3.37f, y: 4.26f));
            AssertContains(u, new(x: 2, y: 1));
            Assert.AreEqual(u.Count, 2*4);
        }
        [Test]
        public void FoundAreaVerticesThatAreSameAsInputCorrectly()
        {
            // Arrange
            var points = new Vector2[3];
            points[0] = new Vector2 (-1.25f,-0.875f);
            points[1] = new Vector2 (-0.4f,0.4f);
            points[2] = new Vector2 (3,-3);

            Vector2 centerPoint = new Vector2 (0, -1);

            var lines = new Line[4];
            lines[0] = new Line();
            lines[1] = Voronoi.FindLineThroughTwoPoints(points[0],points[1]);
            lines[2] = Voronoi.FindLineThroughTwoPoints(points[1],points[2]);
            lines[3] = Voronoi.FindLineThroughTwoPoints(points[0],points[2]);

            // Act
            var u = Voronoi.GetAreaVertices(centerPoint,lines,0);

            // Assert
            
            for (int i = 0; i < points.Length; i++)
            {
                AssertContains(u, points[i]);
            }
        }
        [Test]
        public void FoundMeshBoundsLinesCorrectly()
        {
            var bounds = new Bounds(new Vector2(0, 1), new Vector2(6,8));
            var u = Voronoi.GetBoundsLines(bounds);
            AssertContains(u, new()
            {
                a = 0,
                b = 1,
                c = 5,
            }
            );
            
            AssertContains(u, new()
            {
                a = 0,
                b = 1,
                c = -3,
            }
            );
            AssertContains(u, new()
            {
                a = 1,
                b = 0,
                c = 3,
            }
            );
            AssertContains(u, new()
            {

                a = 1,
                b = 0,
                c = -3,
            }
            );
        }

        private static void AssertContains(List<Vector2> points, Vector2 point)
        {
            bool Contains()
            {
                const float epsilon = 0.01f;
                foreach (var p in points)
                {
                    if (Mathf.Abs(p.x - point.x) > epsilon)
                    {
                        continue;
                    }
                    if (Mathf.Abs(p.y - point.y) > epsilon)
                    {
                        continue;
                    }
                    return true;
                }
                return false;
            }
            Assert.IsTrue(Contains(), "{0} is not contained in the array", point);
        }
        private static void AssertContains(Line[] lines, Line line)
        {
            bool Contains()
            {
                const float epsilon = 0.01f;
                foreach (var p in lines)
                {
                    bool NotEqual(float num1, float num2)
                    {
                        return Mathf.Abs(num1 - num2) > epsilon;
                    }
                    if (NotEqual(p.a, line.a))
                    {
                        continue;
                    }
                    if (NotEqual(p.b, line.b))
                    {
                        continue;
                    }
                    if (NotEqual(p.c, line.c))
                    {
                        continue;
                    }
                    return true;
                }
                return false;
            }
            Assert.IsTrue(Contains(), "{0} is not contained in the array", line);
        }
    }
}