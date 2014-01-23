using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic.Game
{
    public class GameStateUpdateArgs
    {
        public int First { get; set; }
        public int Second { get; set; }
        public GameAction Action { get; set; }
    }
}
