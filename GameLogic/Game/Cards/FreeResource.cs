namespace GameLogic.Game.Cards
{
    public class FreeResource : ICard
    {
        private readonly Game _game;
        public FreeResource(Game game, int id)
        {
            Id = id;
            _game = game;
            CardIndex = 6;
            CardDescription = "Игрок немедленно получает в своё распоряжение две карточки ресурсов на свой выбор";
        }
        public int Id { get; set; }

        public int CardIndex { get; set; }

        public bool IsPlayed { get; set; }

        public string CardDescription { get; set; }

        public void PlayCard()
        {
            IsPlayed = true;
            _game.RiseUpdate(new GameStateUpdateArgs { Action = GameAction.FreeResource });
        }

        public Player Owner { get; set; }
    }
}