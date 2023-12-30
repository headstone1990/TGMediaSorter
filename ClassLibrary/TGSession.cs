using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TL;
using WTelegram;

namespace ClassLibrary;

public delegate string VerificationCode();

public class TGSession : IDisposable
{
    private Client? _wTelegramClient;
    private readonly VerificationCode _verificationCodeCallback;
    private User? _myself;
    private readonly AuthenticatorData _authData;

    public TGSession(VerificationCode verificationCodeCallback, AuthenticatorData authData)
    {
        _authData = authData;
        _verificationCodeCallback = verificationCodeCallback;
    }

    public User Myself => _myself!;

    internal async Task Init ()
    {
        _wTelegramClient = new Client(Config);
        _myself = await _wTelegramClient.LoginUserIfNeeded();
    }
    
    
    private string Config(string what)
    {
        switch (what)
        {
            case "api_id": return _authData.ApiId.ToString();
            case "api_hash": return _authData.ApiHash;
            case "phone_number": return _authData.PhoneNumber;
            case "verification_code": return _verificationCodeCallback();
            case "first_name": return "John";      // if sign-up is required
            case "last_name": return "Doe";        // if sign-up is required
            case "password": return _authData.Password;     // if user has enabled 2FA
            case "session_pathname": return Path.Combine(Environment.CurrentDirectory, "WTelegram.session");
            default: return null!;                  // let WTelegramClient decide the default config
        }
    }

    public void Dispose()
    {
        _wTelegramClient?.Dispose();
    }
}