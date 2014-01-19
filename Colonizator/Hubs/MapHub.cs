using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colonizator.Broadcasters;
using GameLogic.Game;
using GameLogic.Search;
using Microsoft.AspNet.SignalR;

namespace Colonizator.Hubs
{
    public class MapHub : Hub
    {
        private MapBroadcaster _broadcaster;
        public MapHub()
        {
            _broadcaster = MapBroadcaster.Instance;
        }

        public Task JoinGame(string mapId)
        {
            var map = _broadcaster.Maps.Find(m => m.Id == mapId) ?? _broadcaster.CreateGame(mapId);

            if (!map.Players.Any(p => p.PlayerId == Context.ConnectionId) && map.Players.Count < 5)
            {
                var player = new Player();
                player.PlayerScore = 0;
                player.Resources = new List<Resource>();

                var colors = map.Players.Select(p => p.Color).ToList();

                map.Players.Add(player);
            }
            else
            {
                throw new InvalidOperationException("Cannot join game");
            }
            return Groups.Add(Context.ConnectionId, mapId);
        }

        public void SearchGame(string playerName)
        {
            _broadcaster.SearchGame(Context.ConnectionId, playerName);
        }

        public void NotificateRoadBuild(string mapId, string coord)
        {
            
        }
        public void NotificateCityBuild(string mapId, string coord)
        {

        }

        public void NotificateResourceAdded()
        {

        }
    }
}