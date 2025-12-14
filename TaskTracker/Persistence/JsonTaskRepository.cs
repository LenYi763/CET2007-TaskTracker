using System.Text.Json;
using TaskTracker.Models;
using TaskTracker.Logging;

namespace TaskTracker.Persistence;

public class JsonTaskRepository
{
    private readonly string _filePath;
    private readonly Logger? _logger;

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public JsonTaskRepository(string filePath = "tasks.json", Logger? logger = null)
    {
        _filePath = filePath;
        _logger = logger;
    }

    public List<TaskItem> Load()
    {
        if (!File.Exists(_filePath))
        {
            _logger?.Log($"Load skipped: file not found ({_filePath})");
            return new List<TaskItem>();
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger?.Log($"Load skipped: empty file ({_filePath})");
                return new List<TaskItem>();
            }

            var data = JsonSerializer.Deserialize<List<TaskItem>>(json, _options);
            return data ?? new List<TaskItem>();
        }
        catch (JsonException ex)
        {
            // JSON格式坏了
            _logger?.Log($"Load failed: malformed JSON ({_filePath}). {ex.Message}");
            Console.WriteLine("Warning: tasks.json is malformed. Starting with empty task list.");
            return new List<TaskItem>();
        }
        catch (Exception ex)
        {
            // 其他异常
            _logger?.Log($"Load failed: {ex.GetType().Name} ({_filePath}). {ex.Message}");
            Console.WriteLine($"Warning: failed to load tasks.json. Starting with empty task list. ({ex.Message})");
            return new List<TaskItem>();
        }
    }


    public bool Save(List<TaskItem> tasks)
    {
        try
        {
            var json = JsonSerializer.Serialize(tasks, _options);
            File.WriteAllText(_filePath, json);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.Log($"Save failed: {ex.GetType().Name} ({_filePath}). {ex.Message}");
            Console.WriteLine($"[Save Error] {ex.Message}");
            return false;
        }
    }
}
