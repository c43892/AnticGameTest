using Swift.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnticGameTest
{
    public interface IPlayerManager
    {
        public void OnPlayerAdded(Action<PlayerInfo, PlayerBall> handler);
        public void AddPlayer(string id, PlayerBall ball);
        public PlayerInfo GetPlayer(string id);
        public PlayerBall GetPlayerBall(string id);
        public void Clear();
    }

    public class PlayerManager : Component, IPlayerManager
    {
        public override string Name { get => "PlayerManager"; }

        readonly Dictionary<string, PlayerInfo> players = new();
        readonly Dictionary<string, PlayerBall> balls = new();
        readonly List<Action<PlayerInfo, PlayerBall>> onPlayerAddedHandlers = new();

        public void AddPlayer(string id, PlayerBall ball)
        {
            if (players.ContainsKey(id) || balls.ContainsKey(id))
            {
                throw new Exception($"player: {id} already exists");
            }

            var player = new PlayerInfo(id);

            players[id] = player;
            balls[id] = ball;

            onPlayerAddedHandlers.ForEach(h => h?.Invoke(player, ball));
        }

        public void Clear()
        {
            players.Clear();
            balls.Clear();
        }

        public PlayerInfo GetPlayer(string id) => players[id];

        public PlayerBall GetPlayerBall(string id) => balls[id];

        public void OnPlayerAdded(Action<PlayerInfo, PlayerBall> handler)
        {
            onPlayerAddedHandlers.Add(handler);
        }
    }
}
