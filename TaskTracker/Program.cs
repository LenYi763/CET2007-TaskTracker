using TaskTracker.Models;

var sample = new TaskItem
{
    Id = 1,
    Title = "Finish CET2007 assignment skeleton",
    DueDate = DateTime.Today.AddDays(3),
    Priority = 2,
    Assignee = "Me",
    Status = TaskState.Todo
};

Console.WriteLine("TaskTracker started.");
Console.WriteLine(sample);
