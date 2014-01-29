using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Colonizator.Broadcasters;
using Colonizator.Models;
using GameLogic.Game;
using GameLogic.Models;
using Model;

namespace Colonizator.Controllers
{
	public class GameApiController : ApiController
	{
		private readonly MapBroadcaster _broadcaster = MapBroadcaster.Instance;

		public AngularModel GetState(string tokenId, int playerId)
		{
			var game = _broadcaster.Games.Find(g => g.Id == tokenId);
			var player = game.Players[playerId];
			var viewModel = new AngularModel();
			viewModel.Orders = new List<OrderAngularModel>();
			foreach (var order in game.Market.GetOrders(player))
			{
				((ICollection<OrderAngularModel>)viewModel.Orders).Add(new OrderAngularModel()
				{
					BuyList = order.BuyResources,
					SellList = order.SellResources,
					Buyer = (order.Buyer == null) ? "" : order.Buyer.PlayerName,
					Seller = (order.Seller == null) ? "" : order.Seller.PlayerName
				});
			}
			viewModel.CurrentPlayer = player;
			if (game.CurrentPlayer == player)
			{
				viewModel.MapState = new MapStateModel
				{
					Cities = game.AvaibleCityBuild,
					Roads = game.AvaibleRoadBuild,
				};
			}
			else
			{
				viewModel.MapState = new MapStateModel();
			}

			return viewModel;
		}

		[System.Web.Http.HttpGet]
		public List<List<HexagonModel>> Map(string tokenId)
		{
			MapController map = _broadcaster.Games.Find(g => g.Id == tokenId).MapController;

			return
				map.GetMap().Select(
					x => new List<HexagonModel>(
						x.Select(y =>
							new HexagonModel
							{
								FaceNumber = y.Index > 0 ? y.FaceNumber : 0,
								ResourceType = y.ResourceType,
								HexagonIndex = y.Index
							}))).ToList();
		}

		[System.Web.Http.HttpGet]
		public MapStateModel MapViewState(string token, int playerId)
		{
			Game map = _broadcaster.Games.Find(g => g.Id == token);
			if (map.Players[playerId] == map.CurrentPlayer)
			{
				return new MapStateModel
				{
					Cities = map.GetCities(),
					Roads = map.GetRoads(),
					PossibleCities = map.AvaibleCityBuild,
					PossibleRoads = map.AvaibleRoadBuild
				};
			}

			return new MapStateModel
			{
				Cities = map.GetCities(),
				Roads = map.GetRoads()
			};
		}
	}
}
