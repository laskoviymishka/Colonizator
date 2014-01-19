using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using Colonizator.Hubs;
using Colonizator.Models;
using GameLogic.Game;
using GameLogic.Search;
using Microsoft.AspNet.SignalR;
using Model;
using GameLogic.Market;

namespace Colonizator.Broadcasters
{
    public class MapBroadcaster
    {
        #region Private fields


        private static MapBroadcaster _instance;
        private List<Game> _games;
        private SearchGameQueue _queue;
        private const string InQueueUsers = "in_queue_users";
        private IHubContext _context;
        private List<Player> Players;

        #endregion

        #region Constructor

        private MapBroadcaster()
        {
            _queue = new SearchGameQueue();
            _queue.UpdateGameQueue += UpdateGameQueue;
            _games = new List<Game>();
            _context = GlobalHost.ConnectionManager.GetHubContext<MapHub>();
            Players = new List<Player>();
        }

        #endregion

        #region Instantiated

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

        #endregion

        public List<Game> Games { get { return _games; } }

        public Game GameById(string mapId)
        {
            if (_games == null)
            {
                _games = new List<Game>();
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

        public Game CreateGame(string mapId)
        {
            var mapControll = new MapController();
            mapControll.Initialize();
            mapControll.StateChanged += delegate(object sender, EventArgs eventArgs)
            {
                _context.Clients.Group(mapId).updateState(
                new MapStateModel()
                {
                    Cities = mapControll.Nodes.Where(x => x.PlayerId >= 0).Select(x =>
                        new CityModel()
                        {
                            HexagonIndex = x.HexagonA.Hexagon.Index,
                            Position = x.HexagonA.Position,
                            CitySize = x.CitySize > 1 ? 't' : 'v',
                            PlayerId = x.PlayerId
                        }).ToList(),
                    Roads = mapControll.Edges.Where(x => x.PlayerId >= 0).Select(x =>
                        new RoadModel()
                        {
                            HexagonIndex = x.HexagonA.Hexagon.Index,
                            Position = x.HexagonA.Position,
                            PlayerId = x.PlayerId
                        }).ToList(),
                },
                JsonRequestBehavior.AllowGet);
            };
            var map = new Game(mapId, Players, mapControll);
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
            player.Resources.Add(new Resource() { Type = ResourceType.Corn, Qty = 1 });
            player.Resources.Add(new Resource() { Type = ResourceType.Wool, Qty = 1 });
            player.Resources.Add(new Resource() { Type = ResourceType.Wood, Qty = 1 });
            player.Resources.Add(new Resource() { Type = ResourceType.Soil, Qty = 1 });

            player.Orders = new ObservableCollection<Order>();

            Players.Add(player);
            var eventArgs = new UpdateGameQueueArgs();
            if (Players.Count == 3)
            {
                eventArgs.Game = CreateGame(Guid.NewGuid().ToString().Substring(0, 6));
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
        }

        public void UpdateGameQueue(object sender, UpdateGameQueueArgs args)
        {
            if (args.Game != null)
            {
                foreach (var arg in args.Game.Players)
                {
                    _context.Groups.Add(arg.PlayerId, args.Game.Id);
                }

                _context.Clients.Group(args.Game.Id).gameStart(args.Game.Id);
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
    }
}