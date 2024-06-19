using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class PlayerBall : Ball
    {
        public string PlayerId { get; private set; }

        public PlayerBall(string playerId, Vec2 pos, Fix64 size, int color)
            : base(pos, size, color)
        {
            PlayerId = playerId;
        }
    }
}
