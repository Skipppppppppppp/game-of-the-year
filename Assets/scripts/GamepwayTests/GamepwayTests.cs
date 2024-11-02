using UnityEngine;
using NUnit.Framework;
// using System.Numerics; уди нахуй

namespace Assets.scripts.GamepwayTests
{
    public class GamepwayTests
    {
        [Test]
        public void FoundPointsCorrectly_yx()
        {
            // Arrange
            Break.Line line;
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
            var r = Break.ArePointsOnOneSide(x, line, z);

            // Assert
            Assert.AreEqual(false, r);
        }
        [Test]
        public void FoundPointsCorrectly_xx()
        {
            // Arrange
            Break.Line line;
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
            var r = Break.ArePointsOnOneSide(x, line, z);

            // Assert
            Assert.AreEqual(true, r);
        }
        [Test]
        public void FoundIfPointsAreOnOneSide_yy()
        {
            // Arrange
            Break.Line line;
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
            var r = Break.ArePointsOnOneSide(x, line, z);

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
            Break.Line u = Break.EquidistantLineBetweenTwoPoints(x,y);

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
            Break.Line u = Break.EquidistantLineBetweenTwoPoints(x,y);

            // Assert
            Assert.AreEqual(-2, u.a);
            Assert.AreEqual(-5, u.b);
            Assert.AreEqual(-10.5, u.c);
        }
        [Test]

        public void FoundLinesIntersectionCorrectly_NotOnOrigin()
        {
            // Arrange
            Break.Line line1;
            line1.a = -1;
            line1.b = 2;
            line1.c = 0;

            Break.Line line2;
            line2.a = 1;
            line2.b = 1;
            line2.c = 3;

            void DoThings(Break.Line line1, Break.Line line2)
            {
                Vector2 u = Break.FindIntersectionFromLines(line1,line2);
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

            var lines = new Break.Line[5];
            lines[0] = new Break.Line();
            lines[1] = Break.FindLineThroughTwoPoints(points[0],points[1]);
            lines[2] = Break.FindLineThroughTwoPoints(points[1],points[2]);
            lines[3] = Break.FindLineThroughTwoPoints(points[0],points[2]);
            lines[4] = Break.FindLineThroughTwoPoints(points[3],points[4]);

            // Act
            var u = Break.GetAreaVertices(centerPoint,lines,0);

            // Assert
            for (int i = 0; i < u.Count; i++)
            {
                Vector2 x = u[i];
                Debug.Log($"({x})");
            }
        }
    }
}