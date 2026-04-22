using ServerCore;

namespace DummyClient;

public class SessionManager
{
    public static SessionManager Instance { get; } = new SessionManager();

    private HashSet<Session> _sessions = new HashSet<Session>();

    private int _dummyId = 1;
    private object _lock = new object();
    
    public ServerSession Generate()
    {
        lock(_lock)
        {
            ServerSession session = new ServerSession();
            session.DummyId = _dummyId++;

            _sessions.Add(session);
            return session;
        }
    }
}