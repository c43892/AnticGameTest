using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AnticGameTest
{
    public class ScorePanel : MonoBehaviour
    {
        public Text PlayerScoreText;

        readonly Dictionary<string, Text> scoreText = new();

        public void SetPlayerScore(string id, int score)
        {
            Text text;

            if (!scoreText.ContainsKey(id))
            {
                text = Instantiate(PlayerScoreText);
                scoreText[id] = text;
                text.transform.SetParent(transform);
            }
            else
            {
                text = scoreText[id];
            }

            text.text = $"{id} : {score}";
            text.gameObject.SetActive(true);
        }

        public void Clear()
        {
            foreach (var text in scoreText.Values)
                Destroy(text.gameObject);

            scoreText.Clear();
        }
    }
}
