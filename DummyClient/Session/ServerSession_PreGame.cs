using Google.Protobuf.Protocol;
using ServerCore;

namespace DummyClient;

public partial class ServerSession : PacketSession
{
    private TaskCompletionSource<Player> _tcs =
        new TaskCompletionSource<Player>(TaskCreationOptions.RunContinuationsAsynchronously);
    
    private object _lock = new object();
    
    public void SignUp(string email, string password)
    {
        C_SignUp signUpPacket = new C_SignUp();
        // AccountManager 안으로 넣기
        AccountInfo accountInfo = new AccountInfo();
        accountInfo.Email = email;
        accountInfo.Password = password;
        signUpPacket.AccountInfo = accountInfo;
        Send(signUpPacket);
    }

    public Task<Player> SignInAsync(string email, string password)
    {
        if (UIState != PlayerClientUIState.UiLogin)
            return Task.FromException<Player>(
                new InvalidOperationException("로그인 불가"));

        C_SignIn signInPacket = new C_SignIn()
        {
            AccountInfo = new Account().Mapper(email, password)
        };
        Send(signInPacket);
        lock (_lock)
        {
            UIState = PlayerClientUIState.UiLogin;
            return _tcs.Task;
        }
    }

    public void CompletedSignIn(Player player)
    {
        Interlocked.Exchange(ref Player, player);
        setUIStatue(PlayerClientUIState.UiLobby);
        _tcs.TrySetResult(player);
    }
}