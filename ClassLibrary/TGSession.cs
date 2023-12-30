using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TL;
using WTelegram;

namespace ClassLibrary;

public delegate string VerificationCode();

public class TGSession : IDisposable
{
    private Client _wTelegramClient;
    private VerificationCode _getVerificationCode;
    private User _myself;
    private Client? _wTelegramClient;
    private readonly VerificationCode _verificationCodeCallback;
    private User? _myself;

    public TGSession(VerificationCode verificationCodeCallback)
    {
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
        var authData = GetAuthData();
        switch (what)
        {
            case "api_id": return authData.ApiId.ToString();
            case "api_hash": return authData.ApiHash;
            case "phone_number": return authData.PhoneNumber;
            case "verification_code": return _verificationCodeCallback();
            case "first_name": return "John";      // if sign-up is required
            case "last_name": return "Doe";        // if sign-up is required
            case "password": return authData.Password;     // if user has enabled 2FA
            case "session_pathname": return Path.Combine(Environment.CurrentDirectory, "WTelegram.session");
            default: return null!;                  // let WTelegramClient decide the default config
        }
    }
    
    private AuthenticatorData GetAuthData()
    {
        using var host = Host.CreateDefaultBuilder().Build();
        var config = host.Services.GetRequiredService<IConfiguration>();
    
        var phoneNumber = config["Settings:phoneNumber"] ?? throw new InvalidOperationException();
        var apiHash = config["Settings:apiHash"] ?? throw new InvalidOperationException();
        var apiIdString = config["Settings:apiId"];
        var apiId = int.Parse(apiIdString ?? throw new InvalidOperationException());
        var password = config["Settings:password"] ?? throw new InvalidOperationException();
        return new AuthenticatorData(phoneNumber, apiHash, apiId, password);
    }

    public void Dispose()
    {
        _wTelegramClient?.Dispose();
    }
}