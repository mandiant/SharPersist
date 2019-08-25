using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.ServiceProcess;

namespace SharPersist.lib
{
    public class Utils
    {

        /**
        * Parse command line arguments
        * 
        * */
        public static Dictionary<string, string> ParseArgs(string[] args) 
        {
            try
            {
                Dictionary<string, string> ret = new Dictionary<string, string>();
                if (args.Length % 2 == 0 && args.Length > 0)
                {
                    for (int i = 0; i < args.Length; i = i + 2)
                    {
                        ret.Add(args[i].Substring(1).ToLower(), args[i + 1]);

                    }
                }
                return ret;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[-] You specified duplicate switches. Check your command again.");
                return null;
            }

        } // end ParseArgs method


       /**
       * print help
       * 
       * */
        public static void PrintHelp()
        {
            Console.Write("\nSharPersist (v1.0) - C# Windows Persistence Toolkit\nAuthor: Brett Hawkins\n\n");
            Console.Write("DESCRIPTION:\n\n\tWindows persistence toolkit written in C#\n\n");
            Console.Write("SWITCHES:\n\n");
            Console.Write("\t-t: persistence technique\n\n");
            Console.Write("\t-c: command to execute\n\n");
            Console.Write("\t-a: arguments to command to execute (if applicable)\n\n");
            Console.Write("\t-f: the file to create/modify/backdoor\n\n");
            Console.Write("\t-k: registry key to create/modify\n\n");
            Console.Write("\t-v: registry value to create/modify\n\n");
            Console.Write("\t-n: scheduled task or service name\n\n");
            Console.Write("\t-m: method (add, remove, check, list)\n\n");
            Console.Write("\t-o: optional add-ons\n\n");
            Console.Write("\t-h: help page\n\n");

            Console.Write("TECHNIQUES:\n\n");
            Console.Write("\tkeepass: backdoor keepass config file\n\n");
            Console.Write("\treg: registry key addition/modification\n\n");
            Console.Write("\tschtaskbackdoor: backdoor scheduled task\n\n");
            Console.Write("\tstartupfolder: lnk file in startup folder\n\n");
            Console.Write("\ttortoisesvn: tortoise svn hook script\n\n");
            Console.Write("\tservice: create new windows service\n\n");
            Console.Write("\tschtask: create new scheduled task\n\n");

            Console.Write("METHODS:\n\n");
            Console.Write("\tadd: add persistence technique\n\n");
            Console.Write("\tremove: remove persistence technique\n\n");
            Console.Write("\tcheck: perform dry-run of persistence technique\n\n");
            Console.Write("\tlist: list current entries for persistence technique\n\n");

            Console.Write("OPTIONAL ADD-ONS:\n\n");
            Console.Write("\tenv: optional add-on for env variable obfuscation for registry\n\n");
            Console.Write("\thourly: optional add-on for schtask frequency\n\n");
            Console.Write("\tdaily: optional add-on for schtask frequency\n\n");
            Console.Write("\tlogon: optional add-on for schtask frequency\n\n");

            Console.Write("REGISTRY KEYS:\n\n");
            Console.Write("\thklmrunonce\n\n");
            Console.Write("\thklmrunonceex\n\n");
            Console.Write("\thklmrun\n\n");
            Console.Write("\thkcurun\n\n");
            Console.Write("\thkcurunonce\n\n");
            Console.Write("\tlogonscript (reg value not needed)\n\n");
            Console.Write("\tstickynotes (reg value not needed)\n\n");
            Console.Write("\tuserinit (reg value not needed)\n\n");

            Console.Write("USAGE/EXAMPLES(add):\n\n");
            Console.Write("\t-t keepass -c \"<command>\" -a \"<arg>\" -f \"<file path>\" -m add\n\n");
            Console.Write("\t-t reg -c \"<command>\" -a \"<arg>\" -k \"<reg key>\" -v \"<reg value>\" -m add\n\n");
            Console.Write("\t-t reg -c \"<command>\" -a \"<arg>\" -k \"<pre-determined reg key>\" -m add\n\n");
            Console.Write("\t-t schtaskbackdoor -c \"<command>\" -a \"<arg>\" -n \"<schtask name>\" -m add\n\n");
            Console.Write("\t-t schtask -c \"<command>\" -a \"<arg>\" -n \"<schtask name>\" -m add\n\n");
            Console.Write("\t-t schtask -c \"<command>\" -a \"<arg>\" -n \"<schtask name>\" -m add -o <frequency>\n\n");
            Console.Write("\t-t startupfolder -c \"<command>\" -a \"<arg>\" -f \"<file name>\" -m add\n\n");
            Console.Write("\t-t tortoisesvn -c \"<command>\" -a \"<arg>\" -m add\n\n");
            Console.Write("\t-t service -c \"<command>\" -a \"<arg>\" -n \"<service name>\" -m add\n\n");

            Console.Write("USAGE/EXAMPLES(remove):\n\n");
            Console.Write("\t-t keepass -f \"<file path>\" -m remove\n\n");
            Console.Write("\t-t reg -k \"<reg key>\" -v \"<reg value>\" -m remove\n\n");
            Console.Write("\t-t reg -k \"<pre-determined reg key>\" -m remove\n\n");
            Console.Write("\t-t schtaskbackdoor -n \"<schtask name>\" -m remove\n\n");
            Console.Write("\t-t schtask -n \"<schtask name>\" -m remove\n\n");
            Console.Write("\t-t startupfolder -f \"<file name>\" -m remove\n\n");
            Console.Write("\t-t tortoisesvn -m remove\n\n");
            Console.Write("\t-t service -n \"<service name>\" -m remove\n\n");

            Console.Write("USAGE/EXAMPLES(check):\n\n");
            Console.Write("\t-t keepass -c \"<command>\" -a \"<arg>\" -f \"<file path>\" -m check\n\n");
            Console.Write("\t-t reg -c \"<command>\" -a \"<arg>\" -k \"<reg key>\" -v \"<reg value>\" -m check\n\n");
            Console.Write("\t-t schtaskbackdoor -c \"<command>\" -a \"<arg>\" -n \"<schtask name>\" -m check\n\n");
            Console.Write("\t-t schtask -c \"<command>\" -a \"<arg>\" -n \"<schtask name>\" -m check\n\n");
            Console.Write("\t-t startupfolder -c \"<command>\" -a \"<arg>\" -f \"<file name>\" -m check\n\n");
            Console.Write("\t-t tortoisesvn -c \"<command>\" -a \"<arg>\" -m check\n\n");
            Console.Write("\t-t service -c \"<command>\" -a \"<arg>\" -n \"<service name>\" -m check\n\n");

            Console.Write("USAGE/EXAMPLES(list):\n\n");
            Console.Write("\t-t startupfolder -m list\n\n");
            Console.Write("\t-t schtaskbackdoor -m list\n\n");
            Console.Write("\t-t schtaskbackdoor -m list -n <schtask name>\n\n");
            Console.Write("\t-t schtask -m list\n\n");
            Console.Write("\t-t schtask -m list -n <schtask name>\n\n");
            Console.Write("\t-t schtask -m list -o <frequency>\n\n");
            Console.Write("\t-t service -m list\n\n");
            Console.Write("\t-t service -m list -n <service name>\n\n");
            Console.Write("\t-t reg -m list -k <reg key>\n\n");


        } // and print help method



        /**
        * Get SHA256 hash of file
        * 
        * */
        public static string SHA256CheckSum(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                SHA256Managed sha = new SHA256Managed();
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        } // end getting SHA256 sum of file



        /**
         * Returns whether a given registry value exists
         * 
         * 
         * */
        public static bool RegistryValueExists(string hive_HKLM_or_HKCU, string registryRoot, string valueName) 
        {
            try
            {
                RegistryKey key = null;
                registryRoot = registryRoot.Substring(5, registryRoot.Length - 5); // start after the hive specification

                // if the registry hive is HKLM
                if (hive_HKLM_or_HKCU.ToUpper().Equals("HKLM"))
                {

                    // try and open the registry key and get the value contained inside
                    key = Registry.LocalMachine.OpenSubKey(registryRoot);
                    object o = key.GetValue(valueName);

                    // if the value exists, return true
                    if (o != null)
                    {
                        return true;
                    }

                    // if the value does not exist, return false
                    else
                    {
                        return false;
                    }
                } // end if reg hive is hklm


                // if registry hive is HKCU
                else if (hive_HKLM_or_HKCU.ToUpper().Equals("HKCU"))
                {

                    // try and open the registry key and get the value contained inside
                    key = Registry.CurrentUser.OpenSubKey(registryRoot);
                    object o = key.GetValue(valueName);

                    // if the value exists, return true
                    if (o != null)
                    {
                        return true;
                    }

                    // if the value does not exist, return false
                    else
                    {
                        return false;
                    }

                } // end if reg hive is hkcu

            } // end try

            catch (NullReferenceException ex) {

                //Console.WriteLine("[-] ERROR: Registry key provided does not exist");
                return false;

            }
            catch (SecurityException)
            {
                return false;
            }

            return false;


        } // end registryValueExists method


        /**
         * Returns whether you can write to a given reg key
         * 
         * 
         * */
        public static bool CanWriteKey(string key)
        {
            try
            {
                string hive = key.Substring(0, 4); // get reg hive (HKLM, HKCU)
                string registryRoot = key.Substring(5, key.Length - 5); // start after the hive specification

                if (hive.ToLower().Equals("hklm"))
                {
                    Registry.LocalMachine.OpenSubKey(registryRoot, true).Close(); // try and open reg key, if it fails an exception will be thrown in which case you do not have write access
                }
                else if (hive.ToLower().Equals("hkcu"))
                {
                    Registry.CurrentUser.OpenSubKey(registryRoot, true).Close(); // try and open reg key, if it fails an exception will be thrown in which case you do not have write access
                }
                return true;


            }
            catch (SecurityException)
            {
                return false;
            }
            catch (NullReferenceException ex)
            {

                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: Registry key provided does not exist");
                return false;

            }
        } // end can write key method


        /**
          * Returns whether a scheduled task exists
          * 
          * 
          * */
        public static bool ScheduledTaskExists(string schtaskName)
        {

            schtaskName = schtaskName.ToLower();
            TaskService ts = new TaskService();
            IEnumerable<Task> allTasks = ts.AllTasks; // get all scheduled tasks
            bool schtaskExists = false;
            foreach (Task task in allTasks)
            {
                // if scheduled task name matches, change value to true
                if (task.Name.ToLower().Equals(schtaskName))
                {
                    schtaskExists = true;

                }
            } // end for each scheduled task

            return schtaskExists;

        } // end scheduled task exists method


        /**
          * Returns whether a service exists
          * 
          * 
          * */
        public static bool ServiceExists(string serviceName)
        {
            serviceName = serviceName.ToLower();
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            bool serviceExists = false;
            foreach (ServiceController service in scServices)
            {
                if (service.ServiceName.ToLower().Equals(serviceName))
                {
                    serviceExists = true;
                }

            }

            return serviceExists;
        } // end serviceExists method


        /**
         * Get the full reg key for the code that is input
         * 
         * 
         * */
        public static string getRegKeyMapping(string regCode)
        {
            string returnString = "";
            regCode = regCode.ToLower();

            Hashtable tableOfMapping = new Hashtable();
            tableOfMapping.Add("hklmrunonce", @"HKLM\Software\Microsoft\Windows\CurrentVersion\RunOnce");
            tableOfMapping.Add("hklmrunonceex", @"HKLM\Software\Microsoft\Windows\CurrentVersion\RunOnceEx");
            tableOfMapping.Add("hklmrun", @"HKLM\Software\Microsoft\Windows\CurrentVersion\Run");
            tableOfMapping.Add("hkcurun", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Run");
            tableOfMapping.Add("hkcurunonce", @"HKCU\Software\Microsoft\Windows\CurrentVersion\RunOnce");
            tableOfMapping.Add("logonscript", @"HKCU\Environment");
            tableOfMapping.Add("stickynotes", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Run");
            tableOfMapping.Add("userinit", @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            if (tableOfMapping.ContainsKey(regCode))
            {
                returnString = (string)tableOfMapping[regCode];
            }

            return returnString;

        } // end getRegKeyMapping


        /**
         * Get the reg value for pre-definded reg keys
         * 
         * 
         * */
        public static string getRegValueMapping(string regCode)
        {
            string returnString = "";
            regCode = regCode.ToLower();

            Hashtable tableOfMapping = new Hashtable();
            tableOfMapping.Add("logonscript", "UserInitMprLogonScript");
            tableOfMapping.Add("stickynotes", "RESTART_STICKY_NOTES");
            tableOfMapping.Add("userinit", "Userinit");

            if (tableOfMapping.ContainsKey(regCode))
            {
                returnString = (string)tableOfMapping[regCode];
            }

            return returnString;

        } // end getRegKeyMapping





    } // end class


} // end namespace
