using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class PlayerInfo
    {
        public string Id { get; private set; }

        public PlayerInfo(string id)
        {
            Id = id;
        }
    }
}
