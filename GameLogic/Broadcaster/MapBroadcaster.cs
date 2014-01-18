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

        public static Map CreateGame(string gameName)
        {
            var map = new Map() {Id = gameName};
            _maps.Add(map);
            return map;
        }
    }
}