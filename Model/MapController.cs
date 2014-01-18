﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Elements;

namespace Model
{
	public sealed class MapController
	{
		private static int[,] _hexagonIndices =
			new int[,] 
			{
				{       0,    0,   -1,   -2,   -3,   -4,    0,    0,    0},
				{    0,    0,   -5,    1,    2,    3,   -6,    0,    0},
				{       0,   -7,    4,    5,    6,    7,   -8,    0,    0},
				{    0,   -9,    8,    9,   10,   11,   12,  -10,    0},
				{       0,  -11,   13,   14,   15,   16,  -12,    0,    0},
				{    0,    0,  -13,   17,   18,   19,  -14,    0,    0},
				{       0,    0,  -15,  -16,  -17,  -18,    0,    0,    0},
				{    0,    0,    0,    0,    0,    0,    0,    0,    0},
			};

		private static int[,] _nodeMappings =
			new int[,]
			{
				{ 2, 4, 0},
				{ 3, 5, 1}
			};

		private static int _minIndex = -19;
		private static int _maxIndex = 19;

		private readonly int _zeroPosition;
		private readonly Hexagon[] _hexagones;

		private readonly Hexagon[][] _map;

		private readonly Dictionary<NodeKey, Node> _nodes = new Dictionary<NodeKey, Node>();

		private readonly Dictionary<EdgeKey, Edge> _edges = new Dictionary<EdgeKey, Edge>();

		public MapController()
		{
			_hexagones = new Hexagon[_maxIndex - _minIndex + 1];
			_zeroPosition = -_minIndex;

			List<Hexagon[]> map = new List<Hexagon[]>();

			int rowCount = _hexagonIndices.GetLength(0);
			int columnCount = _hexagonIndices.GetLength(1);

			List<Hexagon> row = null;

			for (int i = 0; i < rowCount; ++i)
			{
				for (int j = 0; j < columnCount; ++j)
				{
					int currentIndex = _hexagonIndices[i, j];

					if (currentIndex != 0)
					{
						if (row == null)
						{
							row = new List<Hexagon>();
						}

						int rightIndex = _hexagonIndices[i, j + 1];
						int bottomIndex = _hexagonIndices[i + 1, j - (i & 1)];
						int rightBottomIndex = _hexagonIndices[i + 1, j + 1 - (i & 1)];

						AddEdge(currentIndex, rightIndex, 1);
						AddEdge(currentIndex, rightBottomIndex, 2);
						AddEdge(currentIndex, bottomIndex, 3);

						AddNode(currentIndex, rightIndex, rightBottomIndex, 0);
						AddNode(currentIndex, rightBottomIndex, bottomIndex, 1);

						row.Add(GetOrCreateHexagon(currentIndex));
					}
				}

				if (row != null)
				{
					map.Add(row.ToArray());
					row = null;
				}
			}

			_map = map.ToArray();
		}

		public EventHandler StateChanged
		{
			get;
			set;
		}

		public void Initialize()
		{
			Random random = new Random();

			foreach (Hexagon hexagon in _hexagones)
			{
				if (hexagon != null)
				{
					if (hexagon.Index == 10)
					{
						hexagon.FaceNumber = 7;
					}
					if (hexagon.Index > 0)
					{
						hexagon.ResourceType = random.Next(6) + 1;

						int faceNumber = 1 + random.Next(12);

						hexagon.FaceNumber = faceNumber >= 7 ? faceNumber + 1 : faceNumber;
					}
					else
					{
						hexagon.ResourceType = random.Next(7);
					}
				}
			}
		}

		public void Randomize()
		{
			Random random = new Random();

			foreach(Node node in _nodes.Values)
			{
				if (random.Next(5) == 1)
				{
					node.PlayerId = 1 + random.Next(6);
					node.CitySize = 1 + random.Next(2);
				}
			}

			foreach (Edge edge in _edges.Values)
			{
				if (random.Next(3) == 1)
				{
					edge.PlayerId = 1 + random.Next(6);
				}
			}
		}

		public void BuildRoad(int hexagonIndex, int position, int playerId)
		{
			Hexagon hexagon = _hexagones[hexagonIndex];
			Edge edge = hexagon.Edges[position];

			edge.PlayerId = playerId;
		}

		public void BuildCity(int hexagonIndex, int position, int playerId, int citySize)
		{
			Hexagon hexagon = _hexagones[hexagonIndex];
			Node node = hexagon.Nodes[position];

			node.PlayerId = playerId;
			node.CitySize = citySize;
		}

		public IEnumerable<Node> GetAvailableNodes(int playerId)
		{
			HashSet<Node> potentialNodes = new HashSet<Node>();

			foreach (Edge edge in _edges.Values)
			{
				if (edge.PlayerId == playerId)
				{
					potentialNodes.Add(edge.HexagonA.FirstNode);
					potentialNodes.Add(edge.HexagonA.SecondNode);
					potentialNodes.Add(edge.HexagonB.FirstNode);
					potentialNodes.Add(edge.HexagonB.SecondNode);
				}
			}

			IEnumerable<Node> result = potentialNodes.Count > 0 ? (IEnumerable<Node>)potentialNodes : (IEnumerable<Node>)_nodes.Values;

			return result.Where(x => x != null && ((x.PlayerId == playerId) || (x.PlayerId < 0)));
		}

		public IEnumerable<Edge> GetAvailableEdges(int playerId)
		{
			HashSet<Edge> result = new HashSet<Edge>();

			foreach (Node node in _nodes.Values)
			{
				if (node.PlayerId == playerId)
				{
					result.Add(node.HexagonA.FirstEdge);
					result.Add(node.HexagonA.SecondEdge);
					result.Add(node.HexagonB.FirstEdge);
					result.Add(node.HexagonB.SecondEdge);
					result.Add(node.HexagonC.FirstEdge);
					result.Add(node.HexagonC.SecondEdge);
				}
			}

			return result.Where(x => x != null && x.PlayerId < 0);
		}

		public bool IsEdgeAvailable(int hexagonIndex, int position, int playerId)
		{
			return true;
		}

		public bool IsNodeAvailable(int hexagonIndex, int position, int playerId)
		{
			return true;
		}

		public int GetCitySize(int hexagonIndex, int position)
		{
			return _hexagones[hexagonIndex].Nodes[position].CitySize;
		}

		public Hexagon[][] GetMap()
		{
			return _map;
		}

		public IEnumerable<Node> Nodes
		{
			get
			{
				return _nodes.Values;
			}
		}

		public IEnumerable<Edge> Edges
		{
			get
			{
				return _edges.Values;
			}
		}

		private void AddEdge(int currentIndex, int nextIndex, int positionCurrent)
		{
			if (currentIndex != 0)
			{
				Hexagon current = GetOrCreateHexagon(currentIndex);

				if ((nextIndex > 0) || (currentIndex > 0))
				{
					Hexagon next = GetOrCreateHexagon(nextIndex);

					int positionNext = ReverseOrder(positionCurrent);

					Edge edge = new Edge(
						new HexagonPosition(current, positionCurrent),
						new HexagonPosition(next, positionNext));

					_edges.Add(new EdgeKey(currentIndex, nextIndex), edge);

					current.Edges[positionCurrent] = edge;
					next.Edges[positionNext] = edge;
				}
			}
		}

		private void AddNode(int currentIndex, int rightIndex, int bottomIndex, int mappingIndex)
		{
			if (currentIndex != 0)
			{
				Hexagon current = GetOrCreateHexagon(currentIndex);

				if (rightIndex != 0)
				{
					Hexagon right = GetOrCreateHexagon(rightIndex);

					if (bottomIndex != 0)
					{
						Hexagon bottom = GetOrCreateHexagon(bottomIndex);

						int positionCurrent = _nodeMappings[mappingIndex, 0];
						int positionRight = _nodeMappings[mappingIndex, 1];
						int positionBottom = _nodeMappings[mappingIndex, 2];

						Node node = new Node(
							new HexagonPosition(current, positionCurrent),
							new HexagonPosition(right, positionRight),
							new HexagonPosition(bottom, positionBottom));

						_nodes.Add(new NodeKey(currentIndex, rightIndex, bottomIndex), node);

						current.Nodes[positionCurrent] = node;
						right.Nodes[positionRight] = node;
						bottom.Nodes[positionBottom] = node;
					}
				}
			}
		}

		private Hexagon GetOrCreateHexagon(int index)
		{
			int position = index + _zeroPosition;

			Hexagon result = _hexagones[position];
			
			if (result == null)
			{
				result = new Hexagon(index);

				_hexagones[position] = result;
			}

			return result;
		}

		private static int ReverseOrder(int order)
		{
			return (order + 3) % 6;
		}

		private void OnStateChanged()
		{
			EventHandler handler = StateChanged;

			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}
	}
}
