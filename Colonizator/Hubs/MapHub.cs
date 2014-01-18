using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLogic.Broadcaster;
using GameLogic.Game;
using Microsoft.AspNet.SignalR;

namespace Colonizator.Hubs
{
    public class MapHub : Hub
    {
        private static SearchGameQueue _queue = new SearchGameQueue();

        public MapHub()
        {
            _queue.UpdateGameQueue += UpdateGameQueue;
        }

        public Task JoinGame(string mapId)
        {
            var map = MapBroadcaster.Maps.Find(m => m.Id == mapId) ?? MapBroadcaster.CreateGame(mapId);

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
            _queue.SearchGame(new Player() { PlayerId = Context.ConnectionId, PlayerName = playerName });
        }

        public void NotificateRoadBuild()
        {

        }
        public void NotificateCityBuild()
        {

        }

        public void NotificateREsourceAdded()
        {

        }

        public void UpdateGameQueue(object sender, UpdateGameQueueArgs args)
        {
            if (args.Map != null && args.Players.Count() > 5)
            {
                foreach (var arg in args.Map.Players)
                {
                    Groups.Add(arg.PlayerId, args.Map.Id);
                }

                Clients.Group(args.Map.Id).gameStart(args.Map);
            }
            else
            {
                foreach (var arg in args.Players)
                {
                    Clients.Client(arg.PlayerId).updateQueue(args.Map);
                }
            }

        }
    }
}