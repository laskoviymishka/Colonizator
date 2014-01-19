using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Market
{
	public class Market : IMarket
	{
		public bool PlaceOrder(string playerId, Order order)
		{
			throw new NotImplementedException();
		}

		public bool AcceptOder(string playerId, Guid orderId)
		{
			throw new NotImplementedException();
		}

		public void ScavengeOders(IEnumerable<string> palyerIds)
		{
			throw new NotImplementedException();
		}

        public Order GetOrders(Game.Player player)
        {
            throw new NotImplementedException();
        }
    }
}
