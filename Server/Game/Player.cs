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
    
    public Player()
    {}

    public Player(ProtoPlayer player)
    {
        if (player == null)
            return;
        
        PlayerId = player.Id;
        DisplayName = player.DisplayName;
        HP = player.Hp;
        Email = player.Email;
    }
    
    public ProtoPlayer MapperToProto()
    {
        ProtoPlayer protoPlayer = new ProtoPlayer();
        protoPlayer.DisplayName = DisplayName;
        protoPlayer.Hp = HP;
        protoPlayer.Id = PlayerId;
            
        return protoPlayer;
    }

    public Player MapperToPlayer(ProtoPlayer protoPlayer)
    {
        Player player = new Player();
        player.DisplayName = protoPlayer.DisplayName;
        player.Email = protoPlayer.Email;
        player.PlayerId = protoPlayer.Id;
        player.HP = protoPlayer.Hp;

        return player;
    }

    public void OnDamaged(int damage)
    {
        lock (_lock)
        {
            HP -= damage;
        }
    }
}