using System.Collections.Concurrent;

namespace DummyClient;

public class GameRoom
{
    public int Id { get; set; }
    public string displayName { get; set; }
    public int playerCount { get; set; }

    private OrderedDictionary<int, Player> _players = new OrderedDictionary<int, Player>();
    private ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
    private object _lock = new object();

    public void AddMessage(string message)
    {
        lock (_lock)
        {
            _messageQueue.Enqueue(message);
        }
    }

    public bool TryDequeueMessage(out string message)
    {
        lock (_lock)
        {
            return _messageQueue.TryDequeue(out message);
        }
    }
    
    public Player GetOwner()
    {
        // 가장 먼저 들어오는 사람이 오너.
        // 만약 이 사람이 나가면 그 다음 사람이 오너.
        return _players.GetAt(0).Value;
    }

    public void EnterGame(Player player)
    {
        if (player == null)
            return;

        lock (_lock)
        {
            _players.Add(player.PlayerId, player);
            playerCount++;
        }
    }
}