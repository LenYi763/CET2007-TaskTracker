using TaskTracker.Logging;
using TaskTracker.Persistence;
using TaskTracker.Services;
using TaskTracker.UI;

var logger = new Logger("log.txt");
var service = new TaskService(logger);
var repo = new JsonTaskRepository("tasks.json");

logger.Log("TaskTracker started");

// 启动时自动加载一次
service.ReplaceAll(repo.Load());

var menu = new ConsoleMenu(service, repo, logger);
menu.Run();
