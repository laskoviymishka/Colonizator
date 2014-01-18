using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Market;
using Model;

namespace GameLogic.Game
{
    public class Map
    {
        public event ResourceUpdate ResourceUpdate;
        public event OrderUpdate OrderUpdate;

        public Map(string id, List<Player> players, MapController controller)
        {
            Id = id;
            MapController = controller;
            Players = players;
            foreach (var player in players)
            {
                player.Resources.CollectionChanged += (sender, args) => ResourceUpdate(this, new ResourceUpdateArgs() { Player = player });
                player.Orders.CollectionChanged += (sender, args) => OrderUpdate(this, new OrderUpdateArgs() { Player = player });
            }
        }

        public string Id { get; set; }
        public List<Player> Players { get; set; }
        public MapController MapController { get; set; }
    }

    public delegate void OrderUpdate(Map sender, OrderUpdateArgs args);

    public delegate void ResourceUpdate(Map sender, ResourceUpdateArgs args);
}
