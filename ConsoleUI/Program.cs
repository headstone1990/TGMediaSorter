using ClassLibrary;
using ClassLibrary.Abstractions;
using ClassLibrary.Forward;
using ClassLibrary.Forward.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


VerificationCodeCallback verificationCodeCallback = GetVerificationCode;

WTelegramLogsConfigurator.ConfigureLogs();
var authData = GetAuthData();

IServiceCollection services = new ServiceCollection()
    .AddScoped<IUpdateHandler, UpdateHandler>()
    .AddScoped<ITGSession>(x => new TGSession(
        verificationCodeCallback, 
        authData,
        x.GetRequiredService<IUpdateHandler>()))
    .AddTransient<IForwarder, Forwarder>();

await using ServiceProvider serviceProvider = services.BuildServiceProvider();
using IServiceScope scope = serviceProvider.CreateScope();

var forwarder = scope.ServiceProvider.GetRequiredService<IForwarder>();
await forwarder.Forward(-1001979529376, 5379626745);

Console.ReadLine();
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
