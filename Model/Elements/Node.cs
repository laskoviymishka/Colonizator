using System;

namespace Model.Elements
{
	[Serializable]
	public sealed class Node
	{
		public Node(Hexagon hexagonA, Hexagon hexagonB, Hexagon hexagonC, int orderA)
		{
			if (hexagonA == null)
			{
				throw new ArgumentNullException("hexagonA");
			}

			if (hexagonB == null)
			{
				throw new ArgumentNullException("hexagonB");
			}

			if (hexagonC == null)
			{
				throw new ArgumentNullException("hexagonC");
			}

			HexagonA = hexagonA;
			OrderA = orderA;
			HexagonB = hexagonB;
			HexagonC = hexagonC;
			PlayerId = -1;
		}

		public Hexagon HexagonA
		{
			get;
			private set;
		}

		public int OrderA
		{
			get;
			private set;
		}

		public Hexagon HexagonB
		{
			get;
			private set;
		}

		public Hexagon HexagonC
		{
			get;
			private set;
		}

		public int PlayerId
		{
			get;
			set;
		}

		public int CitySize
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("[{0} : {1} : {2}]-> P:{3}, C:{4}", HexagonA.Index, HexagonB.Index, HexagonC.Index, PlayerId, CitySize);
		}
	}
}
