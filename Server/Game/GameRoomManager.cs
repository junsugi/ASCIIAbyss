using Google.Protobuf.Protocol;

namespace Server.Game;

public class GameRoomManager
{
    public static readonly GameRoomManager Instance = new GameRoomManager();
    
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
        gameRoom.playerCount = 0;

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

    public void EnterGame(int roomId, Player player)
    {
        GameRoom gameRoom = Find(roomId);
        if (gameRoom == null)
            throw new Exception("Game room not found");
        
        gameRoom.EnterGame(player);
    }
}