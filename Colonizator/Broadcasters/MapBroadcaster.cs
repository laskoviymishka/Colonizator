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
using GameLogic.Models;

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
            var game = new Game(mapId, Players, mapControll);
            game.GameMoveUpdate += game_GameStateUpdate;
            game.DiceThrowen += game_DiceThrowen;
            _games.Add(game);
            return game;
        }

        void game_DiceThrowen(Game sender, GameStateUpdateArgs args)
        {
            _context.Clients.Group(sender.Id).throwenDice(args.DiceNumber);
        }

        private void game_GameStateUpdate(Game sender, GameStateUpdateArgs args)
        {
            _context.Clients.Group(sender.Id).updateState(sender.CurrentPlayer);
        }

        public void SearchGame(string playerId, string playerName)
        {
            Player player = new Player();
            player.PlayerId = playerId;
            player.PlayerName = playerName;
            player.Color = (Color)Players.Count;
            player.Resources = new ObservableCollection<Resource>();
            player.Resources.Add(new Resource() { Type = ResourceType.Corn, Qty = 10 });
            player.Resources.Add(new Resource() { Type = ResourceType.Wool, Qty = 10 });
            player.Resources.Add(new Resource() { Type = ResourceType.Wood, Qty = 10 });
            player.Resources.Add(new Resource() { Type = ResourceType.Soil, Qty = 10 });
            player.Resources.Add(new Resource() { Type = ResourceType.Minerals, Qty = 10 });

            player.Orders = new ObservableCollection<Order>();

            Players.Add(player);
            var eventArgs = new UpdateGameQueueArgs();
            if (Players.Count == 3)
            {
                eventArgs.Players = Players;
                eventArgs.Game = CreateGame(Guid.NewGuid().ToString().Substring(0, 6));
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
                    _context.Groups.Remove(arg.PlayerId, InQueueUsers);
                }

                _context.Clients.Group(args.Game.Id).gameStart(new { token = args.Game.Id, playerCount = args.Game.Players.Count, player = args.Game.Players.Last() });
            }
            else
            {
                foreach (var arg in args.Players)
                {
                    _context.Clients.Client(arg.PlayerId).updateQueue(new { playerCount = args.Players.Count(), player = args.Players.Last() });
                }
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