using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLogic.Game;

namespace GameLogic.Broadcaster
{
    public class UpdateGameQueueArgs
    {
        public IEnumerable<Player> Players { get; set; }
        public Map Map { get; set; }
    }
}
