﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Elements
{
	[Serializable]
	public sealed class Hexagon
	{
		public Hexagon(int index)
		{
			Index = index;
			Edges = new Edge[6];
			Nodes = new Node[6];
		}

		public int Index
		{
			get;
			private set;
		}

		public int FaceNumber
		{
			get;
			set;
		}

		public int ResourceType
		{
			get;
			set;
		}

		public Edge[] Edges
		{
			get;
			private set;
		}

		public Node[] Nodes
		{
			get;
			private set;
		}

		public override string ToString()
		{
			return string.Format("{0}{{I:{1}, T:{2}, N:{3}}}", GetType().Name, Index, ResourceType, FaceNumber);
		}
	}
}
