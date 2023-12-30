namespace ClassLibrary;

public class TGSessionsFactory : IDisposable
{
    private static TGSessionsFactory? _instance;
    private static readonly object InstanceLock = new object();
    private readonly object _sessionsListLock = new object();
    private readonly VerificationCode _verificationCodeCallback;
    private readonly List<TGSession> _tgSessionsList = [];
    private TGSessionsFactory(VerificationCode verificationCode)
    {
        _verificationCodeCallback = verificationCode;
    }

    public async Task<TGSession> CreateTGSession(AuthenticatorData authData)
    {
        var session = new TGSession(_verificationCodeCallback, authData);
        lock (_sessionsListLock)
        {
            _tgSessionsList.Add(session);
        }
        await session.Init();
        return session;
    }
    
    public static TGSessionsFactory Create(VerificationCode verificationCode)
    {
        lock (InstanceLock)
        {
            if (_instance == null)
            {
                _instance = new TGSessionsFactory(verificationCode);
            }
        }

        return _instance;
    }

    public void Dispose()
    {
        lock (_sessionsListLock)
        {
            foreach (var tgSession in _tgSessionsList)
            {
                tgSession.Dispose();
            }
        }
    }
}