using System;

namespace Model.Elements
{
    [Serializable]
    public struct NodeKey
    {
        private int _hexagonA;
        private int _hexagonB;
        private int _hexagonC;

        public NodeKey(int hexagonA, int hexagonB, int hexagonC)
        {
            Helper.Minimize(ref hexagonA, ref hexagonB);

            if (Helper.Minimize(ref hexagonB, ref hexagonC))
            {
                Helper.Minimize(ref hexagonA, ref hexagonB);
            }

            _hexagonA = hexagonA;
            _hexagonB = hexagonB;
            _hexagonC = hexagonC;
        }

        public override bool Equals(object obj)
        {
            if (obj is NodeKey)
            {
                return Equals((NodeKey)obj);
            }

            return false;
        }

        public bool Equals(NodeKey other)
        {
            return (_hexagonA == other._hexagonA) && (_hexagonB == other._hexagonB) && (_hexagonC == other._hexagonC);
        }

        public override int GetHashCode()
        {
            return _hexagonA ^ (_hexagonB * 2345) ^ (_hexagonC * 6454);
        }

        public override string ToString()
        {
            return string.Format("[{0} : {1} : {2}]", _hexagonA, _hexagonB, _hexagonC);
        }
    }
}
