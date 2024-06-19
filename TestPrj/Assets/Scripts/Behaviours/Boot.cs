using RVO;
using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnticGameTest
{
    public class Boot : MonoBehaviour
    {
        public GameScene Scene;
        public UIRoot UI;
        public GameDriver GameDriver;

        Game game;

        // Start is called before the first frame update
        void Awake()
        {
            game = new();
            game.Build(0, -80, -45, 80, 45);
            GameDriver.Game = game;

            Scene.BuildScene(game);
            UI.BuildUI(game);
        }

        void Start()
        {
            var colors = Enumerable.Range(0, Scene.BallModels.Length).ToArray();
            game.Start(500, colors);
        }
    }
}
