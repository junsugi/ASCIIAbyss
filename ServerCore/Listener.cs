using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Listener
{
    private Socket _listenSocket;
    
    public void Init(IPEndPoint endPoint)
    {
        _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);    
        _listenSocket.Bind(endPoint);
        _listenSocket.Listen(10);
        
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.Completed += ConnectedTest;
        
        _listenSocket.AcceptAsync(args);
    }

    public void ConnectedTest(object sender, SocketAsyncEventArgs e)
    {
        Console.WriteLine("I'm here..");
    }
}