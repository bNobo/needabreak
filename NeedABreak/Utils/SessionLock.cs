using System.Runtime.InteropServices;
using System.Windows;

namespace NeedABreak.Utils
{
    class SessionLock
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool LockWorkStation();

        public static void LockSession()
        {
            if (LockWorkStation() == false)
            {
                MessageBox.Show("Session Could not be locked!");
            }
        }
    }
}
