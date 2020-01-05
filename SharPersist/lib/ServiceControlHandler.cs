using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;


namespace SharPersist.lib
{
    //Reference https://stackoverflow.com/questions/23481394/programmatically-install-windows-service-on-remote-machine
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    public class ServiceControlHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private ServiceControlHandle()
            : base(true)
        {
        }
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        override protected bool ReleaseHandle()
        {
            return NativeMethods.CloseServiceHandle(this.handle);
        }
    }
}
