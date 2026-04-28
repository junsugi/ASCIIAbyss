using Google.Protobuf.Protocol;
using Server.Game;
using ServerCore;
using Player = Server.Game.Player;

namespace Server;

public partial class ClientSession : PacketSession
{
    private Player _player { get; set; }

    // TODO: 패킷을 몰라야한다.
    public void HandleSignUp(C_SignUp packet)
    {
        // 회원가입 시작
        string email = packet.AccountInfo.Email;
        string password = packet.AccountInfo.Password;
        Account account = AccountService.Instance.SignUp(email, password);
        if (account != null)
        {
            PlayerManager.Instance.Add(account, this);
            Send(new S_SignUp());
        }
    }

    // TODO: 패킷을 몰라야한다.
    public void HandleSignIn(C_SignIn packet)
    {
        string email = packet.AccountInfo.Email;
        string password = packet.AccountInfo.Password;

        SignInResult result = AccountService.Instance.SignIn(email, password);
        if (result != null)
        {
            S_SignIn signInPacket = new S_SignIn();
            _player = PlayerManager.Instance.FindByEmail(result.Player.Email);
            _player.Session = this;
            Console.WriteLine($"SignIn bind playerId={_player.PlayerId}, name={_player.DisplayName}, sessionId={SessionId}");
            signInPacket.Player = _player.MapperToProto();
            Send(signInPacket);
        }
    }

    public void HandleMakeGameRoom(string displayName)
    {
        GameRoom gameRoom = GameRoomManager.Instance.MakeGameRoom(displayName);
        if (gameRoom != null)
        {
            // TODO: 클라에서 방 입장 시키기
            S_CreateGameRoom createGamePacket = new S_CreateGameRoom();
            createGamePacket.GameRoom = gameRoom.MapperToProto(gameRoom);
            Send(createGamePacket);
        }
    }

    public void HandleGameRoomList()
    {
        Dictionary<int, GameRoom> gameRooms = GameRoomManager.Instance.FindAll();
        // TODO: 클라한테 방 정보 전송
        S_GameRoomList gameRoomListPacket = new S_GameRoomList();
        gameRoomListPacket.GameRoom.AddRange(gameRooms.Values.Select(gameRoom => gameRoom.MapperToProto(gameRoom)));
        Send(gameRoomListPacket);
    }

    public void HandleEnterGame(int roomId, Player player)
    {
        // 상태체크
        // 여러가지 상태 로드
        GameRoomManager.Instance.EnterGame(roomId, player);
        // 방입장으로 상태 변경
    }
}
