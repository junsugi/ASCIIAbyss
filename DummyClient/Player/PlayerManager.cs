namespace DummyClient;

public class PlayerManager
{
    private static readonly PlayerManager _Instance = new PlayerManager();
    public static PlayerManager Instance => _Instance;
    
    private Dictionary<string, Player> _players = new Dictionary<string, Player>();

    // 최초 추가
    public void Add(Account account)
    {
        Player player = new Player();
        player.PlayerId = GetHashCode();
        player.DisplayName = Guid.NewGuid().ToString("n").Substring(0, 8);
        player.HP = 100;
        player.Email = account.Email;
        
        _players.Add(account.Email, player);
    }

    public void Remove(Player player)
    {
        
    }

    public Player Find(string email)
    {
        Player player = null;
        if (_players.TryGetValue(email, out player))
            return player;
        
        return null;
    }
    
}