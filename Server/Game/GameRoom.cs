using Google.Protobuf.Protocol;
using Player = Server.Game.Player;
using GameRoomProto = Google.Protobuf.Protocol.GameRoom;

namespace Server;

public class GameRoom
{
    private Map Map { get; } = new Map();
    public int RoomId { get; set; }
    public string DisplayName { get; set; }
    public int playerCount { get; set; }

    Dictionary<int, Player> _players = new Dictionary<int, Player>();
    
    public void Init()
    {
        // 맵 초기화
        Map.LoadMap();
    }

    public void EnterGame(Player player)
    {
        if (player == null)
            throw new Exception("Player is null");
        
        _players.Add(player.PlayerId, player);
        
        // 본인한테 들어왔다고 전송
        S_EnterGame enterPacket = new S_EnterGame();
        enterPacket.Player = player.MapperToProto();
        enterPacket.RoomId = RoomId;
        player.Session.Send(enterPacket);
        
        // 현재 같은 룸에 있는 플레이어들한테 전송
        S_Spawn spawnPacket = new S_Spawn();
        // 지금은 플레이어밖에 스폰데이터가 없는데, 투사체 혹은 몬스터들을 같이 보내줘야할 경우도 존재?
        spawnPacket.Player.Add(player.MapperToProto());
        spawnPacket.RoomId = RoomId;
        BroadCast(spawnPacket, player);
    }

    private void BroadCast(S_Spawn spawnPacket, Player exceptPlayer)
    {
        foreach (Player p in _players.Values)
        {
            if (p == exceptPlayer)
                continue;
            Console.WriteLine($"Broadcast target playerId={p.PlayerId}, name={p.DisplayName}, sessionId={p.Session?.SessionId}");
            p.Session.Send(spawnPacket);
        }
    }

    public GameRoomProto MapperToProto(GameRoom gameRoom)
    {
        GameRoomProto gameRoomProto = new GameRoomProto();
        gameRoomProto.Id = gameRoom.RoomId;
        gameRoomProto.RoomName = gameRoom.DisplayName;
        gameRoomProto.PlayerCount = gameRoom.playerCount;

        return gameRoomProto;
    }
}
