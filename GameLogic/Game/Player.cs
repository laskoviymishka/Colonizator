using System.Collections.Generic;

namespace GameLogic.Game
{
    public class Player
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int PlayerScore { get; set; }
        public Color Color { get; set; }
        public List<Resource> Resources { get; set; } 
    }
}