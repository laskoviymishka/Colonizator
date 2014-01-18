using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameLogic.Game;

namespace GameLogic.Market
{
	public class Order
	{
		public Guid Id { get; private set; }
		public OrderType OrderType { get; set; }
		public ResourceType ResourceType { get; set; }
		public int Qty { get; set; }

	}
}
