using System.Collections.Generic;
using GameLogic.Game;

namespace Colonizator.Models
{
	public class OrderAngularModel
	{
		public string Seller { get; set; }
		public string Buyer { get; set; }
		public List<Resource> SellList { get; set; }
		public List<Resource> BuyList { get; set; } 
	}
}