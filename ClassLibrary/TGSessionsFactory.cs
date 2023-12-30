namespace ClassLibrary;

public class TGSessionsFactory : IDisposable
{
    private static TGSessionsFactory? _instance;
    private VerificationCode _verificationCodeCallback;
    private List<TGSession> _tgSessionsList = new List<TGSession>(0);
    private TGSessionsFactory(VerificationCode verificationCode)
    {
        _verificationCodeCallback = verificationCode;
    }

    public async Task<TGSession> CreateTGSession()
    {
        var session = new TGSession(_verificationCodeCallback);
        _tgSessionsList.Add(session);
        await session.Init();
        return session;
    }
    
    public static TGSessionsFactory Create(VerificationCode verificationCode)
    {
        if (_instance == null)
        {
            _instance = new TGSessionsFactory(verificationCode);
        }

        return _instance;
    }

    public void Dispose()
    {
        foreach (var tgSession in _tgSessionsList)
        {
            tgSession.Dispose();
        }
    }
}