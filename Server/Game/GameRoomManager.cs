namespace Server.Game;

public class GameRoomManager
{
    private static GameRoomManager _instance = new GameRoomManager();
    public static GameRoomManager Instance {get { return _instance; }}
    
    private Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
    private object _lock = new object();
    private int _roomId = 0;

    public GameRoom Add(int mapId)
    {
        lock (_lock)
        {
            GameRoom room = new GameRoom();
            room.Init();

            room.RoomId = _roomId++;
            _rooms.Add(_roomId, room);

            return room;
        }
    }
    
    public GameRoom Find(int roomId)
    {
        GameRoom room = null;
        if (_rooms.TryGetValue(roomId, out room))
            return room;

        return null;
    }
}