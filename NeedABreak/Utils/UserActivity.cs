using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeedABreak.Utils
{
	public class UserActivity
	{
		private TimeSpan _idleTime;
				
		/// <summary>
		/// UserActivity constructor
		/// </summary>
		/// <param name="idleTime">time after which the user will be considered idle.</param>
		public UserActivity(TimeSpan idleTime)
		{
			if (idleTime == TimeSpan.Zero)
			{
				idleTime = TimeSpan.FromSeconds(7);
			}

			_idleTime = idleTime;
		}

		public UserActivity() : this(TimeSpan.Zero) { }

		[StructLayout(LayoutKind.Sequential)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "<Pending>")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		public struct LASTINPUTINFO
		{
			public uint cbSize;
			public uint dwTime;
		}

		[DllImport("user32.dll")]
		static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

		public static TimeSpan GetInactiveTime()
		{
			LASTINPUTINFO info = new LASTINPUTINFO();
			info.cbSize = (uint)Marshal.SizeOf(info);

			if (GetLastInputInfo(ref info))
			{
				return TimeSpan.FromMilliseconds(Environment.TickCount - info.dwTime);
			}
			else
			{
				return TimeSpan.Zero;
			}
		}

		public async Task WaitForUserToBeIdleAsync()
		{
			TimeSpan inactiveTime = GetInactiveTime();

			while (inactiveTime < _idleTime
				|| AMouseButtonIsPressed())
			{
				await Task.Delay(1000);
				inactiveTime = GetInactiveTime();
			}
		}

		public static bool AMouseButtonIsPressed()
		{
#if DEBUG
			System.Diagnostics.Debug.WriteLine("LeftButton State = {0}", GetAsyncKeyState(VK_LBUTTON));
			System.Diagnostics.Debug.WriteLine("MiddleButton State = {0}", GetAsyncKeyState(VK_MBUTTON));
			System.Diagnostics.Debug.WriteLine("RightButton State = {0}", GetAsyncKeyState(VK_RBUTTON)); 
#endif

			return GetAsyncKeyState(VK_LBUTTON) > 0 || GetAsyncKeyState(VK_MBUTTON) > 0 || GetAsyncKeyState(VK_RBUTTON) > 0;
		}

		[DllImport("user32.dll")]		
		static extern ushort GetAsyncKeyState(ushort virtualKeyCode);

		const int VK_LBUTTON = 0x01;
		const int VK_MBUTTON = 0x04;
		const int VK_RBUTTON = 0x02;
	}
}
