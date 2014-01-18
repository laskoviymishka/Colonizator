using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameLogic.Game;

namespace GameLogic.Market
{
	public interface IMarket
	{
	    Order GetOrders(Player player);

		bool PlaceOrder(string playerId, Order order);
		bool AcceptOder(string playerId, Guid orderId);

		void ScavengeOders(IEnumerable<string> palyerIds);
	}
}
