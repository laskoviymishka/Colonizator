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
                        OrderPlaced(_game, new GameStateUpdateArgs() { Action = GameAction.RegularUpdate });
                    }
                }
            }
            return true;
        }

        public Order GetOrder(Guid orderId)
        {
            lock (_syncObject)
            {
                return _orders.FirstOrDefault(o => o.Id == orderId);
            }
        }

        public bool AcceptOder(Guid orderId)
        {
            lock (_syncObject)
            {
                var order = _orders.FirstOrDefault(o => o.Id == orderId);
                if (order == null) return false;
                if (!order.HasBuyerAcceptance || !order.HasSellerAcceptance) return false;
                foreach (var resourceToBuy in order.BuyResources)
                {
                    order.Buyer.Resources.First(r => r.Type == resourceToBuy.Type).Qty += resourceToBuy.Qty;
                    order.Seller.Resources.First(r => r.Type == resourceToBuy.Type).Qty -= resourceToBuy.Qty;
                }
                foreach (var resourceToSell in order.SellResources)
                {
                    order.Buyer.Resources.First(r => r.Type == resourceToSell.Type).Qty -= resourceToSell.Qty;
                    order.Seller.Resources.First(r => r.Type == resourceToSell.Type).Qty += resourceToSell.Qty;
                }

                _orders.Remove(order);
                if (OrderPlaced != null)
                {
                    OrderPlaced(_game, new GameStateUpdateArgs() { Action = GameAction.RegularUpdate });
                }
                return true;
            }
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

        public IEnumerable<Order> GetOrders(Player player)
        {
            IEnumerable<Order> playerOrders;

            lock (_syncObject)
            {
                if (player.PlayerName == _game.CurrentPlayer.PlayerName)
                {
                    playerOrders = _orders.Where(p => p.Buyer == null);
                }
                else
                {
                    playerOrders = _orders.Where(p => p.Seller == null && p.Buyer.PlayerName == _game.CurrentPlayer.PlayerName || p.Seller != null && p.Seller.PlayerName == player.PlayerName);
                }

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

    public delegate void OrderPlaced(Game.Game sender, GameStateUpdateArgs args);
}
