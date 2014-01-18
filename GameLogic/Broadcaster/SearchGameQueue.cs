using System.Collections.Generic;
using GameLogic.Game;

namespace GameLogic.Broadcaster
{
    public class SearchGameQueue
    {
        public event UpdateGameQueue UpdateGameQueue;
        public List<Player> Players; 

        public void SearchGame(Player player)
        {
            Players.Add(player);
            var eventArgs = new UpdateGameQueueArgs();
            if (Players.Count == 5)
            {
                eventArgs.Map = new Map {Id = "test"};
                foreach (var queuePlayer in Players)
                {
                    eventArgs.Map.Players.AddRange(Players);
                }
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