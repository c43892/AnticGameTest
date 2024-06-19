using RVO;
using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AnticGameTest
{
    public class Game : IFrameDriven
    {
        private readonly ComponentContainer cc = new();

        public Fix64 LogicFrameTimeInterval { get; set; } = 0.01f; // in seconds

        // to accumulate the elapsed time
        private Fix64 timeElapsed = 0;

        public void OnElapsed(Fix64 te)
        {
            timeElapsed += te;

            // try driving all frame driven components with a fixed interval
            while (timeElapsed >= LogicFrameTimeInterval)
            {
                cc.Foreach(com =>
                {
                    if (com is IFrameDriven)
                        (com as IFrameDriven).OnElapsed(te);
                });

                timeElapsed -= LogicFrameTimeInterval;
            }
        }

        public void Build(int randomSeed, Fix64 minX, Fix64 minY, Fix64 maxX, Fix64 maxY)
        {
            // QuadTree will be shared in different components
            var quadTree = new QuadTree<Ball>(minX, minY, maxX, maxY);

            // ball spawner
            var staticBallSpawner = new BallSpawner(randomSeed, quadTree);
            cc.Add(staticBallSpawner);

            // player manager
            var playerManager = new PlayerManager();
            cc.Add(playerManager);

            // scene
            var scene = new Scene(quadTree, staticBallSpawner, playerManager);
            cc.Add(scene);

            // input manager
            var inputManager = new InputManager(scene, playerManager);
            cc.Add(inputManager);
            playerManager.OnPlayerAdded((player, ball) => inputManager.Players.Add(player));

            // score manager
            var scoreManager = new ScoreManager(-100, 100);
            cc.Add(scoreManager);
            playerManager.OnPlayerAdded((player, ball) => scoreManager.SetScore(player.Id, 0));
            scene.OnMovingBallCollided += (movingBall, targetBall) =>
            {
                if (movingBall.Color == targetBall.Color)
                {
                    scoreManager.Score(inputManager.CurrentPlayer.Id, 1);
                    scene.DelBall(targetBall);
                }
                else
                {
                    scoreManager.Score(inputManager.CurrentPlayer.Id, -1);
                    scene.DelBall(targetBall);
                }
            };
        }

        public void Reset(int ballCount, int[] colors)
        {
            var scene = cc.Get<Scene>();
            scene.Clear();

            var scoreManager = cc.Get<IScoreManager>();
            scoreManager.Clear();

            // initialize the game status
            scene.SpawnStaticBalls(ballCount, colors, 0.5f, 1);
            scene.AddPlayer("p1", colors[0], 1.5f);
        }

        public T GetComponent<T>() where T : class => cc.Get<T>();
    }
}
