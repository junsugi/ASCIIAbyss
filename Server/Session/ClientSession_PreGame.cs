using Google.Protobuf.Protocol;
using Server.Game;
using ServerCore;
using Player = Server.Game.Player;

namespace Server;

public partial class ClientSession : PacketSession
{
    // TODO: 패킷을 몰라야한다.
    public void HandleSignUp(C_SignUp packet)
    {
        // 회원가입 시작
        string email = packet.AccountInfo.Email;
        string password = packet.AccountInfo.Password;
        bool isSuccess = AccountService.Instance.SignUp(email, password);
        if (isSuccess)
            Send(new S_SignUp());
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
            signInPacket.Player = result.Player.MapperToProto();
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
    
    // TODO: 패킷을 몰라야한다.
    public void HandleEnterGame(C_EnterGame enterGamePacket)
    {
        // 게임 룸에 입장
        Player player = new Player();
        player = player.MapperToPlayer(enterGamePacket.Player);
        
        int roomId = enterGamePacket.RoomId;
        GameRoom room = GameRoomManager.Instance.Find(roomId);
        if (room != null)
            room.EnterGame(roomId, player);
    }
}