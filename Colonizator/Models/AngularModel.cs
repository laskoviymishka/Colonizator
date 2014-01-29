using System.Collections.Generic;
using GameLogic.Game;
using GameLogic.Market;
using GameLogic.Models;

namespace Colonizator.Models
{
	public class AngularModel
	{
		public Player CurrentPlayer { get; set; }
		public int PlayerId { get; set; }
		public MapStateModel MapState { get; set; }
		public IEnumerable<OrderAngularModel> Orders { get; set; }
	}
}