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
                BuyResources = buyResources,
                Seller = seller,
                Buyer = buyer,
                HasSellerAcceptance = seller != null,
                HasBuyerAcceptance = buyer != null
            };
        }

        public override string ToString()
        {
            if (HasBuyerAcceptance && HasSellerAcceptance)
            {
                return string.Format("Игрок {0} продал {2} игроку {1} за {3}", Seller.PlayerName, Buyer.PlayerName, SellResourceString(), BuyResourceString());
            }

            if (HasSellerAcceptance)
            {
                return string.Format("Игрок {0} продаст {1} за {2}", Seller.PlayerName, SellResourceString(), BuyResourceString());
            }

            if (HasBuyerAcceptance)
            {
                return string.Format("Игрок {0} купит {2}  за {1}", Buyer.PlayerName, BuyResourceString(), SellResourceString());
            }
            return base.ToString();
        }

        private string SellResourceString()
        {
            var result = new StringBuilder();
            foreach (var sellResource in SellResources.Where(r => r.Qty > 0))
            {
                result.Append(string.Format(", {0} {1} шт.", sellResource.Type, sellResource.Qty));
            }
            return result.ToString();
        }

        private string BuyResourceString()
        {
            var result = new StringBuilder();
            foreach (var sellResource in BuyResources.Where(r => r.Qty > 0))
            {
                result.Append(string.Format(", {0} {1} шт.", sellResource.Type, sellResource.Qty));
            }
            return result.ToString();
        }
    }
}
