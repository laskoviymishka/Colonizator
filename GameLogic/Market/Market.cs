using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Market
{
	public class Market : IMarket
	{
		public IEnumerable<Order> GetOrders()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Order> GetOrders(string playerId)
		{
			throw new NotImplementedException();
		}

		public bool PlaceOrder(Order order)
		{
			throw new NotImplementedException();
		}

		public bool AcceptOder(Game.Player acceptedBy, Guid orderId, Game.Player orderOwner)
		{
			throw new NotImplementedException();
		}

		public void SyncOrders(IEnumerable<Order> orders)
		{
			throw new NotImplementedException();
		}

		public void ScavengeOrders(IEnumerable<Game.Player> players)
		{
			throw new NotImplementedException();
		}
	}
}
