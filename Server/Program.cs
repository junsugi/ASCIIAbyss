using System.Net;
using Server.Game;
using ServerCore;

namespace Server;

class Program
{
    private static Listener _listener = new Listener();
    
    static void Main(string[] args)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 5555);

        GameRoomManager.Instance.Add(1);
        
        _listener.Init(endPoint, () => SessionManager.Instance.Generate());
        
        Console.WriteLine("Listening...");

        while (true)
        {
            // 서버 종료 방지
        }
    }
}