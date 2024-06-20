using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class GameScene : MonoBehaviour
    {
        public Transform BallRoot;
        public GameBall[] BallModels;
        public PlayerRoot PlayerRoot;
    
        readonly Dictionary<Ball, GameBall> balls = new();

        GameBall CreateBall(Ball ball)
        {
            var model = BallModels[ball.Color];
            var ballObj = Instantiate(model);
            ballObj.Ball = ball;
            ballObj.transform.SetParent(BallRoot);
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

            inputManager.OnPlayerChanged(PlayerRoot.SetActiveIndicator);

            tree4Gizmos = game.Tree;
        }

        QuadTree<Ball> tree4Gizmos = null;
        private void OnDrawGizmos()
        {
            if (tree4Gizmos == null)
                return;

            Gizmos.color = UnityEngine.Color.red;

            DrawTreeGizmos(tree4Gizmos);
        }

        void DrawTreeGizmos(QuadTree<Ball> tree)
        {
            if (tree.Children != null)
            {
                var aabb = tree.AABB;

                Gizmos.DrawLine(new Vector3((float)aabb.MinX, (float)aabb.Centre.y, 0), new Vector3((float)aabb.MaxX, (float)aabb.Centre.y, 0));
                Gizmos.DrawLine(new Vector3((float)aabb.Centre.x, (float)aabb.MinY, 0), new Vector3((float)aabb.Centre.x, (float)aabb.MaxY, 0));

                foreach (var child in tree.Children)
                    DrawTreeGizmos(child);
            }
        }
    }
}
