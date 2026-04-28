using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using Player = Server.Game.Player;

namespace Server;

public partial class ClientSession : PacketSession
{
    private static readonly int HeaderSize = 2;
    public int SessionId { get; set; }
    
    public override void OnConnected()
    {
        ServerPacketManager.Instance.OnConnectedPacket(this);
    }
    
    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ServerPacketManager.Instance.OnRecvPacket(this, buffer);
    }
    
    public void Send(IMessage packet)
    {
        // C_ENTER_GAME 으로 Protocol 작성했으면 Protocol.cs에는 CENTERGAME 이런식으로 _가 빠진 상태로 명명
        string msgName = packet.Descriptor.Name.Replace("_", String.Empty);
        MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
        Console.WriteLine($"Server Send packet={packet.Descriptor.Name}, msgName={msgName}, msgId={(ushort)msgId}, sessionId={SessionId}");
        ushort size = (ushort)packet.CalculateSize();
        byte[] sendBuffer = new byte[size + 4]; // 헤더 사이즈 4 추가
        // 헤더에 data size부터 기록
        Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
        // 2바이트 뒤에 msgId 기록
        Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
        // 패킷 데이터 4바이트 뒤에부터 기록 (전체 사이즈만큼)
        Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

        base.Send(sendBuffer);
    }
    
    public override void OnDisconnected()
    {
        SessionManager.Instance.Remove(this);
    }
    
    #region 아직 미사용

    public override void OnSend(ArraySegment<byte> buffer)
    {
        // 아직 모르겠음.
    }

    #endregion
}
