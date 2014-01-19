﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Colonizator.Broadcasters;
using Colonizator.Models;
using GameLogic.Game;
using GameLogic.Market;
using Model;
using GameLogic.Models;
using GameLogic.Model;
using Newtonsoft.Json;

namespace Colonizator.Controllers
{
    public class GameController : Controller
    {
        private static readonly Dictionary<string, MapController> _maps = new Dictionary<string, MapController>();
        private readonly MapBroadcaster _broadcaster = MapBroadcaster.Instance;
        //
        // GET: /Game/
        public ActionResult Index(string id)
        {
            return View();
        }

        public ActionResult All()
        {
            return View(MapBroadcaster.Instance.Games);
        }

        #region Partials

        public ActionResult GameStatePartial(string token, int playerId)
        {
            var model = new GameStateViewModel();
            model.Game = GetGame(token);
            model.CurrentPlayer = model.Game.Players[playerId];
            model.PlayerId = playerId;
            return PartialView(model);
        }
        public ActionResult MarketPartial(string token, int playerId)
        {
            var game = GetGame(token);
            ViewBag.IsAvaibleAction = game.CurrentPlayer == game.Players[playerId];
            return PartialView(game.Market.GetOrders());
        }

        #endregion

        #region Map State
        [HttpGet]
        public ActionResult AvailableMap(string token, int playerId)
        {
            Game game = GetMap(token);

            if (game.Players[playerId] != game.CurrentPlayer)
            {
                return Json(
                    new MapStateModel()
                    {
                        Cities = new List<CityModel>(),
                        Roads = new List<RoadModel>()
                    },
                    JsonRequestBehavior.AllowGet);
            }

            MapController map = game.MapController;

            return Json(
                new MapStateModel()
                {
                    Cities = game.AvaibleCityBuild,
                    Roads = game.AvaibleRoadBuild,
                },
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Map(string token)
        {
            MapController map = GetMap(token).MapController;

            return Json(
                map.GetMap().Select(
                    x => new List<HexagonModel>(
                        x.Select(y =>
                            new HexagonModel()
                            {
                                FaceNumber = y.Index > 0 ? y.FaceNumber : 0,
                                ResourceType = y.ResourceType
                            }))).ToList(),
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MapState(string token)
        {
            MapController map = GetMap(token).MapController;
            var model = new MapStateModel()
                {
                    Cities = map.Nodes.Where(x => x.PlayerId >= 0).Select(x =>
                        new CityModel()
                        {
                            HexagonIndex = x.HexagonA.Hexagon.Index,
                            Position = x.HexagonA.Position,
                            HexA = x.HexagonA.Hexagon.Index,
                            HexB = x.HexagonB.Hexagon.Index,
                            HexC = x.HexagonC.Hexagon.Index,
                            CitySize = x.CitySize > 1 ? 't' : 'v',
                            PlayerId = x.PlayerId
                        }).ToList(),
                    Roads = map.Edges.Where(x => x.PlayerId >= 0).Select(x =>
                        new RoadModel()
                        {
                            HexagonIndex = x.HexagonA.Hexagon.Index,
                            Position = x.HexagonA.Position,
                            PlayerId = x.PlayerId,
                            HexA = x.HexagonA.Hexagon.Index,
                            HexB = x.HexagonB.Hexagon.Index
                        }).ToList(),
                };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CitiesAndRoads(string token)
        {
            Game map = GetMap(token);
            var model = new MapStateModel()
            {
                Cities = map.GetCities(),
                Roads = map.GetRoads(),
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Map Actions

        [HttpGet]
        public ActionResult ThrowDice(string token, int playerId)
        {
            Game game = GetGame(token);
            var model = game.ThrowDice();
            return Json(new DiceViewModel { First = model[0], Second = model[1] }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void BuildCity(string token, int playerId, int hexA, int hexB, int hexC, int hexIndex)
        {
            Game game = GetMap(token);
            game.BuildCity(token, playerId, hexA, hexB, hexC, hexIndex);
        }

        [HttpPost]
        public void BuildRoad(string token, int playerId, int haxagonIndex, int hexA, int hexB)
        {
            Game game = GetMap(token);
            game.BuildRoad(token, playerId, haxagonIndex, hexA, hexB);
        }

        [HttpPost]
        public void PassMove(string token, int playerId)
        {
            Game game = GetMap(token);
            game.PassMove(token, playerId);
        }

        [HttpPost]
        public void RequestOrder(OrderViewModel order)
        {
            var t = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, int>>>(order.Buy);
            Game game = GetMap(order.Token);
        }

        #endregion

        #region Private Heplers

        private Game GetMap(string token)
        {
            return _broadcaster.GameById(token);
        }

        private Game GetGame(string token)
        {
            return _broadcaster.GameById(token);
        }

        #endregion
    }
}