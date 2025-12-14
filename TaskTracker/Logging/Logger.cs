namespace TaskTracker.Logging;

public class Logger
{
    private readonly string _logPath;

    public Logger(string logPath = "log.txt")
    {
        _logPath = logPath;
    }

    public void Log(string message)
    {
        var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        try
        {
            File.AppendAllText(_logPath, line + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Logger Error] {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"[Logger Path] {_logPath}");
        }
    }
}
