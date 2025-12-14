using TaskTracker.Models;
using TaskTracker.Services;

var service = new TaskService();

service.Add(new TaskItem
{
    Id = 1,
    Title = "Write TaskService",
    DueDate = DateTime.Today.AddDays(2),
    Priority = 1,
    Assignee = "Me",
    Status = TaskState.Todo
});

service.Add(new TaskItem
{
    Id = 2,
    Title = "Create unit tests",
    DueDate = DateTime.Today.AddDays(5),
    Priority = 2,
    Assignee = "Me",
    Status = TaskState.Todo
});

service.Add(new TaskItem
{
    Id = 3,
    Title = "Record screencast",
    DueDate = DateTime.Today.AddDays(7),
    Priority = 3,
    Assignee = "Me",
    Status = TaskState.Todo
});

Console.WriteLine("=== All Tasks ===");
foreach (var t in service.GetAll()) Console.WriteLine(t);

Console.WriteLine("\n=== Search: 'test' ===");
foreach (var t in service.SearchByKeyword("test")) Console.WriteLine(t);

Console.WriteLine("\n=== Manual Sort by DueDate ===");
service.SortByDueDateManual();
foreach (var t in service.GetAll()) Console.WriteLine(t);

Console.WriteLine("\n=== Update Status ID=2 => InProgress ===");
service.UpdateStatus(2, TaskState.InProgress);
Console.WriteLine(service.FindById(2));

Console.WriteLine("\n=== Delete ID=1 ===");
service.Delete(1);
foreach (var t in service.GetAll()) Console.WriteLine(t);
