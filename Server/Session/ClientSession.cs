using System.Net;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

namespace Server;

public class ClientSession : Session
{
    private static readonly int HeaderSize = 2;
    public override void OnConnected()
    {
        Console.WriteLine("Connected to server");

        byte[] buffer = Encoding.UTF8.GetBytes("Hello Client!");
        ArraySegment<byte> sendBuffer = new ArraySegment<byte>(buffer);
        Send(sendBuffer);
    }

    public override void OnReceive(ArraySegment<byte> buffer)
    {
        while (true)
        {   
            if (buffer.Count < HeaderSize)
                break;
            
            // 파싱할 데이터는 있는지?
            if (buffer.Count > 0)
            {
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                
                // 데이터 크기가 버퍼보다 크면 이상한 상태니까
                if (buffer.Count < dataSize)
                    break;
                // 받아서 Console 찍기
                string data = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, dataSize);
                Console.WriteLine(data);
                // 무한루프 탈출
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset, 0);
            }
        }
    }

    public override void OnSend(ArraySegment<byte> buffer)
    {
        throw new NotImplementedException();
    }
}