using System.Net;
using System.Net.Sockets;
using ServerCore;

namespace DummyClient;

public class Connector
{
    public void Connect(EndPoint endPoint, ServerSession session)
    {
        Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.Completed += OnConnectCompleted;
        args.RemoteEndPoint = endPoint;
        args.UserToken = (socket, session);

        RegisterConnect(args);
    }

    private void RegisterConnect(SocketAsyncEventArgs args)
    {
        var (socket, _) = ((Socket, ServerSession))args.UserToken;
        if (socket == null)
            return;

        try
        {
            bool pending = socket.ConnectAsync(args);
            if (!pending)
                OnConnectCompleted(null, args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
    {
        try
        {
            if (args.SocketError == SocketError.Success)
            {
                var (_, session) = ((Socket, ServerSession))args.UserToken;
                session.Start(args.ConnectSocket);
                session.OnConnected();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}