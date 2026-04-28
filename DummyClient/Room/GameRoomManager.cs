namespace DummyClient.Room;

public class GameRoomManager
{
    public static readonly GameRoomManager Instance = new GameRoomManager();
    
    private Dictionary<int, GameRoom> _gameRooms = new Dictionary<int, GameRoom>();
    
    private object _lock = new object();

    public GameRoom EnterGame(int roomId, Player player)
    {
        GameRoom gameRoom = Find(roomId);
        gameRoom.EnterGame(player);

        return gameRoom;
    }

    public void Add(GameRoom gameRoom)
    {
        if (gameRoom == null)
            throw new ArgumentNullException(nameof(gameRoom));

        lock (_lock)
        {
            _gameRooms.Add(gameRoom.Id, gameRoom);
        }
    }
    
    public void AddAll(Dictionary<int, GameRoom> gameRooms)
    {
        _gameRooms = gameRooms;
    }

    public GameRoom Find(int roomId)
    {
        if (roomId < 0)
            throw new ArgumentOutOfRangeException(nameof(roomId));
        GameRoom gameRoom;
        if (_gameRooms.TryGetValue(roomId, out gameRoom))
            return gameRoom;

        return null;
    }
}