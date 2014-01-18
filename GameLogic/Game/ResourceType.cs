using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Game
{
	public enum ResourceType
	{
		/// <summary>
		/// Лес == Дерево
		/// </summary>
 		Wood = 0, 

		/// <summary>
		/// Пастбище  ==  Шерсть
		/// </summary>
		Wool = 1, 

		/// <summary>
		/// Пашни  == Зерно
		/// </summary>
		Corn = 2,

		/// <summary>
		/// Холмы  ==  Глина
		/// </summary>
		Soil = 3,

		/// <summary>
		/// Горы  ==  Руда
		/// </summary>
		Minerals = 4
	}
}
