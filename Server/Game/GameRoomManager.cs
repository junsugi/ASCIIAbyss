namespace Server.Game;

public class GameRoomManager
{
    private static GameRoomManager _instance = new GameRoomManager();
    public static GameRoomManager Instance {get { return _instance; }}
    
    private Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
    private object _lock = new object();
    private int _roomId = 0;

    private GameRoomManager()
    {
    }

    public GameRoom Find(int roomId)
    {
        GameRoom room = null;
        if (_rooms.TryGetValue(roomId, out room))
            return room;

        return null;
    }

    public GameRoom MakeGameRoom(string gameRoomDisplayName)
    {
        GameRoom gameRoom = new GameRoom();
        gameRoom.RoomId = GetHashCode();
        gameRoom.DisplayName = gameRoomDisplayName;
        gameRoom.playerCount = 1;

        lock (_lock)
        {
            _rooms.Add(gameRoom.RoomId, gameRoom);
            return gameRoom;
        }
    }

    public Dictionary<int, GameRoom> FindAll()
    {
        return _rooms;
    }
}