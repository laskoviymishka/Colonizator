using System;

namespace Model.Elements
{
    [Serializable]
    public struct EdgeKey
    {
        private int _hexagonA;
        private int _hexagonB;

        public EdgeKey(int hexagonA, int hexagonB)
        {
            if (hexagonA < hexagonB)
            {
                _hexagonA = hexagonA;
                _hexagonB = hexagonB;
            }
            else
            {
                _hexagonA = hexagonB;
                _hexagonB = hexagonA;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is EdgeKey)
            {
                return Equals((EdgeKey)obj);
            }

            return false;
        }

        public bool Equals(EdgeKey other)
        {
            return (_hexagonA == other._hexagonA) && (_hexagonB == other._hexagonB);
        }

        public override int GetHashCode()
        {
            return _hexagonA ^ (_hexagonB * 2345);
        }

        public override string ToString()
        {
            return string.Format("[{0} : {1}]", _hexagonA, _hexagonB);
        }
    }
}