using GameLogic.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Colonizator.Models
{
    public class GameStateViewModel
    {
        public Player CurrentPlayer { get; set; }
        public int PlayerId { get; set; }
        public Game Game { get; set; }
    }
}