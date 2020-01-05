using System;
using System.Runtime.InteropServices;


namespace SharPersist.lib
{
    class NativeMethods
    {

        [DllImport("advapi32.dll")]
        public static extern ServiceControlHandle OpenSCManager(string lpMachineName, string lpSCDB, SCM_ACCESS scParameter);

        [DllImport("Advapi32.dll")]
        public static extern ServiceControlHandle CreateService(
            ServiceControlHandle serviceControlManagerHandle,
            string lpSvcName,
            string lpDisplayName,
            SERVICE_ACCESS dwDesiredAccess,
            SERVICE_TYPES dwServiceType,
            SERVICE_START_TYPES dwStartType,
            SERVICE_ERROR_CONTROL dwErrorControl,
            string lpPathName,
            string lpLoadOrderGroup,
            IntPtr lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword);

        [DllImport("advapi32.dll")]
        public static extern bool CloseServiceHandle(IntPtr serviceHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ControlService(IntPtr hService, SERVICE_CONTROL dwControl, ref SERVICE_STATUS lpServiceStatus);

        [DllImport("advapi32.dll")]
        public static extern int StartService(ServiceControlHandle serviceHandle, int dwNumServiceArgs, string lpServiceArgVectors);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern ServiceControlHandle OpenService(ServiceControlHandle serviceControlManagerHandle, string lpSvcName, SERVICE_ACCESS dwDesiredAccess);

        [DllImport("advapi32.dll")]
        public static extern int DeleteService(ServiceControlHandle serviceHandle);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        [Flags]
        public enum ACCESS_MASK : uint
        {
            DELETE = 0x00010000,

            READ_CONTROL = 0x00020000,

            WRITE_DAC = 0x00040000,

            WRITE_OWNER = 0x00080000,

            SYNCHRONIZE = 0x00100000,

            STANDARD_RIGHTS_REQUIRED = 0x000F0000,

            STANDARD_RIGHTS_READ = 0x00020000,

            STANDARD_RIGHTS_WRITE = 0x00020000,

            STANDARD_RIGHTS_EXECUTE = 0x00020000,

            STANDARD_RIGHTS_ALL = 0x001F0000,

            SPECIFIC_RIGHTS_ALL = 0x0000FFFF,

            ACCESS_SYSTEM_SECURITY = 0x01000000,

            MAXIMUM_ALLOWED = 0x02000000,

            GENERIC_READ = 0x80000000,

            GENERIC_WRITE = 0x40000000,

            GENERIC_EXECUTE = 0x20000000,

            GENERIC_ALL = 0x10000000,

            DESKTOP_READOBJECTS = 0x00000001,

            DESKTOP_CREATEWINDOW = 0x00000002,

            DESKTOP_CREATEMENU = 0x00000004,

            DESKTOP_HOOKCONTROL = 0x00000008,

            DESKTOP_JOURNALRECORD = 0x00000010,

            DESKTOP_JOURNALPLAYBACK = 0x00000020,

            DESKTOP_ENUMERATE = 0x00000040,

            DESKTOP_WRITEOBJECTS = 0x00000080,

            DESKTOP_SWITCHDESKTOP = 0x00000100,

            WINSTA_ENUMDESKTOPS = 0x00000001,

            WINSTA_READATTRIBUTES = 0x00000002,

            WINSTA_ACCESSCLIPBOARD = 0x00000004,

            WINSTA_CREATEDESKTOP = 0x00000008,

            WINSTA_WRITEATTRIBUTES = 0x00000010,

            WINSTA_ACCESSGLOBALATOMS = 0x00000020,

            WINSTA_EXITWINDOWS = 0x00000040,

            WINSTA_ENUMERATE = 0x00000100,

            WINSTA_READSCREEN = 0x00000200,

            WINSTA_ALL_ACCESS = 0x0000037F
        }

        [Flags]
        public enum SCM_ACCESS : uint
        {
            /// <summary>
            /// Required to connect to the service control manager.
            /// </summary>
            SC_MANAGER_CONNECT = 0x00001,

            /// <summary>
            /// Required to call the CreateService function to create a service
            /// object and add it to the database.
            /// </summary>
            SC_MANAGER_CREATE_SERVICE = 0x00002,

            /// <summary>
            /// Required to call the EnumServicesStatusEx function to list the 
            /// services that are in the database.
            /// </summary>
            SC_MANAGER_ENUMERATE_SERVICE = 0x00004,

            /// <summary>
            /// Required to call the LockServiceDatabase function to acquire a 
            /// lock on the database.
            /// </summary>
            SC_MANAGER_LOCK = 0x00008,

            /// <summary>
            /// Required to call the QueryServiceLockStatus function to retrieve 
            /// the lock status information for the database.
            /// </summary>
            SC_MANAGER_QUERY_LOCK_STATUS = 0x00010,

            /// <summary>
            /// Required to call the NotifyBootConfigStatus function.
            /// </summary>
            SC_MANAGER_MODIFY_BOOT_CONFIG = 0x00020,

            /// <summary>
            /// Includes STANDARD_RIGHTS_REQUIRED, in addition to all access 
            /// rights in this table.
            /// </summary>
            SC_MANAGER_ALL_ACCESS =
                ACCESS_MASK.STANDARD_RIGHTS_REQUIRED | SC_MANAGER_CONNECT | SC_MANAGER_CREATE_SERVICE | SC_MANAGER_ENUMERATE_SERVICE | SC_MANAGER_LOCK | SC_MANAGER_QUERY_LOCK_STATUS
                | SC_MANAGER_MODIFY_BOOT_CONFIG,

            GENERIC_READ = ACCESS_MASK.STANDARD_RIGHTS_READ | SC_MANAGER_ENUMERATE_SERVICE | SC_MANAGER_QUERY_LOCK_STATUS,

            GENERIC_WRITE = ACCESS_MASK.STANDARD_RIGHTS_WRITE | SC_MANAGER_CREATE_SERVICE | SC_MANAGER_MODIFY_BOOT_CONFIG,

            GENERIC_EXECUTE = ACCESS_MASK.STANDARD_RIGHTS_EXECUTE | SC_MANAGER_CONNECT | SC_MANAGER_LOCK,

            GENERIC_ALL = SC_MANAGER_ALL_ACCESS,
        }

        [Flags]
        public enum SERVICE_CONTROL : uint
        {
            STOP = 0x00000001,

            PAUSE = 0x00000002,

            CONTINUE = 0x00000003,

            INTERROGATE = 0x00000004,

            SHUTDOWN = 0x00000005,

            PARAMCHANGE = 0x00000006,

            NETBINDADD = 0x00000007,

            NETBINDREMOVE = 0x00000008,

            NETBINDENABLE = 0x00000009,

            NETBINDDISABLE = 0x0000000A,

            DEVICEEVENT = 0x0000000B,

            HARDWAREPROFILECHANGE = 0x0000000C,

            POWEREVENT = 0x0000000D,

            SESSIONCHANGE = 0x0000000E
        }

        [Flags]
        public enum SERVICE_ACCESS : uint
        {
            STANDARD_RIGHTS_REQUIRED = 0xF0000,

            SERVICE_QUERY_CONFIG = 0x00001,

            SERVICE_CHANGE_CONFIG = 0x00002,

            SERVICE_QUERY_STATUS = 0x00004,

            SERVICE_ENUMERATE_DEPENDENTS = 0x00008,

            SERVICE_START = 0x00010,

            SERVICE_STOP = 0x00020,

            SERVICE_PAUSE_CONTINUE = 0x00040,

            SERVICE_INTERROGATE = 0x00080,

            SERVICE_USER_DEFINED_CONTROL = 0x00100,

            SERVICE_ALL_ACCESS =
                (STANDARD_RIGHTS_REQUIRED | SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG | SERVICE_QUERY_STATUS | SERVICE_ENUMERATE_DEPENDENTS | SERVICE_START | SERVICE_STOP | SERVICE_PAUSE_CONTINUE
                 | SERVICE_INTERROGATE | SERVICE_USER_DEFINED_CONTROL)
        }

        [Flags]
        public enum SERVICE_TYPES : int
        {
            SERVICE_KERNEL_DRIVER = 0x00000001,

            SERVICE_FILE_SYSTEM_DRIVER = 0x00000002,

            SERVICE_WIN32_OWN_PROCESS = 0x00000010,

            SERVICE_WIN32_SHARE_PROCESS = 0x00000020,

            SERVICE_INTERACTIVE_PROCESS = 0x00000100
        }

        public enum SERVICE_ERROR_CONTROL : int
        {
            /// <summary>
            /// The startup program logs the error in the event log, if possible. If the last-known-good configuration is being started, the startup operation fails. Otherwise, the system is restarted with the last-known good configuration.
            /// </summary>
            SERVICE_ERROR_CRITICAL = 0x00000003,

            /// <summary>
            /// The startup program ignores the error and continues the startup operation.
            /// </summary>
            SERVICE_ERROR_IGNORE = 0x00000000,

            /// <summary>
            /// The startup program logs the error in the event log but continues the startup operation.
            /// </summary>
            SERVICE_ERROR_NORMAL = 0x00000001,

            /// <summary>
            /// The startup program logs the error in the event log. If the last-known-good configuration is being started, the startup operation continues. Otherwise, the system is restarted with the last-known-good configuration.
            /// </summary>
            SERVICE_ERROR_SEVERE = 0x00000002,
        }

        public enum SERVICE_START_TYPES : int
        {
            /// <summary>
            /// A service started automatically by the service control manager during system startup. For more information, see Automatically Starting Services.
            /// </summary>
            SERVICE_AUTO_START = 0x00000002,

            /// <summary>
            /// A device driver started by the system loader. This value is valid only for driver services.
            /// </summary>
            SERVICE_BOOT_START = 0x00000000,

            /// <summary>
            /// A service started by the service control manager when a process calls the StartService function. For more information, see Starting Services on Demand.
            /// </summary>
            SERVICE_DEMAND_START = 0x00000003,

            /// <summary>
            /// A service that cannot be started. Attempts to start the service result in the error code ERROR_SERVICE_DISABLED.
            /// </summary>
            SERVICE_DISABLED = 0x00000004,

            /// <summary>
            /// A device driver started by the IoInitSystem function. This value is valid only for driver services.
            /// </summary>
            SERVICE_SYSTEM_START = 0x00000001

        }

        public enum SERVICE_STATE : uint
        {
            SERVICE_STOPPED = 0x00000001,

            SERVICE_START_PENDING = 0x00000002,

            SERVICE_STOP_PENDING = 0x00000003,

            SERVICE_RUNNING = 0x00000004,

            SERVICE_CONTINUE_PENDING = 0x00000005,

            SERVICE_PAUSE_PENDING = 0x00000006,

            SERVICE_PAUSED = 0x00000007
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SERVICE_STATUS
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(SERVICE_STATUS));

            public SERVICE_TYPES dwServiceType;

            public SERVICE_STATE dwCurrentState;

            public uint dwControlsAccepted;

            public uint dwWin32ExitCode;

            public uint dwServiceSpecificExitCode;

            public uint dwCheckPoint;

            public uint dwWaitHint;
        }

    }
}
