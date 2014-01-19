using System.Collections.Generic;

namespace Colonizator.Models
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
	}
}