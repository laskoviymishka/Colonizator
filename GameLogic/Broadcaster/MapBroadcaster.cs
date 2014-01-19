using System.Collections.Generic;
using GameLogic.Game;

namespace GameLogic.Broadcaster
{
    public class MapBroadcaster
    {
        private static List<Map> _maps;

        static MapBroadcaster()
        {
            _maps = new List<Map>();
        }

        public static List<Map> Maps
        {
            get { return _maps; }
        }

        public static void CreateGame(string gameName)
        {
            _maps.Add(new Map() { Id = gameName });
        }
    }
}