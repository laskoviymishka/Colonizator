namespace GameLogic.Game.Cards
{
    public class WinPoint : ICard
    {
        private readonly Game _game;
        public WinPoint(Game game, int id, int cardIndex, string cardDescription)
        {
            Id = id;
            _game = game;
            CardIndex = cardIndex;
            CardDescription = cardDescription;
        }
        public int Id { get; set; }

        public int CardIndex { get; set; }

        public bool IsPlayed { get; set; }

        public string CardDescription { get; set; }

        public void PlayCard()
        {
            Owner.PlayerScore++;
            IsPlayed = true;
            _game.RiseUpdate(new GameStateUpdateArgs { Action = GameAction.CardUpdate });
        }

        public Player Owner { get; set; }
    }
}