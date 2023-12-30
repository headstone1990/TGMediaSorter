using ClassLibrary;


VerificationCode verificationCode = GetVerificationCode;

WTelegramLogsConfigurator.ConfigureLogs();
var factory = TGSessionsFactory.Create(verificationCode);
var session = await factory.CreateTGSession();
Console.WriteLine(session.Myself.first_name);

Console.ReadLine();
factory.Dispose();
return;

string GetVerificationCode()
{
    Console.Write("Code: ");
    return Console.ReadLine() ?? string.Empty;
}
