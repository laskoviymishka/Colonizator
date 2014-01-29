using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameLogic.Market;
using GameLogic.Game.Cards;

namespace GameLogic.Game
{
    public class Player
    {
        public Player()
        {
            Resources = new ObservableCollection<Resource>();
            Orders = new ObservableCollection<Order>();
            Cards = new List<ICard>();
        }

        public int FreeRoadCount { get; set; }
        public int KnightCount { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int PlayerScore { get; set; }
        public Color Color { get; set; }
        public List<ICard> Cards { get; set; }
        public ObservableCollection<Resource> Resources { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}