using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class UIRoot : MonoBehaviour
    {
        public ScorePanel ScorePanel = null;

        public void BuildUI(Game game)
        {
            var scoreManager = game.GetComponent<IScoreManager>();
            scoreManager.OnScoreChanged(ScorePanel.SetPlayerScore);
        }
    }
}
