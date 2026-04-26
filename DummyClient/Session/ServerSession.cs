using DummyClient.Packet;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

namespace DummyClient;

public partial class ServerSession : PacketSession
{
    public int DummyId { get; set; }
    private static readonly int HeaderSize = 4;

    public bool IsConnected = false;

    public PlayerClientUIState UIState { get; private set; } = PlayerClientUIState.UiLogin;
    public Player Player = new Player();

    public override void OnConnected()
    {
        Interlocked.Exchange(ref IsConnected, true);
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ClientPacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public void Send(IMessage packet)
    {
        string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
        MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
        ushort size = (ushort)packet.CalculateSize();
        byte[] sendBuffer = new byte[size + 4];
        Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
        Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
        Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
        Send(new ArraySegment<byte>(sendBuffer));
    }

    public void setUIStatue(PlayerClientUIState state)
    {
        lock (_lock)
        {
            UIState = state;
        }
    }
}