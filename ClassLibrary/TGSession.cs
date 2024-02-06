using ClassLibrary.Abstractions;
using DotNext.Threading;
using TL;
using WTelegram;

namespace ClassLibrary;

public delegate string VerificationCodeCallback();

public class TGSession : ITGSession, IDisposable, IAsyncDisposable
{
    private readonly AsyncLazy<Client> _wTelegramClient;
    private readonly VerificationCodeCallback _verificationCodeCallbackCallback;
    private readonly AuthenticatorData _authData;
    private readonly CancellationToken _token;

    public TGSession(VerificationCodeCallback verificationCodeCallbackCallback, AuthenticatorData authData)
    {
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource(); 
        _token = cancelTokenSource.Token;
        _authData = authData;
        _verificationCodeCallbackCallback = verificationCodeCallbackCallback;
        _wTelegramClient = new AsyncLazy<Client>(async (token) =>
        {
            Client client = new Client(Config);
            await client.LoginUserIfNeeded();
            return client;
        });
    }



    public async Task<User> GetCurrentUserAsync()
    {
        var client = await _wTelegramClient.WithCancellation(_token);
        return client.User;
    }


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
        var client = Task.Run(() => _wTelegramClient.WithCancellation(_token), _token).GetAwaiter().GetResult();
        client.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        var client = await _wTelegramClient.WithCancellation(_token);
        client.Dispose();
    }
}