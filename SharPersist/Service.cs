using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Configuration.Install;
using Microsoft.Win32;
using System.Data;

namespace SharPersist
{
    public class Service : Persistence
    {
        public Service(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status, string option) : base(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status, option)
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
                    Console.WriteLine("[-] ERROR: Must give both a command and service name. See help (-h).");
                    return;
                }

                addPersistence(command, commandArg, theName);
            }

            else if (status.ToLower().Equals("remove"))
            {
                if (theName.Equals(""))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Must give a service name. See help (-h).");
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
                listPersistence(persistMethod, command, commandArg, theKey, theVal, theName, filePath, status);
            }

            else
            {

                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: Invalid method given. Must give add, remove, check or list. See help (-h).");
                return;
            }

        } // end initialize method


        // add persistences
        public void addPersistence(string command, string commandArg, string theName)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Adding service persistence");
            Console.WriteLine("[*] INFO: Command: " + command);
            Console.WriteLine("[*] INFO: Command Args: " + commandArg);
            Console.WriteLine("[*] INFO: Service Name: " + theName);
            Console.WriteLine("");

            bool serviceExists = lib.Utils.ServiceExists(theName);

            // if service doesn't exist, then add it
            if (!serviceExists)
            {

                try
                {


                    // open a handle to the service control manager
                    using (var scmHandle = lib.NativeMethods.OpenSCManager(Environment.MachineName, null, lib.NativeMethods.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE))
                    {

                        // if unable to open service manager, display message and return
                        if (scmHandle.IsInvalid)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("[-] ERROR: Unable to open service manager on host.");
                            return;

                        }

                        else
                        {
                            try
                            {

                                // if able to open service manager, continue to create the service
                                var serviceHandle = lib.NativeMethods.CreateService(
                                    scmHandle,
                                    theName,
                                    theName,
                                    lib.NativeMethods.SERVICE_ACCESS.SERVICE_ALL_ACCESS,
                                    lib.NativeMethods.SERVICE_TYPES.SERVICE_WIN32_OWN_PROCESS,
                                    lib.NativeMethods.SERVICE_START_TYPES.SERVICE_AUTO_START,
                                    lib.NativeMethods.SERVICE_ERROR_CONTROL.SERVICE_ERROR_IGNORE,
                                    command + " " + commandArg,
                                    null,
                                    IntPtr.Zero,
                                    null,
                                    null,
                                    null);
                            }
                            catch (Exception ex)
                            {

                                Console.WriteLine("");
                                Console.WriteLine("[-] ERROR: Unable to create service.");
                                return;
                            }

                        }

                    }

                }

                catch (Exception ex)
                {
                    Console.WriteLine("[-] ERROR: Admin privileges are needed to add a service. Please run as an admin user in high integrity.");
                    return;
                }

                // make sure service did get installed
                serviceExists = lib.Utils.ServiceExists(theName);
                if (serviceExists)
                {
                    Console.WriteLine("");
                    Console.WriteLine("[+] SUCCESS: Service persistence added");
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: Service not added successfully");
                }

            } // end if service doesn't exist


            // if service does exist, display message
            else
            {
                Console.WriteLine("");
                Console.WriteLine("[-] ERROR: Service with that name already exists");
                return;

            }

        } // end add persistence 


        // remove persistence method
        public void removePersistence(string theName)
        {
            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Removing service persistence");
            Console.WriteLine("[*] INFO: Service Name: " + theName);
            Console.WriteLine("");

            bool serviceExists = lib.Utils.ServiceExists(theName);

            // only remove if service exists
            if (serviceExists)
            {

                try
                {

                    // open a handle to the service control manager
                    using (var scmHandle = lib.NativeMethods.OpenSCManager(Environment.MachineName, null, lib.NativeMethods.SCM_ACCESS.SC_MANAGER_ALL_ACCESS))
                    {

                        // if unable to open service manager, display message and return
                        if (scmHandle.IsInvalid)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("[-] ERROR: Unable to open service manager on machine.");
                            return;

                        }

                        else
                        {

                            try
                            {
                                var serviceHandle = lib.NativeMethods.OpenService(scmHandle, theName, lib.NativeMethods.SERVICE_ACCESS.SERVICE_ALL_ACCESS);
                                lib.NativeMethods.DeleteService(serviceHandle);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine("[-] ERROR: Unable to delete service.");
                                return;
                            }
                        }

                    } // end using handle


                }

                catch (ArgumentException ex)
                {
                    Console.WriteLine("[-] ERROR: Service has already been removed.");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[-] ERROR: Admin privileges are needed to remove a service. Please run as an admin user in high integrity.");
                    return;
                }

                Console.WriteLine("");
                Console.WriteLine("[+] SUCCESS: Service persistence removed.");

            } // end if service exists

            // if service does not exist
            else
            {
                Console.WriteLine("[-] ERROR: That service does not exist to remove.");
                return;
            }

        } // end remove persistence 



        // check for persistence technique
        public void checkPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {

            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking if service with that name already exists");


            bool serviceExists = lib.Utils.ServiceExists(theName);


            if (serviceExists)
            {
                Console.WriteLine("[-] ERROR: Service with that name already exists.");

            }
            else
            {
                Console.WriteLine("[+] SUCCESS: Service with that name does NOT exist.");

            }



            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking for correct arguments given");

            if (command.Equals("") || theName.Equals(""))
            {
                Console.WriteLine("[-] ERROR: Must give both a command and service name. See help (-h).");
                return;
            }

            Console.WriteLine("[+] SUCCESS: Correct arguments given");


            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Checking that current user has administrative privileges");

            bool isUserAdmin = lib.Utils.IsUserAnAdmin();

            if (isUserAdmin)
            {
                Console.WriteLine("[+] SUCCESS: Current user has administrative privileges");

            }
            else
            {
                Console.WriteLine("[-] ERROR: Current user does NOT have administrative privileges. Ensure you are running in high integrity context.");
            }



        } // end check persistences


        // list persistence
        public void listPersistence(string persistMethod, string command, string commandArg, string theKey, string theVal, string theName, string filePath, string status)
        {


            // if user specified they only want to list a specific schtask
            if (!theName.Equals(""))
            {
                // if the service exists
                if (lib.Utils.ServiceExists(theName))
                {
                    Console.WriteLine("");
                    Console.WriteLine("[*] INFO: Listing service name provided.");

                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("");

                    ServiceController[] theServices;
                    theServices = ServiceController.GetServices();
                    foreach (ServiceController service in theServices)
                    {
                        if (service.ServiceName.ToLower().Equals(theName.ToLower()))
                        {
                            Console.WriteLine("[*] INFO: SERVICE NAME:");
                            Console.WriteLine(service.ServiceName);
                            Console.WriteLine("");

                            Console.WriteLine("[*] INFO: DISPLAY NAME:");
                            Console.WriteLine(service.DisplayName);
                            Console.WriteLine("");


                            Console.WriteLine("[*] INFO: STATUS:");
                            Console.WriteLine(service.Status.ToString());
                            Console.WriteLine("");


                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine("");
                        }

                    } // end iterating through services

                    return;


                } // end if the service exists

                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("[-] ERROR: That service name does not exist. Please double check the name you provided.");
                    return;
                }

            } // end if user specified they only want to list a specific schtask


            Console.WriteLine("");
            Console.WriteLine("[*] INFO: Listing all services.");

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");

            ServiceController[] scServices;
            scServices = ServiceController.GetServices();

            foreach (ServiceController service in scServices)
            {


                Console.WriteLine("[*] INFO: SERVICE NAME:");
                Console.WriteLine(service.ServiceName);
                Console.WriteLine("");

                Console.WriteLine("[*] INFO: DISPLAY NAME:");
                Console.WriteLine(service.DisplayName);
                Console.WriteLine("");


                Console.WriteLine("[*] INFO: STATUS:");
                Console.WriteLine(service.Status.ToString());
                Console.WriteLine("");


                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");



            } // end iterating through services


        } // end list persistence method

    } // end class

} // end namespace
