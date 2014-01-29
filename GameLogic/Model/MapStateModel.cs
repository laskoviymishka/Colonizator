using System.Collections.Generic;

namespace GameLogic.Models
{
	public class MapStateModel
	{
		public List<CityModel> Cities
		{
			get;
			set;
		}

		public List<RoadModel> Roads
		{
			get;
			set;
		}

		public List<CityModel> PossibleCities
		{
			get;
			set;
		}

		public List<RoadModel> PossibleRoads
		{
			get;
			set;
		}
	}
}