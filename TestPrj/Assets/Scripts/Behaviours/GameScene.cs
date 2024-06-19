using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class GameScene : MonoBehaviour
    {
        public Transform StaticBallRoot;
        public Transform ActiveBallRoot;
        public GameBall[] BallModels;
        public PlayerRoot PlayerRoot;

        readonly Dictionary<Ball, GameBall> balls = new();

        GameBall CreateBall(Ball ball)
        {
            var model = BallModels[ball.Color];
            var ballObj = Instantiate(model);
            ballObj.Ball = ball;
            ballObj.transform.SetParent(StaticBallRoot);
            ballObj.gameObject.SetActive(true);
            balls[ball] = ballObj;

            return ballObj;
        }

        void DeleteBall(Ball ball)
        {
            var beBall = balls[ball];
            balls.Remove(ball);
            Destroy(beBall.gameObject);
        }

        public void BuildScene(Game game)
        {
            var scene = game.GetComponent<Scene>();
            scene.OnBallCreated += ball => CreateBall(ball);
            scene.OnBallDeleted += ball => DeleteBall(ball);

            var inputManager = game.GetComponent<IInputManager>();

            var playerMgr = game.GetComponent<IPlayerManager>();
            playerMgr.OnPlayerAdded((player, ball) =>
            {
                var beBall = CreateBall(ball);
                balls[ball] = beBall;

                var indicator = PlayerRoot.AddPlayerOpIndicator(player.Id, ball.Color, beBall);
                indicator.OnShot += v => inputManager.ShotBall(v * 10);
            });
        }
    }
}
