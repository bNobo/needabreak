using Microsoft.Win32;
using System;

namespace NeedABreak.Utils
{
    public static class RegistryTool
    {
        /// <summary>
        /// Check that key is not null then close and dispose it
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public static void ActOnRegistryKey(RegistryKey key, Action<RegistryKey> action)
        {
            if (key != null)
            {
                action(key);
                key.Close();
                key.Dispose();
            }
        }

        public static RegistryKey GetRunRegistryKey(bool writable)
        {
            // Not possible to put \ in string because it breaks Fody during build :/ 
            // Using Replace to workaround the problem
            return Registry.CurrentUser
                .OpenSubKey("Software/Microsoft/Windows/CurrentVersion/Run"
                .Replace('/', '\\'), writable);
        }

    }
}
