namespace GameLogic.Game.Cards
{
    public class Road : ICard
    {
        private readonly Game _game;
        public Road(Game game, int id)
        {
            Id = id;
            _game = game;
            CardIndex = 7;
            CardDescription = "Игрок, применяющий эту карточку, может бесплатно построить две дороги";
        }
        public int Id { get; set; }

        public int CardIndex { get; set; }

        public bool IsPlayed { get; set; }

        public string CardDescription { get; set; }

        public void PlayCard()
        {
            Owner.FreeRoadCount += 2;
            IsPlayed = true;
            _game.RiseUpdate(new GameStateUpdateArgs { Action = GameAction.CardUpdate });
        }

        public Player Owner { get; set; }
    }
}