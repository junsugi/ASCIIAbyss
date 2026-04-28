using System.Net.Sockets;

namespace ServerCore;

public abstract class Session
{
    private Socket _socket;
    private object _lock = new object();
    private bool _isConnected = true;

    private RecvBuffer _recvBuffer = new RecvBuffer(65535);

    private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    private SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
    private SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

    public abstract void OnConnected();
    public abstract int OnReceive(ArraySegment<byte> buffer);
    public abstract void OnSend(ArraySegment<byte> buffer);
    public abstract void OnDisconnected();

    public void Start(Socket socket)
    {
        _socket = socket;

        _recvArgs.Completed += OnRecvCompleted;
        _sendArgs.Completed += OnSendCompleted;

        RegisterRecv();
    }

    private void RegisterRecv()
    {
        if (_isConnected == false)
            return;

        _recvBuffer.Clean();
        ArraySegment<byte> segment = _recvBuffer.WriteSegment;
        _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
        Console.WriteLine($"RegisterRecv offset={segment.Offset}, count={segment.Count}, dataSize={_recvBuffer.DataSize}");

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
            try
            {
                Console.WriteLine($"OnRecvCompleted bytes={args.BytesTransferred}, socketError={args.SocketError}");
                // Write 커서 이동
                if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                {
                    Disconnected();
                    return;
                }

                int processLen = OnReceive(_recvBuffer.ReadSegment);
                Console.WriteLine($"After OnReceive processLen={processLen}, dataSize={_recvBuffer.DataSize}");
                if (processLen < 0 || _recvBuffer.DataSize < processLen)
                {
                    Disconnected();
                    return;
                }

                if (_recvBuffer.OnRead(processLen) == false)
                {
                    Disconnected();
                    return;
                }
                Console.WriteLine($"After OnRead processLen={processLen}, dataSize={_recvBuffer.DataSize}");

                Console.WriteLine("Before RegisterRecv");
                RegisterRecv();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        else
        {
            Disconnected();
        }
    }

    private void RegisterSend()
    {
        // 이미 Socket이 파괴됐는데 들어와서 sendArgs를 등록하려는 경우가 존재
        // 커넥션이 끊어졌으면 그냥 되돌아가게 하기.
        if (_isConnected == false)
            return;

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
        lock (_lock)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    Console.WriteLine($"OnSendCompleted bytes={args.BytesTransferred}, socketError={args.SocketError}");
                    _sendArgs.BufferList = null;
                    _pendingList.Clear();

                    if (_sendQueue.Count > 0)
                        RegisterSend();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                Disconnected();
            }
        }
    }

    public void Send(ArraySegment<byte> buffer)
    {
        // 보낼 데이터 없음 끝
        if (buffer.Count == 0)
            return;

        lock (_lock)
        {
            _sendQueue.Enqueue(buffer);
            if (_pendingList.Count == 0)
                RegisterSend();
        }
    }

    private void Disconnected()
    {
        if (Interlocked.Exchange(ref _isConnected, false) == false)
            return;

        OnDisconnected();
        Console.WriteLine("Disconnected called");
        Console.WriteLine($"{GetType().Name} disconnected");
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }
}
