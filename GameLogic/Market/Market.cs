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
		private readonly object _syncObject = new object();
		private readonly Dictionary<string, List<Order>> _orders = new Dictionary<string, List<Order>>();

		public bool PlaceOrder(Order order)
		{
			ExceptionHelper.ThrowIfNull(order, "order");
			ExceptionHelper.ThrowIfNull(order.Qty, 0, "order.Qty");
			ExceptionHelper.ThrowIfNull(order.OrderOwnerId, "order.OrderOwnerId");
			if (order.Id == Guid.Empty)
				throw new ArgumentNullException("order.Guid");

			lock (_syncObject)
			{
				List<Order> playerOrders;
				if (!_orders.TryGetValue(order.OrderOwnerId, out playerOrders))
				{
					playerOrders = new List<Order>(32);
					_orders.Add(order.OrderOwnerId, playerOrders);
				}

				playerOrders.Add(order);
			}

			throw new NotImplementedException();
		}

		public bool AcceptOder(Player acceptedBy, Guid orderId, Player orderOwner)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Order> GetOrders()
		{
			Order[] allOrders;

			lock (_syncObject)
			{
				allOrders = _orders.Values.SelectMany(o => o).ToArray();
			}

			return allOrders;
		}

		public IEnumerable<Order> GetOrders(string playerId)
		{
			ExceptionHelper.ThrowIfNull(playerId, "playerId");

			List<Order> playerOrders;

			lock (_syncObject)
			{
				if (!_orders.TryGetValue(playerId, out playerOrders))
				{
					playerOrders = new List<Order>();
				}
			}

			return playerOrders.ToArray();
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
					if (_orders.TryGetValue(p.PlayerId, out ordersList))
					{
						ResourceType rsType;
						for (int i = 0; i < ordersList.Count; i++)
						{
							if (p.PlayerId != ordersList[i].OrderOwnerId)
								throw new InvalidOperationException("Order owner (OrderOwnerId) is invalid");

							if (ordersList[i].OrderType == OrderType.Sell)
							{
								rsType = ordersList[i].ResourceType;
								Resource playerResource = p.Resources.FirstOrDefault(r => r.Type == rsType);

								if (playerResource == null || playerResource.Qty == 0)
								{
									ordersToRemove.Add(ordersList[i]);
									continue;
								}

								ordersList[i].Qty = playerResource.Qty;
							}
						}

						foreach (Order o in ordersToRemove)
						{
							ordersList.Remove(o);
						}
					}
				}
			}
		}

	}
}
