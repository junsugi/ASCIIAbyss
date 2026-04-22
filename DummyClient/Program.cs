using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient;

class Program
{
    static void Main(string[] args)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 5555);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); }, 10);

        Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(endPoint);
    
        Thread t = new Thread(() => ReceiveLoop(socket));
        t.Start();
        
        // 2. 메인 스레드는 여기서 계속 키보드 입력을 대기
        while (true)
        {
            string message = Console.ReadLine(); // 여기서 멈춰서 입력을 대기
            if (string.IsNullOrEmpty(message)) continue;

            byte[] body = Encoding.UTF8.GetBytes(message);
            byte[] packet = new byte[body.Length];
            Array.Copy(body, 0, packet, 0, body.Length);

            socket.Send(packet); // 서버로 전송!
        }
    }

    static void ReceiveLoop(Socket socket)
    {
        byte[] recvBuffer = new byte[65535];
        while (true)
        {
            int nRecv = socket.Receive(recvBuffer); // 서버가 보낼 때까지 여기서 대기.
            if (nRecv <= 0) break;

            string result = Encoding.UTF8.GetString(recvBuffer, 0, nRecv);
            Console.WriteLine($"[From Server]: {result}");
        }
    }
}