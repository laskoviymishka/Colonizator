using System;

namespace Model.Elements
{
	[Serializable]
	public sealed class Edge
	{
		public Edge(Hexagon hexagonA, Hexagon hexagonB)
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

		public Hexagon HexagonA
		{
			get;
			private set;
		}

		public Hexagon HexagonB
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
			return string.Format("[{0} : {1}]-> P:{2}", HexagonA.Index, HexagonB.Index, PlayerId);
		}
	}
}
