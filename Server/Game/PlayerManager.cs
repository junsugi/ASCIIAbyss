namespace Server.Game;

public class PlayerManager
{
    private static readonly PlayerManager _Instance = new PlayerManager();
    public static PlayerManager Instance => _Instance;
    
    private Dictionary<int, Player> _players = new Dictionary<int, Player>();

    // 최초 추가
    public void Add(Account account, ClientSession session)
    {
        // 디비로 옮기면 사라질 친구들
        Random rand = new Random();
        int id = rand.Next(1, 1000000);
        
        Player player = new Player();
        player.PlayerId = id;
        player.DisplayName = Guid.NewGuid().ToString("n").Substring(0, 8);
        player.HP = 100;
        player.Email = account.Email;
        player.Session = session;
        
        _players.Add(player.PlayerId, player);
    }

    public void Remove(Player player)
    {
        
    }

    public Player FindByEmail(string email)
    {
        return _players.Values.FirstOrDefault(p => p.Email == email);
    }

    public Player FindById(int id)
    {
        Player player = null;
        if (_players.TryGetValue(id, out player))
            return player;
        
        return null;
    }
    
}