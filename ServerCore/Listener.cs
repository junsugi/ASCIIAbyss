using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Listener
{
    private Socket _listenSocket;
    private Func<Session> _sessionFactory;
    
    public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
    {
        _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);    
        _listenSocket.Bind(endPoint);
        _listenSocket.Listen(10);

        _sessionFactory = sessionFactory;
        
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.Completed += OnAcceptCompleted;

        RegisterAccept(args);
    }

    private void RegisterAccept(SocketAsyncEventArgs args)
    {
        // TODO : args 초기화 해야됨?
       bool pending = _listenSocket.AcceptAsync(args);
       if (!pending)
           OnAcceptCompleted(null, args);
    }

    private void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            Session clientSession = _sessionFactory.Invoke();
            clientSession.Start(args.AcceptSocket);
            clientSession.OnConnected();
        }
        
        // 여기서 args 반환을 해줘야 다음 클라이언트 접속 대기
        RegisterAccept(args);
    }
}