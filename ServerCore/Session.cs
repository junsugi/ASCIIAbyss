using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

public abstract class Session
{
    private Socket _socket;

    private ArraySegment<byte> _recvBuffer = new ArraySegment<byte>(new byte[65535]);

    private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    private SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
    private SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

    public abstract void OnConnected();
    public abstract void OnReceive(ArraySegment<byte> buffer);
    public abstract void OnSend(ArraySegment<byte> buffer);

    public void Start(Socket socket)
    {
        _socket = socket;

        _recvArgs.Completed += OnRecvCompleted;
        _sendArgs.Completed += OnSendCompleted;

        RegisterRecv();
    }

    private void RegisterRecv()
    {
        _recvArgs.SetBuffer(_recvBuffer.Array, _recvBuffer.Offset, _recvBuffer.Count);

        try
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (!pending)
                OnRecvCompleted(null, _recvArgs);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void OnRecvCompleted(object? sender, SocketAsyncEventArgs args)
    {
        // 소켓에 내용이 있고 Success 일 때
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            OnReceive(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));

            RegisterRecv();
        }
        else
        {
            Disconnected();
        }
    }

    private void RegisterSend()
    {
        _pendingList.Clear();
        
        while (_sendQueue.Count > 0)
        {
            ArraySegment<byte> buffer = _sendQueue.Dequeue();
            _pendingList.Add(buffer);
        }
        _sendArgs.BufferList = _pendingList;
        
        try
        {
            bool pending = _socket.SendAsync(_sendArgs);
            if (!pending)
                OnSendCompleted(null, _sendArgs);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void OnSendCompleted(object? sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            RegisterSend();
        }
    }

    public void Send(ArraySegment<byte> buffer)
    {
        // 보낼 데이터 없음 끝
        if (buffer.Count == 0)
            return;

        _sendQueue.Enqueue(buffer);

        RegisterSend();
    }

    private void Disconnected()
    {
        Console.WriteLine($"{GetType().Name} disconnected");
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    } 
}