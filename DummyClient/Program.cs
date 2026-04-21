using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient;

class Program
{
    private static Socket _socket;
    
    static void Main(string[] args)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 5555);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(endPoint);
        
        // 1. 서버로부터 메시지를 받는 전용 스레드를 하나 실행시킨다네.
        Thread t1 = new Thread(ReceiveLoop);
        t1.Name = "ReceiveLoop";
        t1.Start();

        // 2. 메인 스레드는 여기서 계속 키보드 입력을 기다리지.
        while (true)
        {
            string message = Console.ReadLine(); // 여기서 멈춰서 입력을 기다려.
            if (string.IsNullOrEmpty(message)) continue;

            byte[] body = Encoding.UTF8.GetBytes(message);
            byte[] packet = new byte[body.Length];
            Array.Copy(body, 0, packet, 0, body.Length);

            _socket.Send(packet); // 서버로 전송!
        }
    }

    static void ReceiveLoop()
    {
        byte[] recvBuffer = new byte[65535];
        while (true)
        {
            int nRecv = _socket.Receive(recvBuffer); // 서버가 보낼 때까지 여기서 대기.
            if (nRecv <= 0) break;

            string result = Encoding.UTF8.GetString(recvBuffer, 0, nRecv);
            Console.WriteLine($"[From Server]: {result}");
        }
    }
}