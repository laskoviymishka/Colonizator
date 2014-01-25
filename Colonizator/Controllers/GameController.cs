using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Colonizator.Broadcasters;
using Colonizator.Models;
using GameLogic.Game;
using GameLogic.Market;
using GameLogic.Model;
using GameLogic.Models;
using Model;

namespace Colonizator.Controllers
{
    public class GameController : Controller
    {
        #region Private Fields

        private static readonly Dictionary<string, MapController> _maps = new Dictionary<string, MapController>();
        private readonly MapBroadcaster _broadcaster = MapBroadcaster.Instance;

        #endregion

        #region Main Actions

        public ActionResult Index(string id)
        {
            return View();
        }

        public ActionResult All()
        {
            return View(MapBroadcaster.Instance.Games);
        }

        #endregion

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
            Game game = GetGame(token);
            ViewBag.CurrentUser = game.Players[playerId];
            ViewBag.MoveUser = game.CurrentPlayer;
            return PartialView(game.Market.GetOrders(game.Players[playerId]));
        }

        public ActionResult DeckPartial(string token, int playerId)
        {
            Game game = GetGame(token);
            ViewBag.CurrentUser = game.Players[playerId];
            ViewBag.MoveUser = game.CurrentPlayer;
            return PartialView(game.Players[playerId]);
        }

        #endregion

        #region Map State

        [HttpGet]
        public ActionResult RobberPosition(string token, int playerId)
        {
            Game game = GetMap(token);
            return Json(
                game.RobberPosition,
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AvailableMap(string token, int playerId)
        {
            Game game = GetMap(token);

            if (game.Players[playerId] != game.CurrentPlayer)
            {
                return Json(
                    new MapStateModel
                    {
                        Cities = new List<CityModel>(),
                        Roads = new List<RoadModel>()
                    },
                    JsonRequestBehavior.AllowGet);
            }

            MapController map = game.MapController;

            return Json(
                new MapStateModel
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
                            new HexagonModel
                            {
                                FaceNumber = y.Index > 0 ? y.FaceNumber : 0,
                                ResourceType = y.ResourceType,
                                HexagonIndex = y.Index
                            }))).ToList(),
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MapState(string token)
        {
            MapController map = GetMap(token).MapController;
            var model = new MapStateModel
            {
                Cities = map.Nodes.Where(x => x.PlayerId >= 0).Select(x =>
                    new CityModel
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
                    new RoadModel
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
            var model = new MapStateModel
            {
                Cities = map.GetCities(),
                Roads = map.GetRoads(),
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Map Actions

        #region Card Actions

        [HttpPost]
        public void PlayCard(string token, int playerId, int cardId)
        {
            Game game = GetMap(token);
            try
            {
                game.PlayCard(cardId, playerId);
            }
            catch (Exception exception)
            {
                game.RiseToast(new ToasterUpdateArgs
                {
                    Body = exception.Message,
                    Title = "Что-то определенно пошло не так",
                    Type = ToastType.Error
                });
            }
        }

        [HttpPost]
        public void DrawCard(string token, int playerId)
        {
            Game game = GetMap(token);
            try
            {
                game.BuyCard(playerId);
            }
            catch (Exception exception)
            {
                game.RiseToast(new ToasterUpdateArgs
                {
                    Body = exception.Message,
                    Title = "Что-то определенно пошло не так",
                    Type = ToastType.Error
                });
            }
        }

        [HttpPost]
        public void MoveRobber(string token, int playerId, int hexagonIngex)
        {
            Game game = GetMap(token);

            try
            {
                game.MoveRobber(playerId, hexagonIngex);
            }
            catch (Exception exception)
            {
                game.RiseToast(new ToasterUpdateArgs
                {
                    Body = exception.Message,
                    Title = "Что-то определенно пошло не так",
                    Type = ToastType.Error
                });
            }
        }

        [HttpPost]
        public void ChooseFreeResource(string token, int playerId, int first, int second)
        {
            Game game = GetMap(token);
            try
            {
                game.ChooseFreeResource(playerId, (ResourceType)first, (ResourceType)second);
            }
            catch (Exception exception)
            {
                game.RiseToast(new ToasterUpdateArgs
                {
                    Body = exception.Message,
                    Title = "Что-то определенно пошло не так",
                    Type = ToastType.Error
                });
            }
        }

        [HttpPost]
        public void ChooseMonopolyResource(string token, int playerId, int resource)
        {
            Game game = GetMap(token);
            try
            {
                game.ChooseMonopolyResource(playerId, (ResourceType)resource);
            }
            catch (Exception exception)
            {
                game.RiseToast(new ToasterUpdateArgs
                {
                    Body = exception.Message,
                    Title = "Что-то определенно пошло не так",
                    Type = ToastType.Error
                });
            }
        }

        #endregion

        #region Common Actions

        [HttpGet]
        public ActionResult ThrowDice(string token, int playerId)
        {
            Game game = GetGame(token);
            List<int> model = game.ThrowDice();
            return Json(new DiceViewModel { First = model[0], Second = model[1] }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void PassMove(string token, int playerId)
        {
            Game game = GetMap(token);
            game.PassMove(token, playerId);
        }

        #endregion

        #region Build Actions

        [HttpPost]
        public void BuildCity(string token, int playerId, int hexA, int hexB, int hexC, int hexIndex)
        {
            Game game = GetMap(token);
            try
            {
                game.BuildCity(token, playerId, hexA, hexB, hexC, hexIndex);
            }
            catch (Exception exception)
            {
                game.RiseToast(new ToasterUpdateArgs
                {
                    Body = exception.Message,
                    Title = "Что-то определенно пошло не так",
                    Type = ToastType.Error
                });
            }

        }

        [HttpPost]
        public void BuildRoad(string token, int playerId, int haxagonIndex, int hexA, int hexB)
        {
            Game game = GetMap(token);
            try
            {
                game.BuildRoad(token, playerId, haxagonIndex, hexA, hexB);
            }
            catch (Exception exception)
            {
                game.RiseToast(new ToasterUpdateArgs
                {
                    Body = exception.Message,
                    Title = "Что-то определенно пошло не так",
                    Type = ToastType.Error
                });
            }

        }

        #endregion

        #region Market Actions

        [HttpGet]
        public ActionResult GetTestOrder()
        {
            var order = new OrderViewModel
            {
                Buy = new OrderBatch { Corn = 1, Soil = 2, Wood = 3, Minerals = 0, Wool = 0 },
                PlayerId = 0,
                Token = "asd",
                Sell = new OrderBatch { Minerals = 4, Soil = 0, Wood = 0, Corn = 0, Wool = 1 }
            };
            return Json(order, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void SellOrder(OrderViewModel order)
        {
            Game game = GetGame(order.Token);
            var sellerList = new List<Resource>
            {
                new Resource {Type = ResourceType.Corn, Qty = order.Sell.Corn},
                new Resource {Type = ResourceType.Minerals, Qty = order.Sell.Minerals},
                new Resource {Type = ResourceType.Soil, Qty = order.Sell.Soil},
                new Resource {Type = ResourceType.Wood, Qty = order.Sell.Wood},
                new Resource {Type = ResourceType.Wool, Qty = order.Sell.Wool}
            };

            var buyerList = new List<Resource>
            {
                new Resource {Type = ResourceType.Corn, Qty = order.Buy.Corn},
                new Resource {Type = ResourceType.Minerals, Qty = order.Buy.Minerals},
                new Resource {Type = ResourceType.Soil, Qty = order.Buy.Soil},
                new Resource {Type = ResourceType.Wood, Qty = order.Buy.Wood},
                new Resource {Type = ResourceType.Wool, Qty = order.Buy.Wool}
            };
            try
            {
                game.Market.PlaceOrder(Order.CreatePropose(sellerList, buyerList, game.Players[order.PlayerId], null));
            }
            catch (Exception exception)
            {
                game.RiseToast(new ToasterUpdateArgs
                {
                    Body = exception.Message,
                    Title = "Что-то определенно пошло не так",
                    Type = ToastType.Error
                });
            }
        }

        [HttpPost]
        public void BuyOrder(OrderViewModel order)
        {
            Game game = GetGame(order.Token);
            var sellerList = new List<Resource>
            {
                new Resource {Type = ResourceType.Corn, Qty = order.Sell.Corn},
                new Resource {Type = ResourceType.Minerals, Qty = order.Sell.Minerals},
                new Resource {Type = ResourceType.Soil, Qty = order.Sell.Soil},
                new Resource {Type = ResourceType.Wood, Qty = order.Sell.Wood},
                new Resource {Type = ResourceType.Wool, Qty = order.Sell.Wool}
            };

            var buyerList = new List<Resource>
            {
                new Resource {Type = ResourceType.Corn, Qty = order.Buy.Corn},
                new Resource {Type = ResourceType.Minerals, Qty = order.Buy.Minerals},
                new Resource {Type = ResourceType.Soil, Qty = order.Buy.Soil},
                new Resource {Type = ResourceType.Wood, Qty = order.Buy.Wood},
                new Resource {Type = ResourceType.Wool, Qty = order.Buy.Wool}
            };
            try
            {
                game.Market.PlaceOrder(Order.CreatePropose(sellerList, buyerList, null, game.Players[order.PlayerId]));
            }
            catch (Exception exception)
            {
                game.RiseToast(new ToasterUpdateArgs
                {
                    Body = exception.Message,
                    Title = "Что-то определенно пошло не так",
                    Type = ToastType.Error
                });
            }
        }

        [HttpPost]
        public void ProcessBuy(string token, int userId, string orderId)
        {
            Guid id = Guid.Parse(orderId);
            Game game = GetGame(token);
            Order order = game.Market.GetOrder(id);
            order.Buyer = game.Players[userId];
            order.HasBuyerAcceptance = true;
            try
            {
                game.Market.AcceptOder(id);
            }
            catch (Exception exception)
            {
                game.RiseToast(new ToasterUpdateArgs
                {
                    Body = exception.Message,
                    Title = "Что-то определенно пошло не так",
                    Type = ToastType.Error
                });
            }
        }

        [HttpPost]
        public void ProcessSell(string token, int userId, string orderId)
        {
            Guid id = Guid.Parse(orderId);
            Game game = GetGame(token);
            Order order = game.Market.GetOrder(id);
            order.Seller = game.Players[userId];
            order.HasSellerAcceptance = true;
            try
            {
                game.Market.AcceptOder(id);
            }
            catch (Exception exception)
            {
                game.RiseToast(new ToasterUpdateArgs
                {
                    Body = exception.Message,
                    Title = "Что-то определенно пошло не так",
                    Type = ToastType.Error
                });
            }
        }

        #endregion

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