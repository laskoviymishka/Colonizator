﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameLogic.Market;
using GameLogic.Game.Cards;

namespace GameLogic.Game
{
    public class Player
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int PlayerScore { get; set; }
        public Color Color { get; set; }
        public List<ICard> Cards { get; set; }
        public ObservableCollection<Resource> Resources { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}