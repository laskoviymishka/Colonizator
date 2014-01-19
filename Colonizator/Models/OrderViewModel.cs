using GameLogic.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Colonizator.Models
{
    public class OrderViewModel
    {
        public string Token { get; set; }
        public string PlayerId { get; set; }
        public string Sell { get; set; }
        public string Buy { get; set; }
    }
}