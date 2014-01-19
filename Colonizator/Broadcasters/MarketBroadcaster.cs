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
        private MapBroadcaster _broadcaster;
        private IMarket _market;

        private MarketBroadcaster()
        {
            _context = GlobalHost.ConnectionManager.GetHubContext<MarketHub>();
            _broadcaster = MapBroadcaster.Instance;
            foreach (var game in _broadcaster.Games)
            {
                game.OrderUpdate += GameOnOrderUpdate;
                game.ResourceUpdate += GameOnResourceUpdate;
            }
        }

        private void GameOnResourceUpdate(Game sender, ResourceUpdateArgs args)
        {
            _context.Clients.Clients(args.Player.ConnectionIds.ToList()).updateResource(args.Player.Resources);
        }

        private void GameOnOrderUpdate(Game sender, OrderUpdateArgs args)
        {
            _context.Clients.Clients(args.Player.ConnectionIds.ToList()).updateOrder(args.Player.Orders);
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

        public void InitMarket()
        {
            _broadcaster = MapBroadcaster.Instance;
            foreach (var game in _broadcaster.Games)
            {
                game.OrderUpdate += GameOnOrderUpdate;
                game.ResourceUpdate += GameOnResourceUpdate;
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
            if (sender is Resource)
            {

            }
        }
    }
}
