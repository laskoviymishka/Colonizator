using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Game
{
    public class Map
    {
        public event ResourceUpdate ResourceUpdate;

        public string Id { get; set; }
        public List<Player> Players { get; set; }
    }

    public delegate void ResourceUpdate(object sender, ResourceUpdateArgs args);
}
