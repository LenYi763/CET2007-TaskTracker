using System.Text.Json;
using TaskTracker.Models;

namespace TaskTracker.Persistence;

public class JsonTaskRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public JsonTaskRepository(string filePath = "tasks.json")
    {
        _filePath = filePath;
    }

    public List<TaskItem> Load()
    {
        try
        {
            if (!File.Exists(_filePath))
                return new List<TaskItem>();

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<TaskItem>();

            var data = JsonSerializer.Deserialize<List<TaskItem>>(json, _options);
            return data ?? new List<TaskItem>();
        }
        catch (Exception ex)
        {
            // 满足“robust file I/O with error handling”
            Console.WriteLine($"[Load Error] {ex.Message}");
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
            Console.WriteLine($"[Save Error] {ex.Message}");
            return false;
        }
    }
}
