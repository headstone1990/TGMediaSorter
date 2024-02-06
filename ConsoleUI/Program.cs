﻿using ClassLibrary;
using ClassLibrary.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TL;


VerificationCodeCallback verificationCodeCallback = GetVerificationCode;

WTelegramLogsConfigurator.ConfigureLogs();
var authData = GetAuthData();

IServiceCollection services = new ServiceCollection()
    .AddScoped<ITGSession>(_ => new TGSession(verificationCodeCallback, authData));

await using ServiceProvider serviceProvider = services.BuildServiceProvider();
using IServiceScope scope = serviceProvider.CreateScope();

ITGSession session = scope.ServiceProvider.GetRequiredService<ITGSession>();

User currentUser = await session.GetCurrentUserAsync();
Console.WriteLine(currentUser.first_name);  

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
