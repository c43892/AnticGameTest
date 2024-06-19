using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class GameDriver : MonoBehaviour
    {
        public Game Game { get; set; }

        // Update is called once per frame
        void Update()
        {
            var te = Time.deltaTime;

            if (Game != null)
                Game.OnElapsed(te);
        }
    }
}
