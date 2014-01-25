using System.Collections;
using System.Collections.Generic;
using GameLogic.Game;

namespace GameLogic.Search
{
    public class GameManager : Queue<Player>
    {
        private readonly Queue<Player> _queue;
        private int _queuCount;
        private object _syncObject = new object();
        public event GameCreate GameCreated;
        public event SearchUpdate SearchUpdated;

        public GameManager(int queueCount)
        {
            _queue = new Queue<Player>();
            _queuCount = queueCount;
        }

        public new void Enqueue(Player obj)
        {
            lock (_syncObject)
            {
                _queue.Enqueue(obj);
                if (_queue.Count >= _queuCount)
                {
                    var players = new List<Player>();
                    for (int i = 0; i < _queuCount; i++)
                    {
                        players.Add(_queue.Dequeue());
                    }

                    GameCreated(this, new GameCreateArgs { PLayers = players });
                }
                else
                {
                    SearchUpdated(this, new SearchUpdateArgs { PlayersInQueue = _queue.Count });
                }
            }
        }
    }

    public delegate void SearchUpdate(object sender, SearchUpdateArgs args);

    public delegate void GameCreate(object sender, GameCreateArgs args);
}