using System.Collections.Generic;
using GameLogic.Game;

namespace GameLogic.Search
{
    public class SearchGameQueue
    {
        public event UpdateGameQueue UpdateGameQueue;
        public List<Player> Players;

        public SearchGameQueue()
        {
            Players = new List<Player>();
        }

        public void SearchGame(Player player)
        {
            Players.Add(player);
            var eventArgs = new UpdateGameQueueArgs();
            if (Players.Count == 3)
            {
                eventArgs.Players = new List<Player>();
                Players = new List<Player>();
            }
            else
            {
                eventArgs.Players = Players;
            }
            UpdateGameQueue(this, eventArgs);
        }
    }

    public delegate void UpdateGameQueue(object sender, UpdateGameQueueArgs args);
}