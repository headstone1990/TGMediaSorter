using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TGMediaSorter;

StreamWriter wTelegramLogs = new StreamWriter("WTelegram.log", true, Encoding.UTF8) { AutoFlush = true };
WTelegram.Helpers.Log = (lvl, str) => wTelegramLogs.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{"TDIWE!"[lvl]}] {str}");

using var client = new WTelegram.Client(Config);
var myself = await client.LoginUserIfNeeded();
Console.WriteLine($"We are logged-in as {myself} (id {myself.id})");

Console.ReadLine();
return;

static string Config(string what)
{
    var authData = GetAuthData();
    switch (what)
    {
        case "api_id": return authData.ApiId.ToString();
        case "api_hash": return authData.ApiHash;
        case "phone_number": return authData.PhoneNumber;
        case "verification_code": Console.Write("Code: "); return Console.ReadLine() ?? string.Empty;
        case "first_name": return "John";      // if sign-up is required
        case "last_name": return "Doe";        // if sign-up is required
        case "password": return authData.Password;     // if user has enabled 2FA
        default: return null!;                  // let WTelegramClient decide the default config
    }
}

static AuthenticatorData GetAuthData()
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

