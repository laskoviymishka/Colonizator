using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Colonizator.Broadcasters;
using Colonizator.Models;
using GameLogic.Market;
using Model;

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

        public ActionResult MarketPartial(string mapId)
        {
            return PartialView(new List<Order>());
        }

        [HttpGet]
        public ActionResult Map(string token)
        {
            MapController map = GetMap(token);

            return Json(
                map.GetMap().Select(
                    x => new List<HexagonModel>(
                        x.Select(y =>
                            new HexagonModel()
                            {
                                FaceNumber = y.Index > 0 ? y.FaceNumber : 0,
                                ResourceType = y.Index == 10 ? 8 : y.Index > 0 ? y.ResourceType + 2 : y.ResourceType % 2
                            }))).ToList(),
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MapState(string token)
        {
            MapController map = GetMap(token);

            return Json(
                new MapStateModel()
                {
                    Cities = map.Nodes.Select(x =>
                        new CityModel()
                        {
                            HexagonIndex = x.HexagonA.Index,
                            Position = x.OrderA,
                            CitySize = x.CitySize,
                            PlayerId = x.PlayerId
                        }).ToList(),
                    Roads = map.Edges.Select(x =>
                        new RoadModel()
                        {
                            HexagonIndex = x.HexagonA.Index,
                            Position = x.OrderA,
                            PlayerId = x.PlayerId
                        }).ToList(),
                },
                JsonRequestBehavior.AllowGet);
        }

        private MapController GetMap(string token)
        {
            return _broadcaster.GameById(token).MapController;
        }
    }
}