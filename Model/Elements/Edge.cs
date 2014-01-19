using System;

namespace Model.Elements
{
    [Serializable]
    public sealed class Edge
    {
        private readonly MapController _container;
        private Node _start;
        private Node _end;
        private HexagonPosition _hexTop;
        private HexagonPosition _hexBot;

        public Edge(HexagonPosition hexagonA, HexagonPosition hexagonB, MapController controller)
        {
            _container = controller;
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

        public HexagonPosition HexagonTop
        {
            get
            {
                if (_hexTop == null) CollectHex();
                return _hexTop;
            }
        }

        public HexagonPosition HexagonBot
        {
            get
            {
                if (_hexBot == null) CollectHex();
                return _hexBot;
            }
        }

        public int PlayerId
        {
            get;
            set;
        }

        public Node Start
        {
            get
            {
                if (_start == null) CollectNodes();
                return _start;
            }
        }

        public Node End
        {
            get
            {
                if (_end == null) CollectNodes();
                return _end;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0} : {1}]-> P:{2}", HexagonA.Hexagon.Index, HexagonB.Hexagon.Index, PlayerId);
        }

        private void CollectHex()
        {
            bool firstAdded = false;
            foreach (var edge in HexagonA.Hexagon.Edges)
            {
                if (edge == null || edge == null && _hexBot != null && _hexTop != null) break;
                if (edge.HexagonA.Hexagon.Index == HexagonA.Hexagon.Index || edge.HexagonB.Hexagon.Index == HexagonA.Hexagon.Index)
                {
                    if (!firstAdded)
                    {
                        firstAdded = true;
                        if (edge.HexagonA.Hexagon.Index == HexagonA.Hexagon.Index)
                        {
                            _hexBot = edge.HexagonA;
                        }
                        else
                        {
                            _hexBot = edge.HexagonB;
                        }
                    }
                    else
                    {
                        if (edge.HexagonB.Hexagon.Index == HexagonA.Hexagon.Index)
                        {
                            _hexTop = edge.HexagonA;
                        }
                        else
                        {
                            _hexTop = edge.HexagonB;
                        }
                        break;
                    }
                }
            }
        }

        private void CollectNodes()
        {
            HexagonPosition hexTop = null;
            HexagonPosition hexBot = null;
            bool firstAdded = false;
            foreach (var edge in HexagonA.Hexagon.Edges)
            {
                if (edge == null || edge == null) break;
                if (edge.HexagonA.Hexagon.Index == HexagonA.Hexagon.Index || edge.HexagonB.Hexagon.Index == HexagonA.Hexagon.Index)
                {
                    if (!firstAdded)
                    {
                        firstAdded = true;
                        if (edge.HexagonA.Hexagon.Index == HexagonA.Hexagon.Index)
                        {
                            hexTop = edge.HexagonA;
                        }
                        else
                        {
                            hexTop = edge.HexagonB;
                        }
                    }
                    else
                    {
                        if (edge.HexagonB.Hexagon.Index == HexagonA.Hexagon.Index)
                        {
                            hexBot = edge.HexagonA;
                        }
                        else
                        {
                            hexBot = edge.HexagonB;
                        }
                        break;
                    }
                }
            }
            if (hexTop != null && hexBot != null)
            {
                _start = _container.GetNode(HexagonA.Hexagon.Index, HexagonB.Hexagon.Index, hexTop.Hexagon.Index);
                _end = _container.GetNode(HexagonA.Hexagon.Index, HexagonB.Hexagon.Index, hexBot.Hexagon.Index);
            }
        }
    }
}
