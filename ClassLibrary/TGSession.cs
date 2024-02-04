using ClassLibrary.Abstractions;
using TL;
using WTelegram;

namespace ClassLibrary;

public delegate string VerificationCodeCallback();

public class TGSession : IDisposable, ITGSession
{
    private readonly Client _wTelegramClient;
    private readonly VerificationCodeCallback _verificationCodeCallbackCallback;
    private readonly AuthenticatorData _authData;

    public TGSession(VerificationCodeCallback verificationCodeCallbackCallback, AuthenticatorData authData)
    {
        _authData = authData;
        _verificationCodeCallbackCallback = verificationCodeCallbackCallback;
        _wTelegramClient = new Client(Config);
        Task.Run(() => _wTelegramClient.LoginUserIfNeeded()).Wait();
    }

    public User Myself => _wTelegramClient.User;
    
    
    private string Config(string what)
    {
        switch (what)
        {
            case "api_id": return _authData.ApiId.ToString();
            case "api_hash": return _authData.ApiHash;
            case "phone_number": return _authData.PhoneNumber;
            case "verification_code": return _verificationCodeCallbackCallback();
            case "first_name": return "John";      // if sign-up is required
            case "last_name": return "Doe";        // if sign-up is required
            case "password": return _authData.Password;     // if user has enabled 2FA
            case "session_pathname": return Path.Combine(Environment.CurrentDirectory, "WTelegram.session");
            default: return null!;                  // let WTelegramClient decide the default config
        }
    }

    public void Dispose()
    {
        _wTelegramClient.Dispose();
    }
    
}