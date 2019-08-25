using System;
using System.Collections.Generic;

namespace SharPersist
{
    public class SharPersist
    {

        // variables used
        private static string persistTechnique = "";
        private static string command = "";
        private static string commandArg = "";
        private static string filePath = "";
        private static string theKey = "";
        private static string theVal = "";
        private static string theName = "";
        private static string method = "";
        private static string option = "";


        public static void Main(string[] args)
        {

            try
            {
                Dictionary<string, string> argDict = lib.Utils.ParseArgs(args); // dictionary to hold arguments


                // if no arguments given, display help and return
                if ((args.Length > 0 && argDict.Count == 0) || argDict.ContainsKey("h"))
                {
                    lib.Utils.PrintHelp();
                    return;
                }

                // if method and status not given, display help and return
                if (!argDict.ContainsKey("t") || !argDict.ContainsKey("m"))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must supply technique and method variables. See help (-h).");
                    return;

                }

                // initialize variables
                persistTechnique = argDict["t"];
                if (argDict.ContainsKey("c"))
                {
                    command = argDict["c"];
                }
                if (argDict.ContainsKey("a"))
                {
                    commandArg = argDict["a"];
                }
                if (argDict.ContainsKey("f"))
                {
                    filePath = argDict["f"];
                }
                if (argDict.ContainsKey("k"))
                {
                    theKey = argDict["k"];
                }
                if (argDict.ContainsKey("v"))
                {
                    theVal = argDict["v"];
                }
                if (argDict.ContainsKey("n"))
                {
                    theName = argDict["n"];
                }
                if (argDict.ContainsKey("o"))
                {
                    option = argDict["o"];
                }
                method = argDict["m"];


                // initialize appropriate persistence object
          

                // keepass persistence
                if (persistTechnique.ToLower().Equals("keepass"))
                {
                    KeePassBackdoor keepass = new KeePassBackdoor(persistTechnique, command, commandArg, theKey, theVal, theName, filePath, method, option);

                }

                // registry persistence
                else if (persistTechnique.ToLower().Equals("reg"))
                {
                    RegistryPersist registry = new RegistryPersist(persistTechnique, command, commandArg, theKey, theVal, theName, filePath, method, option);

                }

                // schtaskbackdoor persistence
                else if (persistTechnique.ToLower().Equals("schtaskbackdoor"))
                {
                    SchTaskBackdoor schtaskBackdoor = new SchTaskBackdoor(persistTechnique, command, commandArg, theKey, theVal, theName, filePath, method, option);

                }

                // startupfolder persistence
                else if (persistTechnique.ToLower().Equals("startupfolder"))
                {
                    StartupFolder startupFolder = new StartupFolder(persistTechnique, command, commandArg, theKey, theVal, theName, filePath, method, option);

                }

                // tortoisesvn persistence
                else if (persistTechnique.ToLower().Equals("tortoisesvn"))
                {
                    TortoiseSVNHookScripts tortoiseSVN = new TortoiseSVNHookScripts(persistTechnique, command, commandArg, theKey, theVal, theName, filePath, method, option);

                }

                // service persistence
                else if (persistTechnique.ToLower().Equals("service"))
                {
                    Service service = new Service(persistTechnique, command, commandArg, theKey, theVal, theName, filePath, method, option);

                }

                // schtask persistence
                else if (persistTechnique.ToLower().Equals("schtask"))
                {
                    SchTask schtask = new SchTask(persistTechnique, command, commandArg, theKey, theVal, theName, filePath, method, option);

                }

                // if invalid method was given
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] Invalid technique given. See help (-h).");
                    return;

                }

            } // end try

            catch (NullReferenceException ex)
            {

            }


        } // end main

    } // end class

} // end namespace
