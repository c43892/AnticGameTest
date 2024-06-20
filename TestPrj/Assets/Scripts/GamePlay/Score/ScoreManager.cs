using Swift.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnticGameTest
{
    public interface IScoreManager
    {
        void SetScore(string id, int score);

        void Score(string id, int dScore);

        int GetScore(string id);

        void OnScoreChanged(Action<string, int> handler);
    }

    public class ScoreManager : Component, IScoreManager, IClearable
{
        public override string Name { get => "ScoreManager"; }
        public int MinScore { get; private set; }
        public int MaxScore { get; private set; }

        public readonly Dictionary<string, int> PlayerScores = new();

        private readonly List<Action<string, int>> scoreChangedHandler = new();

        public ScoreManager(int minScore, int maxScore)
        {
            MinScore = minScore;
            MaxScore = maxScore;
        }

        public void SetScore(string id, int score)
        {
            if (score < MinScore) score = MinScore;
            if (score > MaxScore) score = MaxScore;

            PlayerScores[id] = score;

            scoreChangedHandler.ForEach(h => h?.Invoke(id, score));
        }

        public void OnScoreChanged(Action<string, int> handler)
        {
            scoreChangedHandler.Add(handler);
        }

        public void Score(string id, int dScore)
        {
            SetScore(id, PlayerScores[id] + dScore);
        }

        public void Clear()
        {
            PlayerScores.Clear();
        }

        public int GetScore(string id) => PlayerScores[id];
    }
}
