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
                        _game.RiseToast(new ToasterUpdateArgs { Title = "Новое предложение на рынке. Возможны скидки.", Body = order.ToString(), Type = ToastType.Info });
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

                if (order.BuyResources.Any(resourceToBuy => order.Seller.Resources.First(r => r.Type == resourceToBuy.Type).Qty < resourceToBuy.Qty))
                {
                    throw new InvalidOperationException("Продавец бомж, да у него столько нету. Сделка не прокатит");
                }
                if (order.SellResources.Any(resourceToSell => order.Buyer.Resources.First(r => r.Type == resourceToSell.Type).Qty < resourceToSell.Qty))
                {
                    throw new InvalidOperationException("Покупатель бомж, да у него столько нету. Сделка не прокатит");
                }

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
                    _game.RiseToast(new ToasterUpdateArgs { Title = "Зарегестрирована сделка.", Body = order.ToString(), Type = ToastType.Success });
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
            lock (_syncObject)
            {
                var orderToDestroy = new List<Order>();
                foreach (var order in _orders)
                {
                    if (order.HasSellerAcceptance && order.BuyResources.Any(resourceToBuy => order.Seller.Resources.First(r => r.Type == resourceToBuy.Type).Qty < resourceToBuy.Qty))
                    {
                        orderToDestroy.Add(order);
                    }
                    if (order.HasBuyerAcceptance && order.SellResources.Any(resourceToSell => order.Buyer.Resources.First(r => r.Type == resourceToSell.Type).Qty < resourceToSell.Qty))
                    {
                        orderToDestroy.Add(order);
                    }
                }
                foreach (var destroyOrer in orderToDestroy)
                {
                    _orders.Remove(destroyOrer);
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
                throw new InvalidOperationException("Да вы батенька тот еще бомжара. Для постройка поселения нужно 1 глина 1 шерсть 1 пшеница и 1 деревяшка, укрепления для");
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
                throw new InvalidOperationException("Да вы батенька тот еще бомжара. Для агрейда города нужно 3 камня и 2 пшеницы");
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
                if (player.FreeRoadCount <= 0)
                {
                    throw new InvalidOperationException("Да вы батенька тот еще бомжара. Для постройки дороги требуется 1 кирпич и 1 деревяшка");
                }
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
