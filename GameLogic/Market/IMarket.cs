using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameLogic.Game;

namespace GameLogic.Market
{
    public interface IMarket
    {
        IEnumerable<Order> GetOrders();
        IEnumerable<Order> GetOrders(string playerId);

        event OrderPlaced OrderPlaced;

        void BuildCity(Player player);

        void UpgardeCity(Player player);

        void BuildRoad(Player player);

        void RobberTime(IEnumerable<Player> players);

        bool PlaceOrder(Order order);
        bool AcceptOder(Player acceptedBy, Guid orderId, Player orderOwner);

        void SyncOrders(IEnumerable<Order> orders);
        void ScavengeOrders(IEnumerable<Player> players);
    }
}
