using ServerCore;

namespace Server;

public class SessionManager
{
    private static SessionManager _instance = new SessionManager();

    public static SessionManager Instance { get { return _instance; } }

    public Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();

    private int _sessionId = 0;
    private object _lock = new object();

    public ClientSession Generate()
    {
        lock (_lock)
        {
            ClientSession session = new ClientSession();
            session.SessionId = _sessionId;
            _sessions.Add(_sessionId++, session);

            return session;
        }
    }

    public List<ClientSession> GetSessions()
    {
        List<ClientSession> sessions = new List<ClientSession>();

        lock (_lock)
        {
            sessions = _sessions.Values.ToList();
        }

        return sessions;
    }

    public void Remove(ClientSession session)
    {
        lock (_lock)
        {
            _sessions.Remove(session.SessionId);
            Console.WriteLine($"Disconnected {session.SessionId} Remained session : {_sessions.Count}");
        }
    }
}