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
		public Order()
		{
			AcceptedResources = new List<Resource>();
		}

		public Guid Id { get; private set; }

		public OrderType OrderType { get; set; }
		public ResourceType ResourceType { get; set; }
		public int Qty { get; set; }

		public string OrderOwnerId { get; set; }
		public string OrderConsumerId { get; set; }

		public bool HasOwnerAcceptance { get; set; }
		public bool HasConsumerAcceptance { get; set; }

		public List<Resource> AcceptedResources { get; set; }

		public static Order CreateNew(OrderType orderType)
		{
			return new Order
			{
				Id = Guid.NewGuid(),
				OrderType = orderType
			};
		}
	}
}
