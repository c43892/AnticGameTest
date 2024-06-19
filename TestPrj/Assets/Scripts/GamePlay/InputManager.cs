using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public interface IInputManager
    {
        void ShotBall(Vec2 v);
        void Move2NextPlayer();
        void OnPlayerChanged(Action<string> handler);

        int CurrentPlayerIndex { get; set; }
        PlayerInfo CurrentPlayer { get; }
    }

    public class InputManager : Component, IInputManager
    {
        public override string Name { get => "InputManager"; }

        public Scene Scene = null;
        public IPlayerManager PlayerManager = null;

        public readonly List<PlayerInfo> Players = new();

        readonly List<Action<string>> onPlayerChangedHandlers = new();

        public int CurrentPlayerIndex
        {
            get => currentPlayerIndex;
            set
            {
                currentPlayerIndex = value;
                if (currentPlayerIndex < 0 || currentPlayerIndex >= Players.Count)
                    throw new Exception($"there is no {value}th player");

                onPlayerChangedHandlers.ForEach(h => h?.Invoke(CurrentPlayer.Id));
            }
        } int currentPlayerIndex;

        public PlayerInfo CurrentPlayer { get => Players[CurrentPlayerIndex]; }

        public InputManager(Scene scene, IPlayerManager playerManager)
        {
            Scene = scene;
            PlayerManager = playerManager;
        }

        public void OnPlayerChanged(Action<string> handler)
        {
            onPlayerChangedHandlers.Add(handler);
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
            onPlayerChangedHandlers.ForEach(h => h?.Invoke(CurrentPlayer.Id));
        }
    }
}
