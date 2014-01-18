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

        //
        // GET: /Game/
        public ActionResult Index(string id)
        {
            return View();
        }

        public ActionResult All()
        {
            return View(MapBroadcaster.Instance.Maps);
        }

        public ActionResult MarketPartial(string mapId)
        {
            return PartialView(new List<Order>());
        }

        [HttpGet]
		public ActionResult Map(string token)
		{
			token = token ?? string.Empty;

			MapController map;

			if (!_maps.TryGetValue(token, out map))
			{
				map = new MapController();

				map.Initialize();

				_maps.Add(token, map);
			}

			return Json(
				map.GetMap().Select(
					x => new List<HexagonModel>(
						x.Select(y => 
							new HexagonModel() 
							{ 
								FaceNumber = y.FaceNumber,
								ResourceType = y.ResourceType
							}))).ToList(),
				JsonRequestBehavior.AllowGet);
		}
	}
}