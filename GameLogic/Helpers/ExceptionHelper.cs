using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Helpers
{
	public static class ExceptionHelper
	{
		public static void ThrowIfNull(object value, string argName)
		{
			if (value == null)
				throw new ArgumentNullException(argName);
		}
		public static void ThrowIfNull(string value, string argName)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentNullException(argName);
		}

		/// <summary>
		/// Throws ArgumentOutOfRangeException when <paramref name="value"/> is less then zero.
		/// </summary>
		/// <param name="value">Value</param>
		/// <param name="argName">Argument name.</param>
		public static void ThrowIfNull(int value, string argName)
		{
			ThrowIfNull(value, -1, argName);
		}

		/// <summary>
		/// Throws ArgumentOutOfRangeException when <paramref name="value"/> less or equal to <paramref name="lowRange"/>
		/// </summary>
		/// <param name="value">Value</param>
		/// <param name="lowRange">Less or equal.</param>
		/// <param name="argName">Argument name.</param>
		public static void ThrowIfNull(int value, int lowRange, string argName)
		{
			if (value <= lowRange)
				throw new ArgumentOutOfRangeException(argName);
		}

		/// <summary>
		/// Throws ArgumentOutOfRangeException when <paramref name="value"/> less or equal to <paramref name="lowRange"/>
		/// or greater than <param name="highRange"/>
		/// </summary>
		/// <param name="value">Value</param>
		/// <param name="lowRange">Less or equal.</param>
		/// <param name="highRange">Greater than.</param>
		/// <param name="argName">Argument name.</param>
		private static void ThrowIfNull(int value, int lowRange, int highRange, string argName)
		{
			if (value <= lowRange || value > highRange)
				throw new ArgumentOutOfRangeException(argName);
		}
	}
}
