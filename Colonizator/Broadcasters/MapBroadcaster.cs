using System;
using System.Collections.Generic;
using System.Linq;
using Colonizator.Hubs;
using GameLogic.Game;
using GameLogic.Search;
using Microsoft.AspNet.SignalR;
using Model;

namespace Colonizator.Broadcasters
{
    public class MapBroadcaster
    {
        private static MapBroadcaster _instance;
        private List<Map> _games;
        private SearchGameQueue _queue;
        private const string InQueueUsers = "in_queue_users";
        private IHubContext _context;

        private MapBroadcaster()
        {
            _queue = new SearchGameQueue();
            _queue.UpdateGameQueue += UpdateGameQueue;
            _context = GlobalHost.ConnectionManager.GetHubContext<MapHub>();
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
            var map = new Map {Id = mapId, MapController = new MapController()};
            _games.Add(map);
            return map;
        }

        public void SearchGame(string playerId, string playerName)
        {
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
    }
}