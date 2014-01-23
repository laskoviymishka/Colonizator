using System;
using System.Collections.Generic;

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

            if (hexagonA.Hexagon.Index < hexagonB.Hexagon.Index && hexagonA.Hexagon.Index < hexagonC.Hexagon.Index)
            {
                HexagonA = hexagonA;
                if (hexagonB.Hexagon.Index < hexagonC.Hexagon.Index)
                {
                    HexagonB = hexagonB;
                    HexagonC = hexagonC;
                }
                else
                {
                    HexagonC = hexagonB;
                    HexagonB = hexagonC;
                }
            }
            else
            {
                if (hexagonB.Hexagon.Index < hexagonC.Hexagon.Index)
                {
                    HexagonA = hexagonB;
                    if (hexagonA.Hexagon.Index < hexagonC.Hexagon.Index)
                    {
                        HexagonB = hexagonA;
                        HexagonC = hexagonC;
                    }
                    else
                    {
                        HexagonC = hexagonA;
                        HexagonB = hexagonC;
                    }
                }
                else
                {
                    HexagonA = hexagonC;
                    if (hexagonB.Hexagon.Index < hexagonC.Hexagon.Index)
                    {
                        HexagonB = hexagonB;
                        HexagonC = hexagonA;
                    }
                    else
                    {
                        HexagonC = hexagonB;
                        HexagonB = hexagonA;
                    }
                }
            }
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

        public HashSet<Edge> Edges
        {
            get;
            internal set;
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