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
        public void OnPlayerAdded(Action<PlayerInfo> handler);
        public PlayerInfo AddPlayer(string id);
        public PlayerInfo GetPlayer(string id);
        public PlayerInfo[] Players { get; }
        public void Clear();
    }

    public interface IAIManager
    {
        public AIPlayer MakePlayerAIDrive(PlayerInfo player, IAIStrategy strategy);
        public AIPlayer GetAi(string id);
        public AIPlayer[] Ais { get; }
    }

    public class PlayerManager : Component, IPlayerManager, IAIManager
    {
        public override string Name { get => "PlayerManager"; }

        readonly Dictionary<string, PlayerInfo> players = new();
        readonly List<Action<PlayerInfo>> onPlayerAddedHandlers = new();

        public PlayerInfo AddPlayer(string id)
        {
            if (players.ContainsKey(id))
            {
                throw new Exception($"player: {id} already exists");
            }

            var player = new PlayerInfo(id);

            players[id] = player;

            onPlayerAddedHandlers.ForEach(h => h?.Invoke(player));

            return player;
        }

        public PlayerInfo[] Players { get => players.Values.ToArray(); }

        public PlayerInfo GetPlayer(string id) => players.ContainsKey(id) ? players[id] : null;


        #region AI

        readonly Dictionary<string, AIPlayer> aiPlayers = new();

        public AIPlayer MakePlayerAIDrive(PlayerInfo player, IAIStrategy strategy)
        {
            AIPlayer aiPlayer = new(player, strategy);
            aiPlayers[player.Id] = aiPlayer;
            return aiPlayer;
        }

        public AIPlayer GetAi(string id) => aiPlayers.ContainsKey(id) ? aiPlayers[id] : null;

        public AIPlayer[] Ais { get => aiPlayers.Values.ToArray(); }

        #endregion

        public void Clear()
        {
            players.Clear();
            aiPlayers.Clear();
        }

        public void OnPlayerAdded(Action<PlayerInfo> handler)
        {
            onPlayerAddedHandlers.Add(handler);
        }
    }
}
