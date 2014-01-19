using System.Collections.Specialized;
using Colonizator.Hubs;
using GameLogic.Game;
using GameLogic.Market;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonizator.Broadcasters
{
    public class MarketBroadcaster
    {
        private static MarketBroadcaster _instance;
        private IHubContext _context;
        private Map _map;
        private IMarket _market;

        private MarketBroadcaster()
        {
            _context = GlobalHost.ConnectionManager.GetHubContext<MarketHub>();
        }
        public static MarketBroadcaster Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MarketBroadcaster();
                }
                return _instance;
            }
        }

        public void InitMarket(Map map)
        {
            foreach (var player in map.Players)
            {
                player.Resources.CollectionChanged += ResourcesOnCollectionChanged;
                player.Orders.CollectionChanged += OrdersOnCollectionChanged;
                _context.Clients.Client(player.PlayerId).playerOrder(_market.GetOrders(player));
            }
        }

        private void OrdersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (sender is Order)
            {

            }
        }

        private void ResourcesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {

        }
    }
}
