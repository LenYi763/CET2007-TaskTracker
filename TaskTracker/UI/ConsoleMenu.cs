using TaskTracker.Logging;
using TaskTracker.Models;
using TaskTracker.Persistence;
using TaskTracker.Services;

namespace TaskTracker.UI;

public class ConsoleMenu
{
    private readonly TaskService _service;
    private readonly JsonTaskRepository _repo;
    private readonly Logger _logger;

    public ConsoleMenu(TaskService service, JsonTaskRepository repo, Logger logger)
    {
        _service = service;
        _repo = repo;
        _logger = logger;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===== TaskTracker Menu =====");
            Console.WriteLine("1) Add Task");
            Console.WriteLine("2) List Tasks");
            Console.WriteLine("3) Update Status");
            Console.WriteLine("4) Delete Task");
            Console.WriteLine("5) Search");
            Console.WriteLine("6) Sort");
            Console.WriteLine("7) Report (Overdue / Upcoming)");
            Console.WriteLine("8) Save");
            Console.WriteLine("9) Load");
            Console.WriteLine("0) Exit");
            Console.Write("Select: ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1": AddTask(); break;
                case "2": ListTasks(); break;
                case "3": UpdateStatus(); break;
                case "4": DeleteTask(); break;
                case "5": Search(); break;
                case "6": Sort(); break;
                case "7": Report(); break;
                case "8": Save(); break;
                case "9": Load(); break;
                case "0":
                    _logger.Log("Program exited");
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private void AddTask()
    {
        Console.WriteLine("\n--- Add Task ---");
        int id = ReadInt("ID (int): ");
        string title = ReadString("Title: ");
        DateTime due = ReadDate("Due date (yyyy-MM-dd): ");
        int priority = ReadInt("Priority (1-5): ", 1, 5);
        string assignee = ReadString("Assignee: ");

        try
        {
            _service.Add(new TaskItem
            {
                Id = id,
                Title = title,
                DueDate = due,
                Priority = priority,
                Assignee = assignee,
                Status = TaskState.Todo
            });
            Console.WriteLine("Added.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Add failed: {ex.Message}");
        }
    }

    private void ListTasks()
    {
        Console.WriteLine("\n--- Task List ---");
        var all = _service.GetAll();
        if (all.Count == 0)
        {
            Console.WriteLine("(No tasks)");
            return;
        }
        foreach (var t in all) Console.WriteLine(t);
    }

    private void UpdateStatus()
    {
        Console.WriteLine("\n--- Update Status ---");
        int id = ReadInt("Task ID: ");
        Console.WriteLine("1) Todo  2) InProgress  3) Done");
        int s = ReadInt("New status: ", 1, 3);
        var newState = (TaskState)s;

        if (_service.UpdateStatus(id, newState))
            Console.WriteLine("Updated.");
        else
            Console.WriteLine("Task not found.");
    }

    private void DeleteTask()
    {
        Console.WriteLine("\n--- Delete Task ---");
        int id = ReadInt("Task ID: ");
        if (_service.Delete(id))
            Console.WriteLine("Deleted.");
        else
            Console.WriteLine("Task not found.");
    }

    private void Search()
    {
        Console.WriteLine("\n--- Search ---");
        Console.WriteLine("1) By ID");
        Console.WriteLine("2) By keyword (Title/Assignee)");
        int c = ReadInt("Select: ", 1, 2);

        if (c == 1)
        {
            int id = ReadInt("ID: ");
            var t = _service.FindById(id);
            Console.WriteLine(t == null ? "Not found." : t.ToString());
        }
        else
        {
            string kw = ReadString("Keyword: ");
            var results = _service.SearchByKeyword(kw);
            if (results.Count == 0) Console.WriteLine("No matches.");
            else foreach (var t in results) Console.WriteLine(t);
        }
    }

    private void Sort()
    {
        Console.WriteLine("\n--- Sort ---");
        Console.WriteLine("1) By DueDate (manual sort)");
        Console.WriteLine("2) By Priority (built-in sort)");
        int c = ReadInt("Select: ", 1, 2);

        if (c == 1)
        {
            _service.SortByDueDateManual();
            Console.WriteLine("Sorted by due date (manual).");
        }
        else
        {
            _service.SortByPriorityBuiltIn();
            Console.WriteLine("Sorted by priority (built-in).");
        }
    }

    private void Report()
    {
        Console.WriteLine("\n--- Report ---");
        Console.WriteLine("1) Overdue tasks");
        Console.WriteLine("2) Upcoming tasks (next N days)");
        int c = ReadInt("Select: ", 1, 2);

        var now = DateTime.Today;
        if (c == 1)
        {
            var overdue = _service.GetAll()
                .Where(t => t.DueDate.Date < now && t.Status != TaskState.Done)
                .ToList();

            Console.WriteLine(overdue.Count == 0 ? "(No overdue tasks)" : "");
            foreach (var t in overdue) Console.WriteLine(t);
        }
        else
        {
            int days = ReadInt("Days (e.g. 7): ", 1, 365);
            var upcoming = _service.GetAll()
                .Where(t => t.DueDate.Date >= now && t.DueDate.Date <= now.AddDays(days) && t.Status != TaskState.Done)
                .ToList();

            Console.WriteLine(upcoming.Count == 0 ? "(No upcoming tasks)" : "");
            foreach (var t in upcoming) Console.WriteLine(t);
        }
    }

    private void Save()
    {
        var ok = _repo.Save(_service.ExportAll());
        _logger.Log(ok ? "Saved tasks to tasks.json" : "Save failed");
        Console.WriteLine(ok ? "Saved." : "Save failed.");
    }

    private void Load()
    {
        var loaded = _repo.Load();
        _service.ReplaceAll(loaded);
        _logger.Log($"Loaded tasks from tasks.json. Count={loaded.Count}");
        Console.WriteLine($"Loaded. Count={loaded.Count}");
    }

    // -------- helpers --------
    private static int ReadInt(string prompt, int? min = null, int? max = null)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (int.TryParse(s, out int v))
            {
                if (min.HasValue && v < min.Value) { Console.WriteLine($"Must be >= {min}."); continue; }
                if (max.HasValue && v > max.Value) { Console.WriteLine($"Must be <= {max}."); continue; }
                return v;
            }
            Console.WriteLine("Invalid integer.");
        }
    }

    private static string ReadString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(s)) return s;
            Console.WriteLine("Cannot be empty.");
        }
    }

    private static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine()?.Trim();
            if (DateTime.TryParse(s, out var dt)) return dt.Date;
            Console.WriteLine("Invalid date. Example: 2025-12-31");
        }
    }
}
