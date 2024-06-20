using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class AIRandomShot : IAIStrategy
    {
        public readonly Fix64 VMin;
        public readonly Fix64 VMax;

        readonly SRandom rand;

        public AIRandomShot(SRandom random, Fix64 vMin, Fix64 vMax)
        {
            VMin = vMin;
            VMax = vMax;
            rand = random;
        }

        public Vec2 MakeDecision()
        {
            var dir = rand.NextDouble() * MathEx.Pi2;
            var v = rand.NextDouble() * (VMax - VMin) + VMin;
            var vx = Fix64.Cos(dir) * v;
            var vy = Fix64.Sin(dir) * v;

            return new(vx, vy);
        }
    }
}
