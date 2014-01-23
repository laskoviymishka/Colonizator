using System;

namespace Model.Elements
{
    [Serializable]
    public sealed class HexagonPosition
    {
        private readonly Hexagon _hexagon;

        private readonly int _position;

        public HexagonPosition(Hexagon hexagon, int position)
        {
            if (hexagon == null)
            {
                throw new ArgumentNullException("hexagon");
            }

            _hexagon = hexagon;
            _position = position;
        }

        public Hexagon Hexagon
        {
            get
            {
                return _hexagon;
            }
        }

        public int Position
        {
            get
            {
                return _position;
            }
        }

        public Node FirstNode
        {
            get
            {
                return Hexagon.Nodes[Position];
            }
        }

        public Node SecondNode
        {
            get
            {
                return Hexagon.Nodes[(Position + 1) % 6];
            }
        }

        public Edge FirstEdge
        {
            get
            {
                return Hexagon.Edges[Position];
            }
        }

        public Edge SecondEdge
        {
            get
            {
                return Hexagon.Edges[(Position + 5) % 6];
            }
        }
    }
}
