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

        public SRandom Rand { get; private set; }

        public QuadTree<Ball> Tree { get; private set; } // expose it only because the gizmos need it. may need to find a better way

        // to accumulate the elapsed time
        private Fix64 timeElapsed = 0;

        public void OnElapsed(Fix64 te)
        {
            timeElapsed += te;

            // try driving all frame driven components with a fixed interval
            while (timeElapsed >= LogicFrameTimeInterval)
            {
                cc.Foreach<IFrameDriven>(com => com.OnElapsed(te));
                timeElapsed -= LogicFrameTimeInterval;
            }
        }

        public void Build(int randomSeed, Fix64 minX, Fix64 minY, Fix64 maxX, Fix64 maxY)
        {
            // Random & QuadTree will be shared in different components
            Rand = new SRandom(randomSeed);
            Tree = new(minX, minY, maxX, maxY);

            // ball spawner
            BallSpawner ballSpawner = new(Rand, Tree);
            cc.Add(ballSpawner);

            // player manager
            PlayerManager playerManager = new();
            cc.Add(playerManager);

            // scene
            Scene scene = new(Tree, ballSpawner, playerManager);
            cc.Add(scene);

            // input manager
            InputManager inputManager = new(scene, playerManager);
            cc.Add(inputManager);
            playerManager.OnPlayerAdded((player) => inputManager.Players.Add(player));

            // score manager
            ScoreManager scoreManager = new(-100, 100);
            cc.Add(scoreManager);
            playerManager.OnPlayerAdded((player) => scoreManager.SetScore(player.Id, 0));
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

            // game flow control
            GameFlowController gfc = new(scene, inputManager, playerManager);
            cc.Add(gfc);
            gfc.Build();
        }

        public void Start(int ballCount, int[] colors)
        {
            cc.Foreach<IClearable>(com => com.Clear());

            // initialize the game status
            var scene = cc.Get<Scene>();
            scene.SpawnStaticBalls(ballCount, colors, 0.5f, 1);

            // add players
            var playerManager = cc.Get<IPlayerManager>();
            var pb1 = scene.AddPlayerBall("p1", colors[0], 2, Vec2.Zero);
            var pb2 = scene.AddPlayerBall("p2", colors[1], 2, Vec2.Zero);
            var pb3 = scene.AddPlayerBall("p3", colors[2], 2, Vec2.Zero);
            var p1 = playerManager.AddPlayer(pb1.PlayerId);
            var p2 = playerManager.AddPlayer(pb2.PlayerId);
            var p3 = playerManager.AddPlayer(pb3.PlayerId);

            // make p2 an random-ai player, p3 a copy-ai player
            var inputManager = cc.Get<IInputManager>();
            var aiManager = cc.Get<IAIManager>();
            aiManager.MakePlayerAIDrive(p2, new AIRandomShot(Rand, 100, 300));
            aiManager.MakePlayerAIDrive(p3, new AICopyShot(inputManager, p1.Id));

            var gfc = cc.Get<GameFlowController>();
            gfc.StartFlow();
        }

        public T GetComponent<T>() where T : class => cc.Get<T>();
    }
}
