namespace DummyClient;

public class AccountManager
{
    private static readonly AccountManager _instance = new AccountManager();
    public static AccountManager Instance {get { return _instance; }}
    
    private Dictionary<string, Account> _accounts = new Dictionary<string, Account>();

    public void SignUp(string email, string password)
    {
        Account account = new Account();
        account.Email = email;
        account.Password = password;
        _accounts.Add(email, account);
    }

    public Account SignIn(string email, string password)
    {
        Account account = null;
        if (_accounts.TryGetValue(email, out account))
        {
            if (account.Password == password)
                return account;
        }

        return null;
    }
}