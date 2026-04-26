namespace ServerCore;

public abstract class PacketSession : Session
{
    private readonly int HeaderSize = 4;

    public override void OnReceive(ArraySegment<byte> buffer)
    {
        int processLength = 0;

        while (true)
        {
            if (buffer.Count < HeaderSize)
                break;
            
            // 헤더에 있는 총 데이터 크기 읽어오기
            ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            if (buffer.Count < dataSize)
                break;
            
            OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

            processLength += dataSize;
            buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
        }
    }
    
    public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    
    #region 구현 안함

    public override void OnConnected()
    {
    }
    public override void OnSend(ArraySegment<byte> buffer)
    {
    }

    public override void OnDisconnected()
    {
    }

    #endregion
}