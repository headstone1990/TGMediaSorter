using System.Text;

namespace ClassLibrary;

public static class WTelegramLogsConfigurator
{
    public static void ConfigureLogs()
    {
        StreamWriter wTelegramLogs = new StreamWriter("WTelegram.log", true, Encoding.UTF8) { AutoFlush = true };
        WTelegram.Helpers.Log = (lvl, str) => wTelegramLogs.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{"TDIWE!"[lvl]}] {str}");
    }
}