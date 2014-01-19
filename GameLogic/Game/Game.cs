using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Market;
using Model;
using Model.Elements;
using GameLogic.Models;

namespace GameLogic.Game
{
    public class Game
    {
        #region Private Fields

        private int _currentPlayerId = 0;

        #endregion

        #region Events

        public event ResourceUpdate ResourceUpdate;
        public event OrderUpdate OrderUpdate;
        public event GameStateUpdate GameStateUpdate;
        #endregion

        #region Constructor

        public Game(string id, List<Player> players, MapController controller)
        {
            Id = id;
            MapController = controller;
            Players = players;
            foreach (var player in players)
            {
                player.Resources.CollectionChanged += (sender, args) => ResourceUpdate(this, new ResourceUpdateArgs() { Player = player });
                player.Orders.CollectionChanged += (sender, args) => OrderUpdate(this, new OrderUpdateArgs() { Player = player });
            }
            CurrentPlayer = Players[_currentPlayerId];
        }

        #endregion

        #region Properties

        public Player CurrentPlayer
        {
            get;
            private set;
        }

        public string Id { get; set; }
        public List<Player> Players { get; set; }
        public MapController MapController { get; set; }

        #endregion

        #region Game Methods
        public void BuildCity(string token, int playerId, int hexA, int hexB, int hexC, int hexIndex)
        {
            if (Players[playerId] != CurrentPlayer)
            {
                throw new InvalidOperationException("Invalid player");
            }

            if (CurrentPlayer.Resources.All(r => r.Type != ResourceType.Soil)
                || CurrentPlayer.Resources.All(r => r.Type != ResourceType.Wood)
                || CurrentPlayer.Resources.All(r => r.Type != ResourceType.Wool)
                || CurrentPlayer.Resources.All(r => r.Type != ResourceType.Corn))
            {
                throw new InvalidOperationException("not available resource");
            }
            if (!MapController.IsNodeAvailable(hexA, hexB, hexC, playerId, hexIndex))
            {
                throw new InvalidOperationException("Node is not available.");
            }

            MapController.BuildCity(playerId, hexA, hexB, hexC, hexIndex);
            NextPlayer();
        }

        public void BuildRoad(string token, int playerId, int haxagonIndex, int position)
        {
            if (Players[playerId] != CurrentPlayer)
            {
                throw new InvalidOperationException("Invalid player");
            }
            if (!CurrentPlayer.Resources.Any(r => r.Type == ResourceType.Soil)
                || !CurrentPlayer.Resources.Any(r => r.Type == ResourceType.Wood)
                || !CurrentPlayer.Resources.Any(r => r.Type == ResourceType.Wool)
                || !CurrentPlayer.Resources.Any(r => r.Type == ResourceType.Corn))
            {
                throw new InvalidOperationException("not available resource");
            }
            if (!MapController.IsEdgeAvailable(haxagonIndex, position, playerId))
            {
                throw new InvalidOperationException("Edge is not available.");
            }

            MapController.BuildRoad(haxagonIndex, position, playerId);
            NextPlayer();
        }

        public void UpgradeCity(string token, int playerId, int hexA, int hexB, int hexC, int hexIndex)
        {
            if (Players[playerId] != CurrentPlayer)
            {
                throw new InvalidOperationException("Invalid player");
            }

            if (CurrentPlayer.Resources.Count(r => r.Type == ResourceType.Minerals) <= 3
                && CurrentPlayer.Resources.Count(r => r.Type == ResourceType.Corn) <= 2)
            {
                throw new InvalidOperationException("not available resource");
            }
            if (!MapController.IsNodeAvailable(hexA, hexB, hexC, playerId, hexIndex))
            {
                throw new InvalidOperationException("Node is not available.");
            }

            MapController.BuildCity(playerId, hexA, hexB, hexC, hexIndex);
            NextPlayer();
        }

        public void ThrowDice()
        {
            Random random = new Random();

            int cubeValue = random.Next(2, 12);
            if (cubeValue == 7)
            {
                foreach (var player in Players)
                {
                    if (player.Resources.Count > 7)
                    {
                        for (int i = player.Resources.Count; i > 6; i--)
                        {
                            player.Resources.RemoveAt(i);
                        }
                    }
                }
                return;
            }

            foreach (Hexagon[] hexagons in MapController.GetMap())
            {
                foreach (var hexagon in hexagons)
                {
                    if (hexagon.FaceNumber == cubeValue)
                    {
                        foreach (var edge in hexagon.Nodes)
                        {
                            if (edge.PlayerId > 0)
                            {
                                Players[edge.PlayerId].Resources.Add(
                                    new Resource
                                    {
                                        Qty = 1,
                                        Type = (ResourceType)hexagon.ResourceType
                                    });
                            }
                        }
                    }
                }
            }
        }

        public List<CityModel> GetCities()
        {
            var result = new List<CityModel>();
            foreach (var node in MapController.Nodes)
            {
                if (node.CitySize > 0)
                {
                    result.Add(new CityModel
                    {
                        HexagonIndex = node.HexagonA.Hexagon.Index,
                        Position = node.HexagonA.Position,
                        HexA = node.HexagonA.Position,
                        HexB = node.HexagonB.Position,
                        HexC = node.HexagonC.Position,
                        CitySize = node.CitySize > 1 ? 't' : 'v',
                        PlayerId = node.PlayerId
                    });
                }
            }
            return result;
        }
        public List<RoadModel> GetRoads()
        {
            var result = new List<RoadModel>();
            foreach (var edge in MapController.Edges)
            {
                if (edge.PlayerId > 0)
                {
                    result.Add(new RoadModel
                    {
                        HexagonIndex = edge.HexagonA.Hexagon.Index,
                        Position = edge.HexagonA.Position,
                        PlayerId = edge.PlayerId,
                        HexA = edge.HexagonA.Position,
                        HexB = edge.HexagonB.Position
                    });
                }
            }
            return result;
        }

        #endregion

        #region Private Helpers
        private void NextPlayer()
        {
            if (Players.Count > 0)
            {
                _currentPlayerId = (_currentPlayerId + 1) % Players.Count;
            }
            CurrentPlayer = Players[_currentPlayerId];
            GameStateUpdate(this, new GameStateUpdateArgs());
        }
        #endregion
    }

    public delegate void OrderUpdate(Game sender, OrderUpdateArgs args);

    public delegate void ResourceUpdate(Game sender, ResourceUpdateArgs args);
    public delegate void GameStateUpdate(Game sender, GameStateUpdateArgs args);

}
