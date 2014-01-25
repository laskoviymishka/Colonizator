using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Colonizator.Broadcasters;
using GameLogic.Game;
using GameLogic.Search;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using Colonizator.Models;

namespace Colonizator.Hubs
{
    public class MapHub : Hub
    {
        private MapBroadcaster _broadcaster;
        private static readonly ConcurrentDictionary<string, Player> Users = new ConcurrentDictionary<string, Player>();

        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userName, _ => new Player
            {
                PlayerName = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);

                // TODO: Broadcast the connected user
            }

            return base.OnConnected();
        }

        public MapHub()
        {
            _broadcaster = MapBroadcaster.Instance;
        }

        public void JoinGame(string mapId)
        {
            var game = _broadcaster.Games.Find(m => m.Id == mapId);
            if (game != null)
            {
                Groups.Add(Context.ConnectionId, mapId);
            }
        }

        public void ResumeGame(string mapId, string playerName)
        {
            var game = _broadcaster.Games.Find(m => m.Id == mapId) ?? _broadcaster.CreateGame(mapId);
            Player item;
            if (string.IsNullOrEmpty(playerName))
            {
                item = game.Players.FirstOrDefault(p => p.PlayerName == Context.User.Identity.Name);
            }
            else
            {
                item = game.Players.FirstOrDefault(p => p.PlayerName == playerName);
            }

            if (item == null) throw new InvalidOperationException("player not found");

            item.ConnectionIds.Add(Context.ConnectionId);
            Groups.Add(Context.ConnectionId, mapId);
            Clients.Client(Context.ConnectionId).gameStart(new { token = mapId, playerCount = game.Players.IndexOf(item) + 1, player = item });
        }

        public void SearchGame(string playerName)
        {
            var user = Users.GetOrAdd(playerName, _ => new Player
            {
                PlayerName = playerName,
                ConnectionIds = new HashSet<string>()
            });
            user.ConnectionIds.Add(Context.ConnectionId);
            _broadcaster.SearchGame(user);
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