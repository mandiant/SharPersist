## About
The original and most popular .NET wrapper for the [Windows Task Scheduler](https://docs.microsoft.com/en-us/windows/win32/taskschd/task-scheduler-start-page). It provides functionally complete classes that cover all development aspects related to system tasks.

More information can be found on the [project page on GitHub](https://github.com/dahall/taskscheduler).

## Support
Below are links to sites that provide in-depth examples, documentation and discussions. Please go here first with your questions as the community has been active for over a decade.
* [Wiki](https://github.com/dahall/TaskScheduler/wiki) - Sample code, library how-to, troubleshooting, etc.
* [API documentation](https://dahall.github.io/TaskScheduler) - Class/method/property documentation and examples
* [Full Issues Log](https://github.com/dahall/TaskScheduler/issues?q=) - Use the search box to see if your question may already be answered.
* [Discussion Forum](https://github.com/dahall/TaskScheduler/discussions) - Users helping users, enhancement requests, Q&A (retired Google forum [here](https://groups.google.com/forum/#!forum/taskscheduler))
* [Troubleshooting Tool](https://github.com/dahall/TaskSchedulerConfig) - Tool to help identify and fix configuration and connectivity issues. (ClickOnce installer [here](https://github.com/dahall/TaskSchedulerConfig/blob/master/publish/setup.exe?raw=true))

## Key Features
Microsoft introduced version 2.0 (internally version 1.2) with a completely new object model with Windows Vista. The managed assembly closely resembles the new object model but allows the 1.0 (internally version 1.1) COM objects to be manipulated. It will automatically choose the most recent version of the library found on the host system (up through 1.4). Core features include:

* Separate, functionally identical, libraries for .NET 2.0, 3.5, 4.0, 4.52, 5.0, 6.0, .NET Standard 2.0, .NET Core 2.0, 2.1, 3.0, 3.1.
* Unlike the base COM libraries, this wrapper accommodates creating and viewing tasks up and down stream.
* Supports all V2 native properties, even under V1 tasks.
* Maintain EmailAction and ShowMessageAction using PowerShell scripts for systems after Win8 where these actions have been deprecated.
* Supports all action types (not just ExecAction) on V1 systems (XP/WS2003) and earlier (if PowerShell is installed).
* Supports multiple actions on V1 systems (XP/WS2003). Native library only supports a single action.
* Supports serialization to XML for both 1.0 and 2.0 tasks (base library only supports 2.0)
* Supports task validation for targeted version.
* Supports secure task reading and maintenance.
* Fluent methods for task creation.
* Cron syntax for trigger creation.

The currently supported localizations include: English, Spanish, Italian, French, Chinese (Simplified), German, Polish and Russian.

## Usage
You can perform several actions in a single line of code:  
```C#
// Run a program every day on the local machine
TaskService.Instance.AddTask("Test", QuickTriggerType.Daily, "myprogram.exe", "-a arg");

// Run a custom COM handler on the last day of every month
TaskService.Instance.AddTask("Test", new MonthlyTrigger { RunOnLastDayOfMonth = true }, 
    new ComHandlerAction(new Guid("{CE7D4428-8A77-4c5d-8A13-5CAB5D1EC734}")));
```

For many more options, use the library classes to build a complex task. Below is a brief example of how to use the library from C#.  
```C#
using System;
using Microsoft.Win32.TaskScheduler;

class Program
{
   static void Main()
   {
      // Get the service on the remote machine
      using (TaskService ts = new TaskService(@"\\RemoteServer", "username", "domain", "password"))
      {
         // Create a new task definition and assign properties
         TaskDefinition td = ts.NewTask();
         td.RegistrationInfo.Description = "Does something";

         // Create a trigger that will fire the task at this time every other day
         td.Triggers.Add(new DailyTrigger { DaysInterval = 2 });

         // Create an action that will launch Notepad whenever the trigger fires
         td.Actions.Add(new ExecAction("notepad.exe", "c:\\test.log", null));

         // Register the task in the root folder.
         // (Use the username here to ensure remote registration works.)
         ts.RootFolder.RegisterTaskDefinition(@"Test", td, TaskCreation.CreateOrUpdate, "username");
      }
   }
}
```

For extended examples on how to the use the library, look at the [Examples Page](https://github.com/dahall/TaskScheduler/wiki/Examples).