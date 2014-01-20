using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameLogic.Game;
using GameLogic.Helpers;

namespace GameLogic.Market
{
    public class Market : IMarket
    {
        #region Private Fields

        private readonly object _syncObject = new object();
        private readonly Game.Game _game;
        private readonly List<Order> _orders = new List<Order>();

        #endregion

        #region Events

        public event OrderPlaced OrderPlaced;

        #endregion

        #region Constructor

        public Market(Game.Game game)
        {
            _game = game;
        }

        #endregion

        #region Trade

        public bool PlaceOrder(Order order)
        {
            lock (_syncObject)
            {
                List<Order> playerOrders;
                if (!_orders.Contains(order))
                {
                    _orders.Add(order);
                    if (OrderPlaced != null)
                    {
                        OrderPlaced(_game, new EventArgs());
                    }
                }
            }
            return true;
        }

        public bool AcceptOder(Player acceptedBy, Guid orderId, Player orderOwner)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetOrders()
        {
            IEnumerable<Order> allOrders;

            lock (_syncObject)
            {
                allOrders = _orders;
            }

            return allOrders;
        }

        public IEnumerable<Order> GetOrders(string playerId)
        {
            ExceptionHelper.ThrowIfNull(playerId, "playerId");

            IEnumerable<Order> playerOrders;

            lock (_syncObject)
            {
                playerOrders = _orders.Where(p => p.Buyer.PlayerId == playerId || p.Seller.PlayerId == playerId);
            }

            return playerOrders;
        }

        public void SyncOrders(IEnumerable<Order> orders)
        {
            throw new NotImplementedException();
        }

        public void ScavengeOrders(IEnumerable<Player> players)
        {
            ExceptionHelper.ThrowIfNull(players, "players");

            List<Order> ordersList;
            List<Order> ordersToRemove = new List<Order>();

            foreach (Player p in players)
            {
                ordersToRemove.Clear();

                lock (_syncObject)
                {
                    //if (_orders.TryGetValue(p.PlayerId, out ordersList))
                    //{
                    //    ResourceType rsType;
                    //    for (int i = 0; i < ordersList.Count; i++)
                    //    {
                    //        //if (p.PlayerId != ordersList[i].Seller)
                    //        //    throw new InvalidOperationException("Order owner (Seller) is invalid");

                    //        if (ordersList[i].OrderType == OrderType.Sell)
                    //        {
                    //            rsType = ordersList[i].ResourceType;
                    //            Resource playerResource = p.Resources.FirstOrDefault(r => r.Type == rsType);

                    //            if (playerResource == null || playerResource.Qty == 0)
                    //            {
                    //                ordersToRemove.Add(ordersList[i]);
                    //                continue;
                    //            }

                    //            ordersList[i].Qty = playerResource.Qty;
                    //        }
                    //    }

                    //    foreach (Order o in ordersToRemove)
                    //    {
                    //        ordersList.Remove(o);
                    //    }
                    //}
                }
            }
        }

        #endregion

        #region Builds

        public void BuildCity(Player player)
        {
            if (player.Resources.First(r => r.Type == ResourceType.Soil).Qty >= 1
                && player.Resources.First(r => r.Type == ResourceType.Wood).Qty >= 1
                && player.Resources.First(r => r.Type == ResourceType.Wool).Qty >= 1
                && player.Resources.First(r => r.Type == ResourceType.Corn).Qty >= 1)
            {
                player.Resources.First(r => r.Type == ResourceType.Soil).Qty--;
                player.Resources.First(r => r.Type == ResourceType.Wood).Qty--;
                player.Resources.First(r => r.Type == ResourceType.Wool).Qty--;
                player.Resources.First(r => r.Type == ResourceType.Corn).Qty--;
            }
            else
            {
                throw new InvalidOperationException("not available resource");
            }
        }

        public void UpgardeCity(Player player)
        {
            if (player.Resources.First(r => r.Type == ResourceType.Minerals).Qty >= 3
                && player.Resources.First(r => r.Type == ResourceType.Corn).Qty >= 2)
            {
                player.Resources.First(r => r.Type == ResourceType.Minerals).Qty = player.Resources.First(r => r.Type == ResourceType.Minerals).Qty - 3;
                player.Resources.First(r => r.Type == ResourceType.Corn).Qty = player.Resources.First(r => r.Type == ResourceType.Minerals).Qty - 2;
            }
            else
            {
                throw new InvalidOperationException("not available resource");
            }
        }

        public void BuildRoad(Player player)
        {
            if (player.Resources.First(r => r.Type == ResourceType.Soil).Qty >= 1
                && player.Resources.First(r => r.Type == ResourceType.Wood).Qty >= 1)
            {
                player.Resources.First(r => r.Type == ResourceType.Soil).Qty--;
                player.Resources.First(r => r.Type == ResourceType.Wood).Qty--;
            }
            else
            {
                throw new InvalidOperationException("not available resource");
            }
        }

        public void RobberTime(IEnumerable<Player> players)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public delegate void OrderPlaced(Game.Game sender, EventArgs args);
}
