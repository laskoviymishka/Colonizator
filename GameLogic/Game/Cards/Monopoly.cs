namespace GameLogic.Game.Cards
{
    public class Monopoly : ICard
    {
        private readonly Game _game;
        public Monopoly(Game game, int id)
        {
            Id = id;
            _game = game;
            CardIndex = 8;
            CardDescription = " Играя эту карточку, игрок выбирает один тип ресурса. Остальные игроки должны отдать ему все карточки ресурсов этого типа;";
        }
        public int Id { get; set; }

        public int CardIndex { get; set; }

        public bool IsPlayed { get; set; }

        public string CardDescription { get; set; }

        public void PlayCard()
        {
            IsPlayed = true;
            _game.RiseUpdate(new GameStateUpdateArgs { Action = GameAction.Monopoly });
        }

        public Player Owner { get; set; }
    }
}