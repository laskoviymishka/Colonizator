﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
	class Program
	{
		static void Main(string[] args)
		{
			MapController map = new MapController();

			map.Initialize();
			map.Randomize();


		}
	}
}
