using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace SharPersist
{
    public class SchTask : Persistence
    {
        public SchTask(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option) : base(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option)
        {
            initialize(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option);

        }

        // initial function to decide which method needs performed and if valid args given
        public void initialize(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option)
        {

            // add persistence
            if (status.ToLower().Equals("add"))
            {
                if (command.Equals("") || theName.Equals(""))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must give both a command and scheduled task name. See help (-h).");
                    return;
                }

                addPersistence(command, commandArg, theName, option);
            }

            // remove persistence
            else if (status.ToLower().Equals("remove"))
            {
                if (theName.Equals(""))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must give a scheduled task name. See help (-h).");
                    return;
                }

                removePersistence(theName);
            }

            // dry run
            else if (status.ToLower().Equals("check"))
            {
                checkPersistence(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status);
            }

            // list persistence
            else if (status.ToLower().Equals("list"))
            {
                listPersistence(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option);
            }

            // invalid method given
            else
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: Invalid method given. Must give add, remove, check or list. See help (-h).");
                return;
            }
        } // end initialize method


        // add persistence trigger
        public void addPersistence(string command, string commandArg, string theName, string option)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Adding scheduled task persistence");
            Console.WriteLine("[*] INFO: Command: " + command);
            Console.WriteLine("[*] INFO: Command Args: " + commandArg);
            Console.WriteLine("[*] INFO: Scheduled Task Name: " + theName);
            Console.WriteLine("[*] INFO: Option: " + option);
            Console.WriteLine("");

            bool schtaskExists = lib.Utils.ScheduledTaskExists(theName);

            // if schtask does not exist
            if (!schtaskExists)
            {

                try
                {
                    TaskService ts = new TaskService();
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = theName;

                    // set trigger time appropriately based on option provided
                    string triggerTime = option.ToLower();

                    // daily schtask
                    if (triggerTime.Equals("daily"))
                    {
                
                        // Create a trigger that runs every day and will start randomly between 10 a.m. and 12 p.m.
                        DailyTrigger dt = new DailyTrigger();
                        dt.StartBoundary = DateTime.Today + TimeSpan.FromHours(10);
                        dt.DaysInterval = 1;
                        dt.RandomDelay = TimeSpan.FromHours(2);

                        td.Triggers.Add(dt);

                    }

                    // hourly schtask
                    else if (triggerTime.Equals("hourly"))
                    {
                        TimeTrigger tt = new TimeTrigger();
                        tt.Repetition.Interval = TimeSpan.FromMinutes(60);
                        td.Triggers.Add(tt);
                    }

                    // schtask at logon. this will run as system
                    else if (triggerTime.Equals("logon"))
                    {
                        td.Triggers.Add(Trigger.CreateTrigger(TaskTriggerType.Logon));
                        td.Principal.UserId = "SYSTEM";
                    }

                    // otherwise to daily by default
                    else
                    {
                        // Create a trigger that runs every day and will start randomly between 10 a.m. and 12 p.m.
                        DailyTrigger dt = new DailyTrigger();
                        dt.StartBoundary = DateTime.Today + TimeSpan.FromHours(10);
                        dt.DaysInterval = 1;
                        dt.RandomDelay = TimeSpan.FromHours(2);

                        td.Triggers.Add(dt);
                    }

                    // Create an action that will launch whenever the trigger fires
                    td.Actions.Add(command, commandArg, null);

                    td.Settings.DisallowStartIfOnBatteries = false;
                    td.Settings.StopIfGoingOnBatteries = false;

                    // Register the task in the root folder
                    ts.RootFolder.RegisterTaskDefinition(theName, td);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine("[-] ERROR: You do not have permissions to create this scheduled task.");
                    return;
                }

                schtaskExists = lib.Utils.ScheduledTaskExists(theName);

                if (schtaskExists)
                {
                    Console.WriteLine("");
                    Console.WriteLine("[+] SUCCESS: Scheduled task added");
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Scheduled task was not added succcessfully");
                }
            }

            // if schtask does already exist
            else
            {
                Console.WriteLine("[-] ERROR: Scheduled task with that name already exists.");
                return;
            }

        } // end add persistence method



        // remove persistence trigger
        public void removePersistence(string theName)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Removing scheduled task persistence");
            Console.WriteLine("[*] INFO: Scheduled Task Name: " + theName);
            Console.WriteLine("");

            bool schtaskExists = lib.Utils.ScheduledTaskExists(theName);


            // if scheduled task exists, then delete it
            if (schtaskExists)
            {

                try
                {
                    TaskService tsToDelete = new TaskService();
                    tsToDelete.RootFolder.DeleteTask(theName);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine("[-] ERROR: You do not have privileges to remove the scheduled task. Please use an admin user to remove scheduled task.");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[-] ERROR: You do not have privileges to remove the scheduled task. Please use an admin user to remove scheduled task.");
                    return;
                }


                schtaskExists = lib.Utils.ScheduledTaskExists(theName);
                if (!schtaskExists)
                {
                    Console.WriteLine("");
                    Console.WriteLine("[+] SUCCESS: Scheduled task removed");
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Scheduled task was not removed succcessfully");
                }

            } // end if schtask exists


            // if scheduled task does not exist, display message
            else
            {
                Console.WriteLine("[-] ERROR: Scheduled task does not exist.");
                return;
            }

        } // end remove persistence method


        // check for persistence technique
        public void checkPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {

            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking if scheduled task already exists");


            bool schtaskExists = lib.Utils.ScheduledTaskExists(theName);

            if (schtaskExists)
            {
                Console.WriteLine("[-] ERROR: A scheduled task with that name already exists.");

            }
            else
            {
                Console.WriteLine("[+] SUCCESS: A scheduled task with that name does not exist.");

            }


            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking for correct arguments given");

            if (command.Equals("") || theName.Equals(""))
            {
                Console.WriteLine("[-] ERROR: Must give both a command and scheduled task name. See help (-h).");
                return;
            }

            Console.WriteLine("[+] SUCCESS: Correct arguments given");

        } // end check persistence method


        // list persistence
        public void listPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option)
        {

            bool nameSpecified = false;
            bool schTaskExists = false;

            // if user specified they only want to list a specific schtask
            if (!theName.Equals(""))
            {
                nameSpecified = true;
                schTaskExists = lib.Utils.ScheduledTaskExists(theName);

                // if schtask exists, then look for that schtask
                if (schTaskExists)
                {
                    Console.WriteLine("");
                    Console.WriteLine("[*] INFO: Listing scheduled task details of name that was specified.");

                    TaskService theTask = new TaskService();
                    IEnumerable<Task> allOfTheTasks = theTask.AllTasks;
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("");

                    foreach (Task task in allOfTheTasks)
                    {

                        string schtaskName = task.Name;
                        DateTime runTime = task.NextRunTime;
                        string theRunTime = runTime.ToString("G", CultureInfo.CurrentCulture);

                        // once we find the schtask, display its details
                        if (schtaskName.ToLower().Equals(theName.ToLower()))
                        {
                            ActionCollection allActions = task.Definition.Actions;

                            // getschtask owner
                            string schtaskAction = allActions.Context;
                            SecurityIdentifier schtaskOwner = task.SecurityDescriptor.Owner;
                            NTAccount ntAccount = (NTAccount)schtaskOwner.Translate(typeof(NTAccount));
                            string owner = ntAccount.ToString();
                            string schtaskFolder = task.Folder.Path;


                            // get the frequency in which the schtask executes
                            TriggerCollection triggers = task.Definition.Triggers;
                            string triggerType = "";
                            foreach (Trigger trigger in triggers)
                            {
                                RepetitionPattern pattern = trigger.Repetition;

                                triggerType = trigger.TriggerType.ToString();
                            }


                            Console.WriteLine("[*] INFO: TASK NAME:");
                            Console.WriteLine(schtaskName);
                            Console.WriteLine("");
                            Console.WriteLine("[*] INFO: TASK PATH:");
                            Console.WriteLine(schtaskFolder);
                            Console.WriteLine("");
                            Console.WriteLine("[*] INFO: TASK OWNER:");
                            Console.WriteLine(owner);
                            Console.WriteLine("");
                            Console.WriteLine("[*] INFO: NEXT RUN TIME:");
                            Console.WriteLine(theRunTime);
                            Console.WriteLine("");
                            Console.WriteLine("[*] INFO: TASK TRIGGER:");
                            Console.WriteLine(triggerType);
                            Console.WriteLine("");


                            // get all actions and print
                            foreach (Microsoft.Win32.TaskScheduler.Action action in allActions)
                            {
                                Console.WriteLine("[*] INFO: TASK ACTION:");
                                Console.WriteLine(action.ToString());
                                Console.WriteLine("");

                            }


                        } // end once we find the schtask

                    } // end for each task

                    return;
                } // end if schtask exists

                // if schtask doesn't exist
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: That scheduled task name does not exist. Please double check the name you provided.");
                    return;

                }

            } // end if user specified they only want to list a specific schtask


            // if user wants to see all schtasks
            else
            {

                // determine whether option was specified
                bool optionSpecified = false;
                if (!option.Equals(""))
                {
                    optionSpecified = true;
                }

                Console.WriteLine("");
                Console.WriteLine("[*] INFO: Listing all scheduled tasks.");


                TaskService ts = new TaskService();
                IEnumerable<Task> allTasks = ts.AllTasks;
                bool schtaskExists = lib.Utils.ScheduledTaskExists(theName);
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");


                foreach (Task task in allTasks)
                {
                    string schtaskName = task.Name;
                    DateTime runTime = task.NextRunTime;
                    string theRunTime = runTime.ToString("G", CultureInfo.CurrentCulture);
                    bool taskActive = task.IsActive;

                    // only proceed to list schtask info if it is active
                    if (taskActive)
                    {
                        // get collection of all actions the schtask performs
                        ActionCollection allActions = task.Definition.Actions;

                        // getschtask owner
                        string schtaskAction = allActions.Context;
                        SecurityIdentifier schtaskOwner = task.SecurityDescriptor.Owner;
                        NTAccount ntAccount = (NTAccount)schtaskOwner.Translate(typeof(NTAccount));
                        string owner = ntAccount.ToString();
                        string schtaskFolder = task.Folder.Path;

                        // get the frequency in which the schtask executes
                        TriggerCollection triggers = task.Definition.Triggers;
                        string triggerType = "";
                        foreach (Trigger trigger in triggers)
                        {
                            RepetitionPattern pattern = trigger.Repetition;
                            triggerType = trigger.TriggerType.ToString();
                       
                        }

                        // if option was specified, only display schtasks with frequency given
                        if (optionSpecified)
                        {

                            if (option.ToLower().Equals("hourly") && triggerType.ToLower().Equals("time"))
                            {
                                Console.WriteLine("[*] INFO: TASK NAME:");
                                Console.WriteLine(schtaskName);
                                Console.WriteLine("");
                                Console.WriteLine("[*] INFO: TASK PATH:");
                                Console.WriteLine(schtaskFolder);
                                Console.WriteLine("");
                                Console.WriteLine("[*] INFO: TASK OWNER:");
                                Console.WriteLine(owner);
                                Console.WriteLine("");
                                Console.WriteLine("[*] INFO: NEXT RUN TIME:");
                                Console.WriteLine(theRunTime);
                                Console.WriteLine("");

                                // get the frequency in which the schtask executes
                                TriggerCollection theTriggers = task.Definition.Triggers;
                                string theTriggerType = "";
                                foreach (Trigger trigger in theTriggers)
                                {
                                    RepetitionPattern pattern = trigger.Repetition;

                                    theTriggerType = trigger.TriggerType.ToString();
                                    Console.WriteLine("[*] INFO: TASK TRIGGER:");
                                    Console.WriteLine(theTriggerType);
                                    Console.WriteLine("");
                                }



                                // get all actions and print
                                foreach (Microsoft.Win32.TaskScheduler.Action action in allActions)
                                {
                                    Console.WriteLine("[*] INFO: TASK ACTION:");
                                    Console.WriteLine(action.ToString());
                                    Console.WriteLine("");

                                }

                                Console.WriteLine("");
                                Console.WriteLine("");
                                Console.WriteLine("");
                            }


                            else if (option.ToLower().Equals("daily") && triggerType.ToLower().Equals("daily"))
                            {
                                Console.WriteLine("[*] INFO: TASK NAME:");
                                Console.WriteLine(schtaskName);
                                Console.WriteLine("");
                                Console.WriteLine("[*] INFO: TASK PATH:");
                                Console.WriteLine(schtaskFolder);
                                Console.WriteLine("");
                                Console.WriteLine("[*] INFO: TASK OWNER:");
                                Console.WriteLine(owner);
                                Console.WriteLine("");
                                Console.WriteLine("[*] INFO: NEXT RUN TIME:");
                                Console.WriteLine(theRunTime);
                                Console.WriteLine("");

                                // get the frequency in which the schtask executes
                                TriggerCollection theTriggers = task.Definition.Triggers;
                                string theTriggerType = "";
                                foreach (Trigger trigger in theTriggers)
                                {
                                    RepetitionPattern pattern = trigger.Repetition;

                                    theTriggerType = trigger.TriggerType.ToString();
                                    Console.WriteLine("[*] INFO: TASK TRIGGER:");
                                    Console.WriteLine(theTriggerType);
                                    Console.WriteLine("");
                                }



                                // get all actions and print
                                foreach (Microsoft.Win32.TaskScheduler.Action action in allActions)
                                {
                                    Console.WriteLine("[*] INFO: TASK ACTION:");
                                    Console.WriteLine(action.ToString());
                                    Console.WriteLine("");

                                }

                                Console.WriteLine("");
                                Console.WriteLine("");
                                Console.WriteLine("");
                            }


                            else if ((option.ToLower().Equals("logon") && triggerType.ToLower().Equals("logon")) || (option.ToLower().Equals("boot") && triggerType.ToLower().Equals("boot")))
                            {
                                Console.WriteLine("[*] INFO: TASK NAME:");
                                Console.WriteLine(schtaskName);
                                Console.WriteLine("");
                                Console.WriteLine("[*] INFO: TASK PATH:");
                                Console.WriteLine(schtaskFolder);
                                Console.WriteLine("");
                                Console.WriteLine("[*] INFO: TASK OWNER:");
                                Console.WriteLine(owner);
                                Console.WriteLine("");
                                Console.WriteLine("[*] INFO: NEXT RUN TIME:");
                                Console.WriteLine(theRunTime);
                                Console.WriteLine("");

                                // get the frequency in which the schtask executes
                                TriggerCollection theTriggers = task.Definition.Triggers;
                                string theTriggerType = "";
                                foreach (Trigger trigger in theTriggers)
                                {
                                    RepetitionPattern pattern = trigger.Repetition;

                                    theTriggerType = trigger.TriggerType.ToString();
                                    Console.WriteLine("[*] INFO: TASK TRIGGER:");
                                    Console.WriteLine(theTriggerType);
                                    Console.WriteLine("");
                                }



                                // get all actions and print
                                foreach (Microsoft.Win32.TaskScheduler.Action action in allActions)
                                {
                                    Console.WriteLine("[*] INFO: TASK ACTION:");
                                    Console.WriteLine(action.ToString());
                                    Console.WriteLine("");

                                }

                                Console.WriteLine("");
                                Console.WriteLine("");
                                Console.WriteLine("");
                            }

                        } // end if option specified

                        // otherwise display as normal
                        else
                        {

                            Console.WriteLine("[*] INFO: TASK NAME:");
                            Console.WriteLine(schtaskName);
                            Console.WriteLine("");
                            Console.WriteLine("[*] INFO: TASK PATH:");
                            Console.WriteLine(schtaskFolder);
                            Console.WriteLine("");
                            Console.WriteLine("[*] INFO: TASK OWNER:");
                            Console.WriteLine(owner);
                            Console.WriteLine("");
                            Console.WriteLine("[*] INFO: NEXT RUN TIME:");
                            Console.WriteLine(theRunTime);
                            Console.WriteLine("");

                            // get the frequency in which the schtask executes
                            TriggerCollection theTriggers = task.Definition.Triggers;
                            string theTriggerType = "";
                            foreach (Trigger trigger in theTriggers)
                            {
                                RepetitionPattern pattern = trigger.Repetition;

                                theTriggerType = trigger.TriggerType.ToString();
                                Console.WriteLine("[*] INFO: TASK TRIGGER:");
                                Console.WriteLine(theTriggerType);
                                Console.WriteLine("");
                            }



                            // get all actions and print
                            foreach (Microsoft.Win32.TaskScheduler.Action action in allActions)
                            {
                                Console.WriteLine("[*] INFO: TASK ACTION:");
                                Console.WriteLine(action.ToString());
                                Console.WriteLine("");

                            }

                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine("");
                        }


                    } // end if schtask is active


                } // end iterating through each schtask

            } // end if user wants to see all schtasks

        } // end list persitence method

    } // end class

} // end namespace
