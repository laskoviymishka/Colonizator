using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic.Game.Cards
{
    public interface ICard
    {
        int Id { get; set; }
        int CardIndex { get; set; }
        bool IsPlayed { get; set; }
        string CardDescription { get; set; }
        void PlayCard();
        Player Owner { get; set; }
    }
}
