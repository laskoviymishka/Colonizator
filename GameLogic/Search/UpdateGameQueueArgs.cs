using System.Collections.Generic;
using GameLogic.Game;

namespace GameLogic.Search
{
    public class UpdateGameQueueArgs
    {
        public IEnumerable<Player> Players { get; set; }
        public Map Map { get; set; }
    }
}
