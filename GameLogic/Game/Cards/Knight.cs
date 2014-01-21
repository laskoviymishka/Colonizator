using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Game.Cards
{
    public class Knight : ICard
    {
        private readonly Game _game;

        public Knight(Game game)
        {
            _game = game;
        }


        public int Id { get; set; }

        public int CardIndex { get; set; }

        public bool IsPlayed { get; set; }

        public string CardDescription { get; set; }

        public void PlayCard()
        {

        }

        public Player Owner { get; set; }
    }
}
