using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Market;
using GameLogic.Models;
using Model;
using Model.Elements;
using GameLogic.Game.Cards;

namespace GameLogic.Game
{
    public class Game
    {
        #region Private Fields

        private readonly Dictionary<Player, int> _startUpRoads = new Dictionary<Player, int>();
        private readonly Dictionary<Player, int> _startUpTowns = new Dictionary<Player, int>();
        private int _currentPlayerId;
        private bool _isStartUp = true;
        private int _robberHex;

        #endregion

        #region Events

        public event ResourceUpdate ResourceUpdate;
        public event OrderUpdate OrderUpdate;
        public event GameStateUpdate GameMoveUpdate;
        public event DiceThrowen DiceThrowen;
        public event CardPlayed CardPlayed;

        #endregion

        #region Constructor

        public Game(string id, List<Player> players, MapController controller)
        {
            Id = id;
            MapController = controller;
            Players = players;
            CurrentPlayer = Players[_currentPlayerId];

            foreach (Player player in players)
            {
                _startUpRoads.Add(player, 2);
                _startUpTowns.Add(player, 2);
            }

            Market = new Market.Market(this);
            Deck = new Deck(this);
        }

        #endregion

        #region Properties

        public Achive Achive { get; set; }
        public Deck Deck { get; set; }
        public Player CurrentPlayer { get; private set; }
        public string Id { get; set; }
        public List<Player> Players { get; set; }
        public MapController MapController { get; set; }
        public IMarket Market { get; set; }

        public bool IsStartUp
        {
            get { return _isStartUp; }
        }

        public List<CityModel> AvaibleCityBuild
        {
            get
            {
                var result = new List<CityModel>();

                if (_isStartUp)
                {
                    foreach (Node node in MapController.Nodes)
                    {
                        if (node.CitySize == 0)
                        {
                            result.Add(new CityModel
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
                        new CityModel
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

        public List<RoadModel> AvaibleRoadBuild
        {
            get
            {
                return MapController.GetAvailableEdges(_currentPlayerId).Select(x =>
                    new RoadModel
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

        #region Card Actions

        public void ChooseFreeResource(int playerId, ResourceType first, ResourceType second)
        {
            if (CurrentPlayer != Players[playerId])
            {
                throw new InvalidOperationException("Cannot draw card not your move.");
            }
            CurrentPlayer.Resources.First(r => r.Type == first).Qty += 2;
            CurrentPlayer.Resources.First(r => r.Type == second).Qty += 2;
            RiseUpdate(new GameStateUpdateArgs { Action = GameAction.RegularUpdate });
        }

        public void MoveRobber(int playerId, int robberNewPosition)
        {
            if (CurrentPlayer != Players[playerId])
            {
                throw new InvalidOperationException("Cannot draw card not your move.");
            }
            var random = new Random();
            _robberHex = robberNewPosition;
            //foreach (var node in MapController.Nodes)
            //{
            //    if ((node.PlayerId >= 0 && node.PlayerId <= 5) &&
            //        node.HexagonA.Hexagon.Index == _robberHex ||
            //        node.HexagonB.Hexagon.Index == _robberHex ||
            //        node.HexagonB.Hexagon.Index == _robberHex)
            //    {
            //        int resourceType = random.Next(0, 5);
            //        if (node.PlayerId >= 0 && Players[node.PlayerId] != CurrentPlayer)
            //        {
            //            Players[node.PlayerId].Resources.First(
            //                r => r.Type == (ResourceType)(resourceType)).Qty--;
            //            CurrentPlayer.Resources.First(
            //                r => r.Type == (ResourceType)(resourceType)).Qty++;
            //        }
            //    }
            //}
            foreach (var hexagons in MapController.GetMap())
            {
                Hexagon hexagon = hexagons.FirstOrDefault(h => h.Index == _robberHex);
                if (hexagon != null)
                {
                    foreach (Node node in hexagon.Nodes.Where(n => n != null && n.PlayerId >= 0 && n.PlayerId <= 5))
                    {
                        int resourceType = random.Next(0, 5);
                        if (Players[node.PlayerId] != CurrentPlayer)
                        {
                            Players[node.PlayerId].Resources.First(
                                r => r.Type == (ResourceType)(resourceType)).Qty--;
                            CurrentPlayer.Resources.First(
                                r => r.Type == (ResourceType)(resourceType)).Qty++;
                        }
                    }
                }
            }

            RiseUpdate(new GameStateUpdateArgs() { Action = GameAction.RegularUpdate });
        }

        public void PlayCard(int cardId)
        {
            var card = CurrentPlayer.Cards.First(c => c.Id == cardId);
            card.PlayCard();
        }

        public void BuyCard(int playerId)
        {
            if (CurrentPlayer != Players[playerId])
            {
                throw new InvalidOperationException("Cannot draw card not your move.");
            }
            if (CurrentPlayer.Resources.First(r => r.Type == ResourceType.Minerals).Qty <= 0 &&
                CurrentPlayer.Resources.First(r => r.Type == ResourceType.Wool).Qty <= 0 &&
                CurrentPlayer.Resources.First(r => r.Type == ResourceType.Corn).Qty <= 0)
            {
                throw new InvalidOperationException("Cannot draw card not enough resources.");
            }

            Deck.DrawCard(CurrentPlayer);
            CurrentPlayer.Resources.First(r => r.Type == ResourceType.Minerals).Qty--;
            CurrentPlayer.Resources.First(r => r.Type == ResourceType.Wool).Qty--;
            CurrentPlayer.Resources.First(r => r.Type == ResourceType.Corn).Qty--;
            GameMoveUpdate(this, new GameStateUpdateArgs() { Action = GameAction.CardUpdate });
        }

        #endregion

        #region Common Actions

        public void PassMove(string token, int playerId)
        {
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
                RobberTime();
                DiceThrowen(this, new GameStateUpdateArgs { First = cubeValue1, Second = cubeValue2, Action = GameAction.DiceThrowen });
                GameMoveUpdate(this, new GameStateUpdateArgs { Action = GameAction.MoveRobber });
                return result;
            }

            foreach (var hexagons in MapController.GetMap())
            {
                foreach (Hexagon hexagon in hexagons.Where(h => h.ResourceType > 2 && h.Index != _robberHex))
                {
                    if (hexagon.FaceNumber == cubeValue)
                    {
                        foreach (Node node in hexagon.Nodes.Where(n => n != null))
                        {
                            if (node != null && node.PlayerId >= 0 && node.PlayerId <= 5)
                            {
                                Players[node.PlayerId].Resources.First(
                                    r => r.Type == (ResourceType)(hexagon.ResourceType - 3)).Qty += node.CitySize;
                            }
                        }
                    }
                }
            }

            DiceThrowen(this, new GameStateUpdateArgs { First = cubeValue1, Second = cubeValue2, Action = GameAction.DiceThrowen });
            return result;
        }

        public List<CityModel> GetCities()
        {
            var result = new List<CityModel>();
            foreach (Node node in MapController.Nodes)
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
            foreach (Edge edge in MapController.Edges)
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

        #region Build Actions

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
                throw new InvalidOperationException("Illegal move player");
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
                UpgradeCity(token, playerId, hexA, hexB, hexC, hexIndex);
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
                throw new InvalidOperationException("Illegal move player");
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

        #endregion

        #endregion

        #region Helpers

        internal void RiseUpdate(GameStateUpdateArgs args)
        {
            GameMoveUpdate(this, args);
        }

        private Edge GetCommonEdge(Hexagon hexA, Hexagon hexB)
        {
            foreach (Edge edgeA in hexA.Edges)
            {
                if (edgeA != null)
                {
                    foreach (Edge edgeT in hexB.Edges)
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

        private void RobberTime()
        {
            var random = new Random();
            foreach (Player player in Players)
            {
                int resCount = 0;
                foreach (Resource res in player.Resources)
                {
                    resCount += res.Qty;
                }
                while (resCount > 7)
                {
                    Resource res = player.Resources[random.Next(0, 5)];
                    if (res.Qty > 0)
                    {
                        res.Qty--;
                        resCount--;
                    }
                }
            }
        }

        private void NextPlayer()
        {
            if (Players.Count > 0)
            {
                _currentPlayerId = (_currentPlayerId + 1) % Players.Count;
            }
            CurrentPlayer = Players[_currentPlayerId];
            foreach (var player in Players)
            {
                player.PlayerScore = 0;
                int score = 0;
                foreach (var node in MapController.Nodes.Where(n => n.PlayerId == (int)player.Color))
                {
                    score += node.CitySize;
                }
                player.PlayerScore += score;
            }
            GameMoveUpdate(this, new GameStateUpdateArgs() { Action = GameAction.NextMove });
        }

        #endregion

        public void ChooseMonopolyResource(int playerId, ResourceType resourceType)
        {
            throw new NotImplementedException();
        }
    }

    public delegate void OrderUpdate(Game sender, OrderUpdateArgs args);
    public delegate void ResourceUpdate(Game sender, ResourceUpdateArgs args);
    public delegate void GameStateUpdate(Game sender, GameStateUpdateArgs args);
    public delegate void DiceThrowen(Game sender, GameStateUpdateArgs args);
    public delegate void CardPlayed(Game sender, GameStateUpdateArgs args);
}