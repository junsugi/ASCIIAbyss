using Google.Protobuf;
using Google.Protobuf.Protocol;

namespace DummyClient.Packet;

public class PacketHandler
{
    public static void S_ConnectedHandler(PacketSession session, IMessage packet)
    {
    }

    public static void S_SignUpHandler(PacketSession session, IMessage packet)
    {
        // 나중에 웹서버로 바꿀꺼니까
        ServerSession serverSession = (ServerSession)session;
        S_SignUp signUpPacket = (S_SignUp)packet;
    }

    public static void S_SignInHandler(PacketSession session, IMessage packet)
    {
        // 받아와서 화면 업데이트? 그리고 플레이어 정보 가지고 있어야돼.
        ServerSession serverSession = (ServerSession)session;
        S_SignIn signInPacket = (S_SignIn)packet;

        Player player = new Player();
        player.PlayerId = signInPacket.Player.Id;
        player.DisplayName = signInPacket.Player.DisplayName;
        player.HP = signInPacket.Player.Hp;

        // Async/Await 문법으로 바꾸니까 기존의 문제점도 해결됨.
        serverSession.CompletedSignIn(player);
    }

    public static void S_GameRoomListHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = (ServerSession)session;
        S_GameRoomList gameRoomListPacket = (S_GameRoomList)packet;
        Dictionary<int, GameRoom> gameRooms = gameRoomListPacket.GameRoom
            .ToDictionary(room => room.Id, room => new GameRoom()
            {
                Id = room.Id,
                displayName = room.RoomName,
                playerCount = room.PlayerCount,
            });

        serverSession.CompletedLobby(gameRooms);
    }

    public static void S_CreateGameRoomHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = (ServerSession)session;
        S_CreateGameRoom createGameRoomPacket = (S_CreateGameRoom)packet;
        
        // GameRoomManager에서 GameRoom을 생성해서 넘겨주는게 맞는거 같음.
        GameRoom gameRoom = new GameRoom();
        gameRoom.Id = createGameRoomPacket.GameRoom.Id;
        gameRoom.displayName = createGameRoomPacket.GameRoom.RoomName;
        gameRoom.playerCount = createGameRoomPacket.GameRoom.PlayerCount;
        
        serverSession.CompletedCreateRoom(gameRoom);
    }
}