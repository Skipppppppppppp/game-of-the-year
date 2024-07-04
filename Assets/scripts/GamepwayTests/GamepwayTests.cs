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
        public void FoundPointsCorrectly()
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
        public void FoundPointsCorrectly2()
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
        public void FoundPointsCorrectly3()
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
        public void LineMadeCorrectly()
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
            Assert.AreEqual(0, u.a);
            Assert.AreEqual(-1, u.b);
            Assert.AreEqual(1, u.c);
        }
    }
}