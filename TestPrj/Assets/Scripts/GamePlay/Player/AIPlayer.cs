using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public interface IAIStrategy
    {
        public Vec2 MakeDecision();
    }

    public class AIPlayer
    {
        public PlayerInfo Player { get; private set; }
        public IAIStrategy Strategy { get; private set; }

        public AIPlayer(PlayerInfo player, IAIStrategy strategy)
        {
            Player = player;
            Strategy = strategy;
        }

        public Vec2 MakeDecision() => Strategy.MakeDecision();
    }
}
