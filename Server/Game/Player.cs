using System.Runtime.CompilerServices;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ProtoPlayer = Google.Protobuf.Protocol.Player;

namespace Server.Game;

public class Player
{
    public ClientSession Session { get; set; }
    public int PlayerId {get; set;}
    public string DisplayName { get; set; }
    public int HP { get; set; }
    public string Email { get; set; }

    private object _lock = new object();

    public ProtoPlayer Mapper()
    {
        ProtoPlayer protoPlayer = new ProtoPlayer();
        protoPlayer.DisplayName = DisplayName;
        protoPlayer.Hp = HP;
        protoPlayer.Id = PlayerId;
            
        return protoPlayer;
    }

    public void OnDamaged(int damage)
    {
        lock (_lock)
        {
            HP -= damage;
        }
    }
}