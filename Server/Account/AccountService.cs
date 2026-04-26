using Server.Game;

namespace Server;

public class AccountService
{
    private static readonly AccountService _Instance = new AccountService();
    public static AccountService Instance => _Instance;
    
    // 메모리 디비 (계정정보)
    private Dictionary<string, Account> _accounts = new Dictionary<string, Account>();

    public bool SignUp(string email, string password)
    {
        Account account = new Account(email, password);
        bool isDuplicate = _accounts.ContainsKey(account.Email);
        if (!isDuplicate)
        {
            _accounts.Add(account.Email, account);
            PlayerManager.Instance.Add(account);
            return true;
        }
        
        return false;
    }

    public SignInResult SignIn(string email, string password)
    {
        SignInResult result = new SignInResult();
        // 디비에서 플레이어 정보 찾기
        Account account = null;
        if (_accounts.TryGetValue(email, out account))
        {
            if (account.Password == password)
            {
                Player player = PlayerManager.Instance.Find(account.Email);
                result.Player = player;
                return result;
            }
        }
        
        return null;
    }
}