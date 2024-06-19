using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public interface IBallSpawner
    {
        public void OnBallSpawned(Action<Ball> handler);

        public PlayerBall SpawnPlayerBall(string id, Fix64 size, int color);

        public List<Ball> SpawnBalls(int maxCount, Fix64 minSize, Fix64 maxSize, int[] colors);
    }

    public class BallSpawner : Component, IBallSpawner
    {
        readonly SRandom rand;
        readonly QuadTree<Ball> tree;

        public override string Name { get => "StaticBallSpawner"; }

        private readonly List<Action<Ball>> onBallSpawnedHandlers = new();

        public BallSpawner(int randomSeed, QuadTree<Ball> quadTree)
        {
            rand = new SRandom(randomSeed);
            tree = quadTree;
        }

        public void OnBallSpawned(Action<Ball> handler)
        {
            onBallSpawnedHandlers.Add(handler);
        }

        public PlayerBall SpawnPlayerBall(string id, Fix64 size, int color)
        {
            var posX = (rand.Next(0, 100) + 1) * tree.WorldBound.HalfSize.x * 0.02f + tree.WorldBound.MinX;
            var posY = (rand.Next(0, 100) + 1) * tree.WorldBound.HalfSize.y * 0.02f + tree.WorldBound.MinY;

            var ball = new PlayerBall(id, new(posX, posY), size, color);

            onBallSpawnedHandlers.ForEach(h => h?.Invoke(ball));

            return ball;
        }

        public List<Ball> SpawnBalls(int maxCount, Fix64 minSize, Fix64 maxSize, int[] colors)
        {
            var balls = new List<Ball>();

            for (var i = 0; i < maxCount; i++)
            {
                var colorIndex = rand.Next(0, colors.Length);
                var color = colors[colorIndex];

                var size = (rand.Next(0, 10) + 1) * (maxSize - minSize) / 10 + minSize;
                var posX = (rand.Next(0, 100) + 1) * tree.WorldBound.HalfSize.x * 0.02f + tree.WorldBound.MinX;
                var posY = (rand.Next(0, 100) + 1) * tree.WorldBound.HalfSize.y * 0.02f + tree.WorldBound.MinY;
                var ball = new Ball(new(posX, posY), size, color);

                balls.Add(ball);

                onBallSpawnedHandlers.ForEach(h => h?.Invoke(ball));
            }

            return balls;
        }
    }
}
