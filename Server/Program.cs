using System.Net;
using ServerCore;

namespace Server;

class Program
{
    private static Listener _listener = new Listener();
    
    static void Main(string[] args)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 5555);
        
        _listener.Init(endPoint);
        
        Console.WriteLine("Listening...");

        while (true)
        {
            // 서버 종료 방지
        }
    }
}