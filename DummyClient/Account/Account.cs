using Google.Protobuf.Protocol;

namespace DummyClient;

public class Account
{
    public string Email { get; set; }
    public string Password { get; set; }

    public AccountInfo Mapper(string email, string password)
    {
        AccountInfo accountInfo = new AccountInfo();
        accountInfo.Email = email;
        accountInfo.Password = password;

        return accountInfo;
    }
}