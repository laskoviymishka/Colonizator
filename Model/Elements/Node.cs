using System;

namespace Model.Elements
{
	[Serializable]
	public sealed class Node
	{
		public Node(HexagonPosition hexagonA, HexagonPosition hexagonB, HexagonPosition hexagonC)
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
			HexagonB = hexagonB;
			HexagonC = hexagonC;
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

		public HexagonPosition HexagonC
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
			return string.Format("[{0} : {1} : {2}]-> P:{3}, C:{4}", HexagonA.Hexagon.Index, HexagonB.Hexagon.Index, HexagonC.Hexagon.Index, PlayerId, CitySize);
		}
	}
}
