using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

namespace Server;

public class ServerPacketManager
{
    private static ServerPacketManager _instance = new ServerPacketManager();
    public static ServerPacketManager Instance {get {return _instance;}}

    private Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    private Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

    private ServerPacketManager()
    {
        Register();
    }
    
    private void Register()
    {
        _onRecv.Add((ushort) MsgId.CSignUp, MakePacket<C_SignUp>);
        _handler.Add((ushort) MsgId.CSignUp, PacketHandler.C_SignUpHandler);
        _onRecv.Add((ushort) MsgId.CSignIn, MakePacket<C_SignIn>);
        _handler.Add((ushort) MsgId.CSignIn, PacketHandler.C_SignInHandler);
        _onRecv.Add((ushort) MsgId.CGameRoomList, MakePacket<C_GameRoomList>);
        _handler.Add((ushort) MsgId.CGameRoomList, PacketHandler.C_GameRoomListHandler);
        _onRecv.Add((ushort) MsgId.CCreateGameRoom, MakePacket<C_CreateGameRoom>);
        _handler.Add((ushort) MsgId.CCreateGameRoom, PacketHandler.C_CreateGameRoomHandler);
        _onRecv.Add((ushort) MsgId.CEnterGame, MakePacket<C_EnterGame>);
        _handler.Add((ushort) MsgId.CEnterGame, PacketHandler.C_EnterGameHandler);
    }
    
    public void OnConnectedPacket(ClientSession clientSession)
    {
        // TODO: 더미클라이언트에서 메뉴 선택 화면이 출력될 수 있도록 하기.
        S_Connected connectedPacket = new S_Connected();
        clientSession.Send(connectedPacket);
    }
    
    public void OnRecvPacket(ClientSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>, ushort> action = null;
        if(_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer, id);
    }

    private void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        //C_EnterGame 패킷 생성
        T packet = new T();
        // 헤더 파싱한 부분 빼고
        packet.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

        Action<PacketSession, IMessage> action = null;
        if (_handler.TryGetValue(id, out action))
            action.Invoke(session, packet);
    }
}