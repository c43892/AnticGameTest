using RVO;
using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AnticGameTest
{
    public class Scene : Component, IFrameDriven
    {
        public override string Name { get => "Scene"; }

        public IBallSpawner BallSpawner;
        public IPlayerManager PlayerManager;

        public AABBInfo WorldBound { get => tree.AABB; }

        public event Action<Ball> OnBallCreated = null;
        public event Action<Ball> OnBallDeleted = null;
        public event Action<Ball, Vec2> OnMovingBallMoved = null;
        public event Action<Ball, Ball> OnMovingBallCollided = null;
        public event Action<PlayerBall> OnMovingBallStopped = null;

        private readonly QuadTree<Ball> tree = null;

        private PlayerBall movingBall = null;
        private Vec2 movingBallSpeed = Vec2.Zero;
        private Fix64 movingBallAccelaration = 500;

        public Scene(QuadTree<Ball> quadTree, IBallSpawner ballSpawner, IPlayerManager playerManager)
        {
            tree = quadTree;
            BallSpawner = ballSpawner;
            PlayerManager = playerManager;

            BallSpawner.OnBallSpawned(ball => OnBallCreated?.Invoke(ball));
        }

        public void Clear()
        {
            tree.Clear();
            PlayerManager.Clear();
        }

        public List<Ball> SpawnStaticBalls(int count, int[] colors, Fix64 minBallSize, Fix64 maxBallSize)
        {
            var balls = BallSpawner.SpawnBalls(count, minBallSize, maxBallSize, colors);
            tree.Add(balls);

            AdjustBallsPositionInSimulator(balls);
            tree.Update();

            return balls;
        }

        public PlayerBall AddPlayer(string id, int color, Fix64 ballSize, Vec2? atSpecificPosition = null)
        {
            var ball = BallSpawner.SpawnPlayerBall(id, ballSize, color);

            PlayerManager.AddPlayer(id, ball);
            if (atSpecificPosition.HasValue)
                ball.Pos = atSpecificPosition.Value;

            tree.Add(ball);

            AdjustBallsPositionInSimulator(tree.AllObjs.Select(aabb => aabb as Ball));
            tree.Update();

            return ball;
        }

        public void ShotBall(PlayerBall playerBall, Vec2 v)
        {
            movingBall = playerBall;
            movingBallSpeed = v;
        }

        public void DelBall(Ball ball)
        {
            tree.Remove(ball);
            OnBallDeleted?.Invoke(ball);
        }

        public void OnElapsed(Fix64 te)
        {
            if (movingBall == null || movingBallSpeed.Length < 0.1f)
                return;

            var fromPos = movingBall.Pos;

            movingBall.Pos += movingBallSpeed * te;

            var v = movingBallSpeed.Length;
            if (tree.InBound(movingBall.Pos))
            {
                v -= movingBallAccelaration * te;
                if (v < 0)
                    v = 0;
            }
            else
            {
                v = 0;
                movingBall.Pos = fromPos;
            }

            movingBallSpeed = movingBallSpeed * v / movingBallSpeed.Length;
            
            if (movingBallSpeed.Length < 0.1f)
                OnMovingBallStopped?.Invoke(movingBall);
            else
                OnMovingBallMoved?.Invoke(movingBall, fromPos);

            CheckMovingBallCollision();
        }

        void CheckMovingBallCollision()
        {
            var targetBalls = tree.CheckCollision(movingBall.AABB);
            foreach (Ball ball in targetBalls)
            {
                if (!(ball is PlayerBall) && (movingBall.Pos - ball.Pos).Length <= movingBall.Size + ball.Size)
                    OnMovingBallCollided?.Invoke(movingBall, ball as Ball);
            }
        }

        void AdjustBallsPositionInSimulator(IEnumerable<Ball> balls)
        {
            Simulator simulator = new();
            Dictionary<int, Ball> aid2Ball = new();

            // Simulator simulator = new();
            simulator.setAgentDefaults(1, 1, 1, 1, 1, Fix64.MaxValue, Vec2.Zero);

            // put all balls in and move one step forward the simulator. all balls' position will be recaculated if there is any collision between balls
            // Dictionary<int, Ball> aid2Ball = new();
            foreach (var ball in balls)
            {
                var aid = simulator.addAgent(ball.Pos, ball.Size, Fix64.MaxValue);
                aid2Ball[aid] = ball;

                simulator.setAgentPosition(aid, ball.Pos);
                simulator.setAgentPrefVelocity(aid, Vec2.Zero);
            }

            var simulationTimeStep = 0.1f;
            simulator.setTimeStep(simulationTimeStep);
            for (var i = 0; i < 10; i++)
            {
                // need to improve when the QuadTree is ready

                simulator.doStep();

                foreach (var aid in simulator.getAllAgents())
                {
                    var pos = simulator.getAgentPosition(aid);
                    var v = simulator.getAgentVelocity(aid);
                    simulator.setAgentPosition(aid, pos + v * simulationTimeStep);
                }
            }

            // retrieve the balls' position back
            foreach (var aid in simulator.getAllAgents())
            {
                var agent = simulator.getAgentPosition(aid);
                var ball = aid2Ball[aid];
                ball.Pos = new(agent.x, agent.y);
            }

            simulator.Clear();
        }
    }
}
