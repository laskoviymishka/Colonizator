using System;
using System.Collections.Generic;
using System.Linq;
using Model.Elements;

namespace Model
{
    public sealed class MapController
    {
        #region private fields

        private static readonly int[,] _hexagonIndices =
        {
            {0, 0, -1, -2, -3, -4, 0, 0, 0},
            {0, 0, -5, 1, 2, 3, -6, 0, 0},
            {0, -7, 4, 5, 6, 7, -8, 0, 0},
            {0, -9, 8, 9, 10, 11, 12, -10, 0},
            {0, -11, 13, 14, 15, 16, -12, 0, 0},
            {0, 0, -13, 17, 18, 19, -14, 0, 0},
            {0, 0, -15, -16, -17, -18, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0}
        };

        private static readonly int[,] _nodeMappings =
        {
            {2, 4, 0},
            {3, 5, 1}
        };

        private static int _minIndex = -19;
        private static int _maxIndex = 19;
        private readonly Dictionary<EdgeKey, Edge> _edges = new Dictionary<EdgeKey, Edge>();

        private readonly Hexagon[] _hexagones;

        private readonly Hexagon[][] _map;

        private readonly Dictionary<NodeKey, Node> _nodes = new Dictionary<NodeKey, Node>();
        private readonly int _zeroPosition;

        #endregion

        #region Constructor

        public MapController()
        {
            _hexagones = new Hexagon[_maxIndex - _minIndex + 1];
            _zeroPosition = -_minIndex;

            var map = new List<Hexagon[]>();

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

        #endregion

        #region Events

        public EventHandler StateChanged { get; set; }

        #endregion

        #region Initialize

        public void Initialize()
        {
            var random = new Random();

            var tiles = new List<int>();
            tiles.Add(2); //пустыня
            tiles.Add(3);//дерево
            tiles.Add(3);//дерево
            tiles.Add(3);//дерево
            tiles.Add(3);//дерево
            tiles.Add(4);//шерсть
            tiles.Add(4);//шерсть
            tiles.Add(4);//шерсть
            tiles.Add(4);//шерсть
            tiles.Add(5);//зерно
            tiles.Add(5);//зерно
            tiles.Add(5);//зерно
            tiles.Add(5);//зерно
            tiles.Add(6);//глина
            tiles.Add(6);//глина
            tiles.Add(6);//глина
            tiles.Add(7);//руда
            tiles.Add(7);//руда
            tiles.Add(7);//руда

            foreach (Hexagon hexagon in _hexagones)
            {
                if (hexagon != null)
                {
                    if (hexagon.Index == 10)
                    {
                        hexagon.FaceNumber = 7;
                        hexagon.ResourceType = 7;
                    }
                    if (hexagon.Index > 0)
                    {
                        var randomPossibleTile = tiles[random.Next(tiles.Count)];
                        hexagon.ResourceType = randomPossibleTile;
                        tiles.Remove(randomPossibleTile);

                        int faceNumber = 1 + random.Next(12);
                        if (randomPossibleTile == 2)
                        {
                            hexagon.FaceNumber = 7;
                        }
                        else
                        {
                            hexagon.FaceNumber = faceNumber >= 7 ? faceNumber + 1 : faceNumber;
                        }
                    }
                    else
                    {
                        hexagon.ResourceType = 0;
                    }
                }
            }
        }

        public void Randomize()
        {
            var random = new Random();

            foreach (Node node in _nodes.Values)
            {
                if (random.Next(5) == 1)
                {
                    node.PlayerId = random.Next(5);
                    node.CitySize = 1 + random.Next(2);
                }
            }

            foreach (Edge edge in _edges.Values)
            {
                if (random.Next(3) == 1)
                {
                    edge.PlayerId = random.Next(5);
                }
            }
        }

        #endregion

        #region Building


        public void BuildRoad(int hexIndex, int hexA, int hexB, int playerId)
        {
            Edge edge = GetEdge(hexA, hexB, hexIndex);

            edge.PlayerId = playerId;

            OnStateChanged();
        }

        public void BuildCity(int playerId, int hexA, int hexB, int hexC, int hexIndex)
        {
            Node node = GetNode(hexA, hexB, hexC, hexIndex);

            node.PlayerId = playerId;
            node.CitySize++;

            OnStateChanged();
        }

        #endregion

        #region Misc Methods

        public Node GetNode(int hexA, int hexB, int hexC, int hexIndex)
        {
            return
                Nodes.First(
                    n =>
                        n.HexagonA.Position == hexA && n.HexagonB.Position == hexB &&
                        n.HexagonC.Position == hexC && n.HexagonA.Hexagon.Index == hexIndex);
        }

        public Edge GetEdge(int hexA, int hexB, int hexIndex)
        {
            return
                Edges.First(
                    n =>
                        n.HexagonA.Hexagon.Index == hexA
                        && n.HexagonB.Hexagon.Index == hexB);
        }

        public IEnumerable<Node> GetAvailableNodes(int playerId)
        {
            var potentialNodes = new HashSet<Node>();

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

            IEnumerable<Node> result = potentialNodes.Count > 0 ? potentialNodes : (IEnumerable<Node>)_nodes.Values;

            return result.Where(x => x != null && ((x.PlayerId == playerId) || (x.PlayerId < 0)));
        }

        public IEnumerable<Edge> GetAvailableEdges(int playerId)
        {
            var result = new HashSet<Edge>();

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

        public bool IsEdgeAvailable(int hexagonIndex, int hexA, int hexB)
        {
            return true;
        }

        public bool IsNodeAvailable(int hexA, int hexB, int hexC)
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

        #endregion

        #region Properties

        public IEnumerable<Node> Nodes
        {
            get { return _nodes.Values; }
        }

        public IEnumerable<Edge> Edges
        {
            get { return _edges.Values; }
        }

        #endregion

        #region Private Helpers

        private void AddEdge(int currentIndex, int nextIndex, int positionCurrent)
        {
            if (currentIndex != 0)
            {
                Hexagon current = GetOrCreateHexagon(currentIndex);

                if ((nextIndex > 0) || (currentIndex > 0))
                {
                    Hexagon next = GetOrCreateHexagon(nextIndex);

                    int positionNext = ReverseOrder(positionCurrent);

                    var edge = new Edge(
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

                        var node = new Node(
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

        #endregion

        public bool IsNodeAvailable(int hexA, int hexB, int hexC, int playerId, int hexIndex)
        {
            var node = GetNode(hexA, hexB, hexC, hexIndex);
            if (node.PlayerId != playerId && node.PlayerId >= 0) { return false; }
            if (node.CitySize != 0) { return false; }
            return true;
        }

        public bool IsUpgradeTown(int hexA, int hexB, int hexC, int playerId, int hexIndex)
        {
            var node = GetNode(hexA, hexB, hexC, hexIndex);
            if (node.PlayerId != playerId) { return false; }
            if (node.CitySize != 1) { return false; }
            return true;
        }
    }
}