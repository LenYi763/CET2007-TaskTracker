namespace TaskTracker.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime DueDate { get; set; }
    public int Priority { get; set; } // 1 (highest) ... 5 (lowest)
    public string Assignee { get; set; } = "";
    public TaskState Status { get; set; } = TaskState.Todo;

    public override string ToString()
    {
        return $"[{Id}] {Title} | Due: {DueDate:yyyy-MM-dd} | P{Priority} | {Assignee} | {Status}";
    }
}
