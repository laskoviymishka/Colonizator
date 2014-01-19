using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Colonizator.Hubs;
using GameLogic.Game;
using GameLogic.Search;
using Microsoft.AspNet.SignalR;
using Model;
using GameLogic.Market;

namespace Colonizator.Broadcasters
{
    public class MapBroadcaster
    {
        private static MapBroadcaster _instance;
        private List<Map> _games;
        private SearchGameQueue _queue;
        private const string InQueueUsers = "in_queue_users";
        private IHubContext _context;
        private List<Player> Players;
        private MapBroadcaster()
        {
            _queue = new SearchGameQueue();
            _queue.UpdateGameQueue += UpdateGameQueue;
            _games = new List<Map>();
            _context = GlobalHost.ConnectionManager.GetHubContext<MapHub>();
            Players = new List<Player>();
        }

        public static MapBroadcaster Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MapBroadcaster();
                }
                return _instance;
            }
        }


        public List<Map> Games { get { return _games; } }

        public Map GameById(string mapId)
        {
            if (_games == null)
            {
                _games = new List<Map>();
            }
            if (Games.Any(g => g.Id == mapId))
            {
                return Games.First(g => g.Id == mapId);
            }
            else
            {
                return CreateGame(mapId);
            }
        }

        public Map CreateGame(string mapId)
        {
            var mapControll = new MapController();
            mapControll.Initialize();
            mapControll.Randomize();
            mapControll.StateChanged += delegate(object sender, EventArgs eventArgs)
            {
                _context.Clients.Group(mapId).updateState();
            };
            var map = new Map(mapId, Players, mapControll);
            _games.Add(map);
            return map;
        }

        public void SearchGame(string playerId, string playerName)
        {
            Player player = new Player();
            player.PlayerId = playerId;
            player.PlayerName = playerName;
            player.Color = (Color)Players.Count;
            player.Resources = new ObservableCollection<Resource>();
            player.Orders = new ObservableCollection<Order>();

            Players.Add(player);
            var eventArgs = new UpdateGameQueueArgs();
            if (Players.Count == 3)
            {
                eventArgs.Map = CreateGame(Guid.NewGuid().ToString().Substring(0, 6));
                eventArgs.Players = Players;
                Players = new List<Player>();
            }
            else
            {
                eventArgs.Players = Players;
            }
            UpdateGameQueue(this, eventArgs);

            if (_queue.Players.Any(p => p.PlayerId == playerId))
            {
                throw new InvalidOperationException("Cannot added user in game twice");
            }
            _queue.SearchGame(new Player { PlayerId = playerId, PlayerName = playerName });
        }

        public void UpdateGameQueue(object sender, UpdateGameQueueArgs args)
        {
            if (args.Map != null)
            {
                foreach (var arg in args.Map.Players)
                {
                    _context.Groups.Add(arg.PlayerId, args.Map.Id);
                }

                _context.Clients.Group(args.Map.Id).gameStart(args.Map.Id);
            }
            else
            {
                foreach (var arg in args.Players)
                {
                    _context.Groups.Add(arg.PlayerId, InQueueUsers);
                }
                _context.Clients.Group(InQueueUsers).updateQueue(args.Players.Count());
            }
        }

        public bool CanUpdradeCity(string mapId, int userId)
        {
            var game = GameById(mapId);
            var player = game.Players[userId];
            if (player.Resources.Any(r => r.Type == ResourceType.Soil)
                && player.Resources.Any(r => r.Type == ResourceType.Wood))
            {
                return true;
            }
            return false;
        }

        public bool CanBuildRoad(string mapId, int userId)
        {
            var game = GameById(mapId);
            var player = game.Players[userId];
            if (player.Resources.Any(r => r.Type == ResourceType.Soil)
                && player.Resources.Any(r => r.Type == ResourceType.Wood)
                && player.Resources.Any(r => r.Type == ResourceType.Wool)
                && player.Resources.Any(r => r.Type == ResourceType.Corn))
            {
                return true;
            }
            return false;
        }
        public bool CanBuildCity(string mapId, int userId)
        {
            var game = GameById(mapId);
            var player = game.Players[userId];
            if (player.Resources.Count(r => r.Type == ResourceType.Minerals) == 3
                && player.Resources.Count(r => r.Type == ResourceType.Corn) == 2)
            {
                return true;
            }
            return false;
        }
    }
}