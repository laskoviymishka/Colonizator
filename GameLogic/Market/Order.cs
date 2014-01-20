using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameLogic.Game;

namespace GameLogic.Market
{
    public class Order
    {
        public Order()
        {
            BuyResources = new List<Resource>();
            SellResources = new List<Resource>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }

        public List<Resource> SellResources { get; set; }

        public Player Seller { get; set; }
        public Player Buyer { get; set; }

        public bool HasSellerAcceptance { get; set; }
        public bool HasBuyerAcceptance { get; set; }

        public List<Resource> BuyResources { get; set; }

        public static Order CreatePropose(List<Resource> sellResources, List<Resource> buyResources, Player seller, Player buyer)
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                SellResources = sellResources,
                BuyResources =  buyResources,
                Seller =  seller,
                Buyer =  buyer,
                HasSellerAcceptance = seller != null,
                HasBuyerAcceptance = buyer != null
            };
        }
    }
}
