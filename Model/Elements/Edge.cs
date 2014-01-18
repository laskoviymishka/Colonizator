using System;

namespace Model.Elements
{
	[Serializable]
	public sealed class Edge
	{
		public Edge(HexagonPosition hexagonA, HexagonPosition hexagonB)
		{
			if (hexagonA == null)
			{
				throw new ArgumentNullException("hexagonA");
			}

			if (hexagonB == null)
			{
				throw new ArgumentNullException("hexagonB");
			}

			HexagonA = hexagonA;
			HexagonB = hexagonB;
			PlayerId = -1;
		}

		public HexagonPosition HexagonA
		{
			get;
			private set;
		}

		public HexagonPosition HexagonB
		{
			get;
			private set;
		}

		public int PlayerId
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("[{0} : {1}]-> P:{2}", HexagonA.Hexagon.Index, HexagonB.Hexagon.Index, PlayerId);
		}
	}
}
