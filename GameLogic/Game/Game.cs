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
        private bool _isStartUp = true;
        private Dictionary<Player, int> _startUpTowns = new Dictionary<Player, int>();
        private Dictionary<Player, int> _startUpRoads = new Dictionary<Player, int>();


        #endregion

        #region Events

        public event ResourceUpdate ResourceUpdate;
        public event OrderUpdate OrderUpdate;
        public event GameStateUpdate GameMoveUpdate;
        public event DiceThrowen DiceThrowen;

        #endregion

        #region Constructor

        public Game(string id, List<Player> players, MapController controller)
        {
            Id = id;
            MapController = controller;
            Players = players;
            CurrentPlayer = Players[_currentPlayerId];

            foreach (var player in players)
            {
                _startUpRoads.Add(player, 2);
                _startUpTowns.Add(player, 2);
            }

            Market = new Market.Market();
            Market.PlaceOrder(new Order
            {
                OrderConsumerId = players[0],
                HasConsumerAcceptance = true,
                AcceptedResources = new List<Resource>
                {
                    new Resource { Type = ResourceType.Minerals, Qty = 1 },
                    new Resource { Type = ResourceType.Wool, Qty = 1 },
                },
                ProposedResources = new List<Resource>
                {
                    new Resource { Type = ResourceType.Corn, Qty = 1 },
                    new Resource { Type = ResourceType.Wood, Qty = 1 },
                }
            });

            Market.PlaceOrder(new Order
            {
                OrderOwnerId = players[1],
                HasOwnerAcceptance = true,
                AcceptedResources = new List<Resource>
                {
                    new Resource { Type = ResourceType.Minerals, Qty = 1 },
                    new Resource { Type = ResourceType.Wool, Qty = 1 },
                },
                ProposedResources = new List<Resource>
                {
                    new Resource { Type = ResourceType.Soil, Qty = 1 },
                    new Resource { Type = ResourceType.Wood, Qty = 1 },
                }
            });
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
        public IMarket Market { get; set; }
        public bool IsStartUp { get { return _isStartUp; } }

        public List<CityModel> AvaibleCityBuild
        {
            get
            {
                var result = new List<CityModel>();

                if (_isStartUp)
                {
                    foreach (var node in MapController.Nodes)
                    {
                        if (node.CitySize == 0)
                        {
                            result.Add(new CityModel()
                            {
                                HexagonIndex = node.HexagonA.Hexagon.Index,
                                Position = node.HexagonA.Position,
                                HexA = node.HexagonA.Hexagon.Index,
                                HexB = node.HexagonB.Hexagon.Index,
                                HexC = node.HexagonC.Hexagon.Index,
                                CitySize = 'v',
                                PlayerId = _currentPlayerId
                            });
                        }
                    }
                }
                else
                {
                    result = MapController.GetAvailableNodes(_currentPlayerId).Where(x => x.CitySize < 2).Select(x =>
                      new CityModel()
                      {
                          HexagonIndex = x.HexagonA.Hexagon.Index,
                          Position = x.HexagonA.Position,
                          HexA = x.HexagonA.Hexagon.Index,
                          HexB = x.HexagonB.Hexagon.Index,
                          HexC = x.HexagonC.Hexagon.Index,
                          CitySize = x.CitySize > 0 ? 't' : 'v',
                          PlayerId = _currentPlayerId
                      }).ToList();
                }
                return result;
            }
        }
        private Edge GetCommonEdge(Hexagon hexA, Hexagon hexB)
        {
            foreach (var edgeA in hexA.Edges)
            {
                if (edgeA != null)
                {
                    foreach (var edgeT in hexB.Edges)
                    {
                        if (edgeA == edgeT)
                        {
                            return edgeA;
                        }
                    }
                }
            }
            return null;
        }
        public List<RoadModel> AvaibleRoadBuild
        {
            get
            {
                var result = new HashSet<Edge>();

                foreach (var edge in MapController.Edges)
                {
                    if (edge.PlayerId == _currentPlayerId)
                    {
                        result.Add(GetCommonEdge(edge.HexagonA.Hexagon, edge.HexagonBot.Hexagon));
                        result.Add(GetCommonEdge(edge.HexagonA.Hexagon, edge.HexagonB.Hexagon));
                        result.Add(GetCommonEdge(edge.HexagonA.Hexagon, edge.HexagonTop.Hexagon));
                        result.Add(GetCommonEdge(edge.HexagonB.Hexagon, edge.HexagonBot.Hexagon));
                        result.Add(GetCommonEdge(edge.HexagonB.Hexagon, edge.HexagonTop.Hexagon));
                    }
                }

                foreach (var edge in MapController.GetAvailableEdges(_currentPlayerId)) { result.Add(edge); }
                result.RemoveWhere(x => x == null);
                return MapController.Edges.Select(x =>
                        new RoadModel()
                        {
                            HexagonIndex = x.HexagonA.Hexagon.Index,
                            Position = x.HexagonA.Position,
                            PlayerId = _currentPlayerId,
                            HexA = x.HexagonA.Hexagon.Index,
                            HexB = x.HexagonB.Hexagon.Index
                        }).ToList();
            }
        }


        #endregion

        #region Game Methods
        public void BuildCity(string token, int playerId, int hexA, int hexB, int hexC, int hexIndex)
        {
            if (_isStartUp)
            {
                if (_startUpTowns[CurrentPlayer] > 0
                    && _startUpTowns[CurrentPlayer] == _startUpRoads[CurrentPlayer])
                {
                    MapController.BuildCity(playerId, hexA, hexB, hexC, hexIndex);
                    _startUpTowns[CurrentPlayer]--;
                    GameMoveUpdate(this, new GameStateUpdateArgs());
                    return;
                }
                else
                {
                    throw new InvalidOperationException("Illegal move player");
                }
            }
            if (Players[playerId] != CurrentPlayer)
            {
                throw new InvalidOperationException("Invalid player");
            }

            if (!MapController.IsNodeAvailable(hexA, hexB, hexC, playerId, hexIndex))
            {
                if (!MapController.IsUpgradeTown(hexA, hexB, hexC, playerId, hexIndex))
                {
                    throw new InvalidOperationException("Node is not available.");
                }
                else
                {
                    this.UpgradeCity(token, playerId, hexA, hexB, hexC, hexIndex);
                }
            }
            else
            {
                Market.BuildCity(Players[playerId]);
                MapController.BuildCity(playerId, hexA, hexB, hexC, hexIndex);
                CurrentPlayer.PlayerScore++;
                NextPlayer();
            }
        }

        public void BuildRoad(string token, int playerId, int haxagonIndex, int hexA, int hexB)
        {
            if (_isStartUp)
            {
                if (_startUpRoads[CurrentPlayer] > 0
                    && _startUpTowns[CurrentPlayer] < _startUpRoads[CurrentPlayer]
                    && _startUpTowns[CurrentPlayer] == (_startUpRoads[CurrentPlayer] - 1))
                {
                    MapController.BuildRoad(haxagonIndex, hexA, hexB, playerId);
                    _startUpRoads[CurrentPlayer]--;
                    if (!_startUpRoads.Any(k => k.Value != 0))
                    {
                        _isStartUp = false;
                    }
                    NextPlayer();
                    return;
                }
                else
                {
                    throw new InvalidOperationException("Illegal move player");
                }
            }
            if (Players[playerId] != CurrentPlayer)
            {
                throw new InvalidOperationException("Invalid player");
            }

            if (Players[playerId] != CurrentPlayer)
            {
                throw new InvalidOperationException("Invalid player");
            }
            if (!MapController.IsEdgeAvailable(haxagonIndex, hexA, hexB))
            {
                throw new InvalidOperationException("Edge is not available.");
            }

            Market.BuildRoad(Players[playerId]);
            MapController.BuildRoad(haxagonIndex, hexA, hexB, playerId);
            NextPlayer();
        }

        public void UpgradeCity(string token, int playerId, int hexA, int hexB, int hexC, int hexIndex)
        {
            if (Players[playerId] != CurrentPlayer)
            {
                throw new InvalidOperationException("Invalid player");
            }

            Market.UpgardeCity(Players[playerId]);
            MapController.BuildCity(playerId, hexA, hexB, hexC, hexIndex);
            CurrentPlayer.PlayerScore++;
            NextPlayer();
        }

        public List<int> ThrowDice()
        {
            var random = new Random();
            int cubeValue1 = random.Next(1, 7);
            int cubeValue2 = random.Next(1, 7);
            var result = new List<int>();
            result.Add(cubeValue1);
            result.Add(cubeValue2);
            int cubeValue = cubeValue1 + cubeValue2;
            if (cubeValue == 7)
            {
                foreach (var player in Players)
                {
                    int resCount = 0;
                    foreach (var res in player.Resources)
                    {
                        resCount += res.Qty;
                    }
                    while (resCount > 7)
                    {
                        var res = player.Resources[random.Next(0, 5)];
                        if (res.Qty > 0)
                        {
                            res.Qty--;
                            resCount--;
                        }
                    }
                }
                DiceThrowen(this, new GameStateUpdateArgs { First = cubeValue1, Second = cubeValue2 });
                return result;
            }

            foreach (Hexagon[] hexagons in MapController.GetMap())
            {
                foreach (var hexagon in hexagons.Where(h => h.ResourceType > 2))
                {
                    if (hexagon.FaceNumber == cubeValue)
                    {
                        foreach (var node in hexagon.Nodes)
                        {
                            if (node.PlayerId >= 0 && node.PlayerId <= 5)
                            {
                                Players[node.PlayerId].Resources.First(r => r.Type == (ResourceType)(hexagon.ResourceType - 3)).Qty += node.CitySize;
                            }
                        }
                    }
                }
            }

            DiceThrowen(this, new GameStateUpdateArgs { First = cubeValue1, Second = cubeValue2 });
            return result;
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
                        HexA = node.HexagonA.Hexagon.Index,
                        HexB = node.HexagonB.Hexagon.Index,
                        HexC = node.HexagonC.Hexagon.Index,
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
                if (edge.PlayerId >= 0 && edge.PlayerId < 5)
                {
                    result.Add(new RoadModel
                    {
                        HexagonIndex = edge.HexagonA.Hexagon.Index,
                        Position = edge.HexagonA.Position,
                        PlayerId = edge.PlayerId,
                        HexA = edge.HexagonA.Hexagon.Index,
                        HexB = edge.HexagonB.Hexagon.Index
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
            GameMoveUpdate(this, new GameStateUpdateArgs());
        }
        #endregion

        public void PassMove(string token, int playerId)
        {
            NextPlayer();
        }
    }

    public delegate void OrderUpdate(Game sender, OrderUpdateArgs args);
    public delegate void ResourceUpdate(Game sender, ResourceUpdateArgs args);
    public delegate void GameStateUpdate(Game sender, GameStateUpdateArgs args);
    public delegate void DiceThrowen(Game sender, GameStateUpdateArgs args);
}
