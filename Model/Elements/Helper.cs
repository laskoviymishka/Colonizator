using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Elements
{
	internal static class Helper
	{
		public static bool Minimize(ref int x0, ref int x1)
		{
			if (x0 > x1)
			{
				int temp = x0;
				x0 = x1;
				x1 = temp;

				return true;
			}

			return false;
		}
	}
}
