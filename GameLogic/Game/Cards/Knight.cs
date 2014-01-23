namespace GameLogic.Game.Cards
{
    public class Knight : ICard
    {
        private readonly Game _game;
        public Knight(Game game, int id)
        {
            Id = id;
            _game = game;
            CardIndex = 0;
            CardDescription = "Сыграв карточку «Рыцарь» игрок имеет право переставить фишку «Разбойник» на другую карточку «Суша» (за исключением Пустыни) и вытянуть карточку у одного из игроков, имеющих поселение или город на этой карточке";
        }

        public int Id { get; set; }

        public int CardIndex { get; set; }

        public bool IsPlayed { get; set; }

        public string CardDescription { get; set; }

        public void PlayCard()
        {
            IsPlayed = true;
            _game.RiseUpdate(new GameStateUpdateArgs { Action = GameAction.MoveRobber });
        }

        public Player Owner { get; set; }
    }
}