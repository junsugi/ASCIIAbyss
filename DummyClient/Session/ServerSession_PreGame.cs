using DummyClient.Room;
using Google.Protobuf.Protocol;

namespace DummyClient;

public partial class ServerSession : PacketSession
{
    private TaskCompletionSource<Player> _playerTcs;

    private TaskCompletionSource<Dictionary<int, GameRoom>> _gameRoomsTcs;
    private TaskCompletionSource<GameRoom> _gameRoomTcs;

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

        _playerTcs = new TaskCompletionSource<Player>(TaskCreationOptions.RunContinuationsAsynchronously);

        C_SignIn signInPacket = new C_SignIn()
        {
            AccountInfo = new Account().Mapper(email, password)
        };
        Send(signInPacket);

        lock (_lock)
        {
            UIState = PlayerClientUIState.UiLogin;
            return _playerTcs.Task;
        }
    }

    public void CompletedSignIn(Player player)
    {
        Interlocked.Exchange(ref Player, player);
        setUIStatue(PlayerClientUIState.UiLobby);
        _playerTcs.TrySetResult(player);
    }

    // 현재 생성된 게임룸 리스트 서버로부터 받아오기
    public Task<Dictionary<int, GameRoom>> LobbyAsync()
    {
        C_GameRoomList gameRoomListPacket = new C_GameRoomList();
        _gameRoomsTcs = new TaskCompletionSource<Dictionary<int, GameRoom>>();
        Send(gameRoomListPacket);

        return _gameRoomsTcs.Task;
    }

    public void CompletedLobby(Dictionary<int, GameRoom> gameRooms)
    {
        lock (_lock)
        {
            GameRoomManager.Instance.AddAll(gameRooms);
            _gameRoomsTcs.TrySetResult(gameRooms);
        }
    }

    public Task<GameRoom> CreateRoomAsync(string displayName)
    {
        _gameRoomTcs = new TaskCompletionSource<GameRoom>(TaskCreationOptions.RunContinuationsAsynchronously);
        C_CreateGameRoom createRoomPacket = new C_CreateGameRoom()
        {
            DisplayName = displayName
        };
        Send(createRoomPacket);

        return _gameRoomTcs.Task;
    }

    public void CompletedCreateRoom(GameRoom gameRoom)
    {
        lock (_lock)
        {
            GameRoomManager.Instance.Add(gameRoom);
            _gameRoomTcs.TrySetResult(gameRoom);
        }
    }

    public Task<GameRoom> EnterGame(int roomId, Player player)
    {
        _gameRoomTcs = new TaskCompletionSource<GameRoom>(TaskCreationOptions.RunContinuationsAsynchronously);
        
        if (roomId < 0 || player == null)
            throw new Exception($"roomId={roomId}, player={player} is not found");
        
        GameRoom gameRoom = GameRoomManager.Instance.Find(roomId);
        
        C_EnterGame enterGamePacket = new C_EnterGame();
        enterGamePacket.RoomId = gameRoom.Id;
        enterGamePacket.Player = player.Mapper();
        Send(enterGamePacket);
        
        return _gameRoomTcs.Task;
    }

    public void CompletedEnterGame(GameRoom gameRoom)
    {
        lock (_lock)
        {
            _gameRoomTcs.TrySetResult(gameRoom);
        }
    }
}