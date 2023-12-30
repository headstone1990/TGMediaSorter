using ClassLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


VerificationCode verificationCode = GetVerificationCode;

WTelegramLogsConfigurator.ConfigureLogs();
var factory = TGSessionsFactory.Create(verificationCode);
var session = await factory.CreateTGSession(GetAuthData());
Console.WriteLine(session.Myself.first_name);

Console.ReadLine();
factory.Dispose();
return;

string GetVerificationCode()
{
    Console.Write("Code: ");
    return Console.ReadLine() ?? string.Empty;
}

AuthenticatorData GetAuthData()
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
