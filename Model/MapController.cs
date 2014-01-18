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
				{    0,    0,  -19,    1,    2,    3,   -5,    0,    0},
				{       0,  -18,    4,    5,    6,    7,   -6,    0,    0},
				{    0,  -17,    8,    9,   10,   11,   12,   -7,    0},
				{       0,  -16,   13,   14,   15,   16,   -8,    0,    0},
				{    0,    0,  -15,   17,   18,   19,   -9,    0,    0},
				{       0,    0,  -14,  -13,  -12,  -11,  -10,    0,    0},
				{    0,    0,    0,    0,    0,    0,    0,    0,    0},
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

						AddEdge(currentIndex, rightIndex);
						AddEdge(currentIndex, bottomIndex);
						AddEdge(currentIndex, rightBottomIndex);

						AddNode(currentIndex, rightIndex, rightBottomIndex);
						AddNode(currentIndex, bottomIndex, rightBottomIndex);

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
						hexagon.ResourceType = random.Next(5) + 1;

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

		public Hexagon[][] GetMap()
		{
			return _map;
		}

		private void AddEdge(int currentIndex, int nextIndex)
		{
			if (currentIndex != 0)
			{
				Hexagon current = GetOrCreateHexagon(currentIndex);

				if (nextIndex != 0)
				{
					Hexagon next = GetOrCreateHexagon(nextIndex);

					Edge edge = new Edge(current, next);

					_edges.Add(new EdgeKey(currentIndex, nextIndex), edge);

					current.Edges.Add(edge);
					next.Edges.Add(edge);
				}
			}
		}

		private void AddNode(int currentIndex, int rightIndex, int bottomIndex)
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

						Node node = new Node(current, right, bottom);

						_nodes.Add(new NodeKey(currentIndex, rightIndex, bottomIndex), node);

						current.Nodes.Add(node);
						right.Nodes.Add(node);
						bottom.Nodes.Add(node);
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
	}
}