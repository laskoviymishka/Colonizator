using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic.Game
{
    public class Deck
    {
        private readonly List<ICard> _deck;
        private readonly Random r = new Random();
        private int _currentIndex;

        public Deck()
        {
            _deck = new List<ICard>();
            Initialize();
            _currentIndex = _deck.Count;
        }


        public ICard DrawCard(Player player)
        {
            if (_currentIndex < 0)
            {
                return null;
            }
            var card = _deck[_currentIndex];
            card.Owner = player;
            if (player.Cards == null) { player.Cards = new List<ICard>(); }

            player.Cards.Add(card);
            _currentIndex--;
            return card;
        }
        public void Shuffle()
        {
            for (int n = _deck.Count - 1; n > 0; --n)
            {
                int k = r.Next(n + 1);
                ICard temp = _deck[n];
                _deck[n] = _deck[k];
                _deck[k] = temp;
            }
        }

        private void Initialize()
        {

        }
    }
}
