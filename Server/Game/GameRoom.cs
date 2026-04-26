using Google.Protobuf.Protocol;

namespace Server.Game;

public class GameRoom
{
    private Map Map { get; } = new Map();
    public int RoomId { get; set; }

    Dictionary<int, Player> _players = new Dictionary<int, Player>();
    
    public void Init()
    {
        // 맵 초기화
        Map.LoadMap();
    }

    public void EnterGame(Player player)
    {
        if (player == null)
            return;
        
        _players.Add(player.PlayerId, player);
        
        // 본인한테 들어왔다고 전송
        S_EnterGame enterPacket = new S_EnterGame();
        enterPacket.Player = player.Mapper();
        player.Session.Send(enterPacket);
        Console.WriteLine($"{player.PlayerId} : EnterGame");
        
        // 현재 같은 룸에 있는 플레이어들한테 전송
        S_Spawn spawnPacket = new S_Spawn();
        spawnPacket.Player.Add(player.Mapper());
        BroadCast(spawnPacket);
    }

    private void BroadCast(S_Spawn spawnPacket)
    {
        foreach (Player p in _players.Values)
        {
            p.Session.Send(spawnPacket);
        }
    }
}