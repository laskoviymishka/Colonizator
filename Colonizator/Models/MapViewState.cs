using System.Collections.Generic;
using GameLogic.Models;

namespace Colonizator.Models
{
	public class MapViewState
	{
		public List<CityModel> Cities { get; set; }
		public List<RoadModel> Roads { get; set; } 
	}
}