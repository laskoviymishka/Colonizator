using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Colonizator.Hubs
{
    public class MapHub : Hub
    {
        public Task JoinGame(string mapId)
        {
            return Groups.Add(Context.ConnectionId, mapId);
        }

        public void NotificateRoadBuild()
        {

        }
        public void NotificateCityBuild()
        {

        }

        public void NotificateREsourceAdded()
        {

        }
    }
}