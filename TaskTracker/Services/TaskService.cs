using TaskTracker.Models;

namespace TaskTracker.Services;

public class TaskService
{
    private readonly List<TaskItem> _tasks = new();

    public IReadOnlyList<TaskItem> GetAll() => _tasks;
    public void ReplaceAll(IEnumerable<TaskItem> tasks)
    {
        _tasks.Clear();
        _tasks.AddRange(tasks);
    }

    public List<TaskItem> ExportAll()
    {
        return _tasks.ToList();
    }


    public void Add(TaskItem task)
    {
        if (_tasks.Any(t => t.Id == task.Id))
            throw new ArgumentException($"Task with ID {task.Id} already exists.");

        _tasks.Add(task);
    }

    public bool Delete(int id)
    {
        var t = _tasks.FirstOrDefault(x => x.Id == id);
        if (t == null) return false;
        _tasks.Remove(t);
        return true;
    }

    public TaskItem? FindById(int id)
    {
        // 线性搜索（满足要求）
        foreach (var t in _tasks)
            if (t.Id == id) return t;
        return null;
    }

    public List<TaskItem> SearchByKeyword(string keyword)
    {
        keyword = (keyword ?? "").Trim();
        if (keyword.Length == 0) return new List<TaskItem>();

        return _tasks
            .Where(t =>
                t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                t.Assignee.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public bool UpdateStatus(int id, TaskState newState)
    {
        var t = FindById(id);
        if (t == null) return false;
        t.Status = newState;
        return true;
    }

    // 手写排序：按 DueDate 升序（选择排序）
    public void SortByDueDateManual()
    {
        for (int i = 0; i < _tasks.Count - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < _tasks.Count; j++)
            {
                if (_tasks[j].DueDate < _tasks[minIndex].DueDate)
                    minIndex = j;
            }
            if (minIndex != i)
                (_tasks[i], _tasks[minIndex]) = (_tasks[minIndex], _tasks[i]);
        }
    }

    // 内置排序：按 Priority 升序（1 最优先）
    public void SortByPriorityBuiltIn()
    {
        _tasks.Sort((a, b) => a.Priority.CompareTo(b.Priority));
    }
}
