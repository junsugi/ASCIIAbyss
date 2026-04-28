using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

namespace DummyClient.Packet;

public class ClientPacketManager
{
    private static readonly ClientPacketManager instance = new();
    public static ClientPacketManager Instance => instance;

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv =
        new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();

    Dictionary<ushort, Action<PacketSession, IMessage>> _handler =
        new Dictionary<ushort, Action<PacketSession, IMessage>>();

    public ClientPacketManager()
    {
        Register();
    }

    public void Register()
    {
        _onRecv.Add((ushort)MsgId.SConnected, MakePacket<S_Connected>);
        _handler.Add((ushort)MsgId.SConnected, PacketHandler.S_ConnectedHandler);
        _onRecv.Add((ushort)MsgId.SSignUp, MakePacket<S_SignUp>);
        _handler.Add((ushort)MsgId.SSignUp, PacketHandler.S_SignUpHandler);
        _onRecv.Add((ushort)MsgId.SSignIn, MakePacket<S_SignIn>);
        _handler.Add((ushort)MsgId.SSignIn, PacketHandler.S_SignInHandler);
        _onRecv.Add((ushort)MsgId.SGameRoomList, MakePacket<S_GameRoomList>);
        _handler.Add((ushort)MsgId.SGameRoomList, PacketHandler.S_GameRoomListHandler);
        _onRecv.Add((ushort)MsgId.SCreateGameRoom, MakePacket<S_CreateGameRoom>);
        _handler.Add((ushort)MsgId.SCreateGameRoom, PacketHandler.S_CreateGameRoomHandler);
        _onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
        _handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);
        _onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
        _handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        Console.WriteLine($"OnRecvPacket id={id}, size={size}");

        Action<PacketSession, ArraySegment<byte>, ushort> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer, id);
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        T pkt = new T();
        pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
        Console.WriteLine($"MakePacket type={typeof(T).Name}, id={id}");

        Action<PacketSession, IMessage> action = null;
        if (_handler.TryGetValue(id, out action))
            action.Invoke(session, pkt);
    }
}
