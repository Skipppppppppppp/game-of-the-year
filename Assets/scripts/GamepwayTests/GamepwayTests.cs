    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Security.Cryptography.X509Certificates;

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
            Assert.AreEqual(-1, u.b);
            Assert.AreEqual(1, u.c);
        }
        [Test]
        public void LineMadeCorrectly_OffAxes()
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
            Assert.AreEqual(-5, u.a);
            Assert.AreEqual(2, u.b);
            Assert.AreEqual(10, u.c);
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
    }
}