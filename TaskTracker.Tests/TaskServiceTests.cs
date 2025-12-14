using TaskTracker.Models;
using TaskTracker.Services;
using Xunit;

namespace TaskTracker.Tests;

public class TaskServiceTests
{
    private static TaskItem MakeTask(int id, DateTime due, int priority = 3, string title = "t", string assignee = "a")
    {
        return new TaskItem
        {
            Id = id,
            Title = title,
            DueDate = due,
            Priority = priority,
            Assignee = assignee,
            Status = TaskState.Todo
        };
    }

    [Fact]
    public void Add_ShouldAdd_WhenIdIsUnique()
    {
        var svc = new TaskService(null);

        svc.Add(MakeTask(1, new DateTime(2025, 12, 20)));

        Assert.Equal(1, svc.GetAll().Count);
        Assert.Equal(1, svc.GetAll()[0].Id);
    }

    [Fact]
    public void Add_ShouldThrow_WhenIdIsDuplicate()
    {
        var svc = new TaskService(null);
        svc.Add(MakeTask(1, new DateTime(2025, 12, 20)));

        Assert.Throws<ArgumentException>(() =>
            svc.Add(MakeTask(1, new DateTime(2025, 12, 21))));
    }

    [Fact]
    public void FindById_ShouldReturnTask_WhenExists()
    {
        var svc = new TaskService(null);
        svc.Add(MakeTask(7, new DateTime(2025, 12, 20), title: "hello"));

        var found = svc.FindById(7);

        Assert.NotNull(found);
        Assert.Equal("hello", found!.Title);
    }

    [Fact]
    public void UpdateStatus_ShouldChangeStatus_WhenIdExists()
    {
        var svc = new TaskService(null);
        svc.Add(MakeTask(2, new DateTime(2025, 12, 20)));

        var ok = svc.UpdateStatus(2, TaskState.InProgress);

        Assert.True(ok);
        Assert.Equal(TaskState.InProgress, svc.FindById(2)!.Status);
    }

    [Fact]
    public void SortByDueDateManual_ShouldSortAscending()
    {
        var svc = new TaskService(null);
        svc.Add(MakeTask(1, new DateTime(2025, 12, 21)));
        svc.Add(MakeTask(2, new DateTime(2025, 12, 19)));
        svc.Add(MakeTask(3, new DateTime(2025, 12, 20)));

        svc.SortByDueDateManual();

        var all = svc.GetAll();
        Assert.Equal(2, all[0].Id); // 12/19
        Assert.Equal(3, all[1].Id); // 12/20
        Assert.Equal(1, all[2].Id); // 12/21
    }
}
