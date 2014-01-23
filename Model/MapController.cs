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
        public int RobberInitPosition { get; set; }
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
                    if (hexagon.Index > 0)
                    {
                        if (hexagon.Index == 10)
                        {
                        }
                        var randomPossibleTile = tiles[random.Next(tiles.Count)];
                        hexagon.ResourceType = randomPossibleTile;
                        tiles.Remove(randomPossibleTile);
                        if (randomPossibleTile == 2)
                        {
                            hexagon.FaceNumber = 7;
                            RobberInitPosition = hexagon.Index;
                        }
                        else
                        {
                            int faceNumber = 1 + random.Next(12);
                            hexagon.FaceNumber = faceNumber >= 7 ? faceNumber + 1 : faceNumber;
                        }
                    }
                    else
                    {
                        hexagon.ResourceType = 0;
                    }
                }
            }
            foreach (var hexagon in _hexagones.Where(h => h != null && h.Edges != null && h.Edges.Count(e => e != null) == 6 && h.Nodes != null && h.Nodes.Count(e => e == null) != 6).ToList())
            {
                foreach (var edge in hexagon.Edges)
                {
                    foreach (var node in hexagon.Nodes)
                    {
                        if (edge.HexagonA.Hexagon.Index == node.HexagonA.Hexagon.Index && edge.HexagonB.Hexagon.Index == node.HexagonB.Hexagon.Index ||
                            edge.HexagonA.Hexagon.Index == node.HexagonB.Hexagon.Index && edge.HexagonB.Hexagon.Index == node.HexagonC.Hexagon.Index ||
                            edge.HexagonA.Hexagon.Index == node.HexagonA.Hexagon.Index && edge.HexagonB.Hexagon.Index == node.HexagonC.Hexagon.Index)
                        {
                            if (edge.NodeA == null || edge.NodeB == node)
                            {
                                edge.NodeA = node;
                            }
                            else
                            {
                                edge.NodeB = node;
                            }
                            if(node.Edges == null) node.Edges = new HashSet<Edge>();
                            node.Edges.Add(edge);
                            foreach (var nodeEdge in node.Edges)
                            {
                                if (edge.Edges == null) edge.Edges = new HashSet<Edge>();
                                edge.Edges.Add(nodeEdge);
                            }
                        }
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
            var key = new NodeKey(hexA, hexB, hexC);
            return _nodes[key];
        }

        public Edge GetEdge(int hexA, int hexB, int hexIndex)
        {
            var key = new EdgeKey(hexA, hexB);
            return _edges[key];
        }

        public IEnumerable<Node> GetAvailableNodes(int playerId)
        {
            var result = new HashSet<Node>();
            foreach (var node in from node in _nodes.Values
                                 where node.PlayerId < 0 && node.Edges != null
                                    && node.Edges.All(e => e.NodeA != null && e.NodeB != null)
                                 where node.Edges.Any(e => e.PlayerId == playerId)
                                 where node.Edges.All(e => e.NodeA.PlayerId < 0 && e.NodeB.PlayerId < 0)
                                 select node)
            {
                result.Add(node);
            }
            return result;
        }

        public IEnumerable<Edge> GetAvailableEdges(int playerId)
        {
            var result = new HashSet<Edge>();
            foreach (var edge in _edges.Values)
            {
                if (edge.PlayerId < 0)
                {
                    if (edge.NodeA != null && edge.NodeA.PlayerId == playerId ||
                        edge.NodeB != null && edge.NodeB.PlayerId == playerId)
                    {
                        result.Add(edge);
                    }
                }
                else
                {
                    if (edge.PlayerId != playerId) continue;
                    foreach (var childEdge in edge.Edges)
                    {
                        result.Add(childEdge);
                    }
                }
            }
            return result;
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