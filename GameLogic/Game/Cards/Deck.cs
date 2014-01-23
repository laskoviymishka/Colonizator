using System;
using System.Collections.Generic;

namespace GameLogic.Game.Cards
{
    public class Deck
    {
        private readonly List<ICard> _deck;
        private readonly Random r = new Random();
        private readonly Game _game;
        private int _currentIndex;

        public Deck(Game game)
        {
            _game = game;
            _deck = new List<ICard>();
            Initialize();
            _currentIndex = _deck.Count - 1;
        }

        public IEnumerable<ICard> Cards
        {
            get { return _deck; }
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
            _deck.Add(new Monopoly(_game, 1));
            _deck.Add(new Monopoly(_game, 2));
            _deck.Add(new FreeResource(_game, 3));
            _deck.Add(new FreeResource(_game, 4));
            _deck.Add(new Road(_game, 5));
            _deck.Add(new Road(_game, 6));
            _deck.Add(new WinPoint(_game, 7, 1, "Костел"));
            _deck.Add(new WinPoint(_game, 8, 2, "Университет"));
            _deck.Add(new WinPoint(_game, 9, 3, "рынок"));
            _deck.Add(new WinPoint(_game, 10, 4, "Библиотека"));
            _deck.Add(new WinPoint(_game, 11, 5, "Мэрия"));
            _deck.Add(new Knight(_game, 12));
            _deck.Add(new Knight(_game, 13));
            _deck.Add(new Knight(_game, 14));
            _deck.Add(new Knight(_game, 15));
            _deck.Add(new Knight(_game, 16));
            _deck.Add(new Knight(_game, 17));
            _deck.Add(new Knight(_game, 18));
            _deck.Add(new Knight(_game, 19));
            _deck.Add(new Knight(_game, 20));
            _deck.Add(new Knight(_game, 21));
            _deck.Add(new Knight(_game, 22));
            _deck.Add(new Knight(_game, 23));
            _deck.Add(new Knight(_game, 24));
            _deck.Add(new Knight(_game, 25));
            Shuffle();
        }
    }
}
