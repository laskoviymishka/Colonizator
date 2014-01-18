using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace GameLogic.Game
{
    public class Map
    {
        public string Id { get; set; }
        public List<Player> Players { get; set; }
        public MapController MapController { get; set; }
    }
}
