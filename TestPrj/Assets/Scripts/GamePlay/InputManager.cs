using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public interface IInputManager
    {
        void ShotBall(Vec2 v);
    }

    public class InputManager : Component, IInputManager
    {
        public override string Name { get => "InputManager"; }

        public Scene Scene = null;
        public IPlayerManager PlayerManager = null;

        public readonly List<PlayerInfo> Players = new();

        public int CurrentPlayerIndex { get; set; }
        public PlayerInfo CurrentPlayer { get => Players[CurrentPlayerIndex]; }

        public InputManager(Scene scene, IPlayerManager playerManager)
        {
            Scene = scene;
            PlayerManager = playerManager;
        }

        public void ShotBall(Vec2 v)
        {
            var p = Players[CurrentPlayerIndex];
            var ball = PlayerManager.GetPlayerBall(p.Id);
            Scene.ShotBall(ball, v);
        }

        public void Move2NextPlayer()
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
        }
    }
}
