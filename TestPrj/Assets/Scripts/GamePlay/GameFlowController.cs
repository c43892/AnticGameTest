using Swift;
using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class GameFlowController : Component
    {
        public override string Name { get => "GameFlowController"; }
        public readonly Scene Scene = null;
        public readonly IInputManager InputManager;
        public readonly IAIManager AiManager;

        public GameFlowController(Scene scene, IInputManager inputManager, IAIManager aiManager)
        {
            Scene = scene;
            InputManager = inputManager;
            AiManager = aiManager;
        }

        public void Build()
        {
            Scene.OnMovingBallStopped += _ =>
            {
                InputManager.Move2NextPlayer();

                var pid = InputManager.CurrentPlayer.Id;
                var ai = AiManager.GetAi(pid);
                if (ai != null)
                {
                    var v = ai.MakeDecision();
                    InputManager.ShotBall(v);
                }
            };
        }

        public void StartFlow()
        {
            InputManager.CurrentPlayerIndex = 0;
        }
    }
}
