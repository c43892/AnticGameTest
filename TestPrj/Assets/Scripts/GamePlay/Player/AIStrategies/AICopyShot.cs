using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class AICopyShot : IAIStrategy
    {
        Vec2 copyV;

        public AICopyShot(IInputManager inputManager, string copyPlayerId)
        {
            inputManager.OnBallShot((p, b, v) =>
            {
                if (p.Id == copyPlayerId)
                    copyV = v;
            });
        }

        public Vec2 MakeDecision() => copyV;
    }
}
