# NEED A BREAK!

## Presentation
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FbNobo%2Fneedabreak.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FbNobo%2Fneedabreak?ref=badge_shield)

NEED A BREAK! is an open source application intended to help you take care of your health while you work on a computer.
It will encourage you to regularly have a break in order to avoid health issues like musculoskeletal disorders, headaches or eye strain.

I have made it for my personal usage and personaly use it every day at work and it helps me to not sit for too long. So I decided to share it because maybe it could be useful to someone else.

You will be notified a minute before lockdown:

<img src="https://github.com/bNobo/needabreak/blob/root/Captures/imminent_lockdown.jpg" alt="imminent lockdown" width="300" />

When it is time to have a break a countdown will appear. The screen will be locked when it reaches 0:

<img src="https://github.com/bNobo/needabreak/blob/root/Captures/mainwindow.gif" alt="countdown" width="600" />

You can verify that the application is running thanks to the task bar icon: 

<img src="https://github.com/bNobo/needabreak/blob/root/Captures/taskbar.jpg" alt="task bar icon" width="400" />

You can display the application menu by clicking on the coffee cup:

<img src="https://github.com/bNobo/needabreak/blob/root/Captures/menu.jpg" alt="menu" width="300" />

The settings window let you choose how long the application will wait before lockdown:

<img src="https://github.com/bNobo/needabreak/blob/root/Captures/settings.jpg" alt="settings" width="500" />

You can find this documentation on my github page: https://bnobo.github.io/needabreak/

## Installation

If you just want to install and enjoy the latest version of the application, you can download the setup from [here](https://bnobo.github.io/needabreak/NeedABreak/publish/setup.exe).

> Note that you will have a big red warning from the UAC during installation because the installer is not digitally signed.

## Get started

The project is a WPF application targeting .Net Framework 4.8. All you need is a copy of Visual Studio Community in order to build it.
Once started, the application creates a coffee cup icon in the task bar to manifest its presence and permits user to interact with it. 
Just click on the coffee cup to open the application menu.

## Contribute

I'm sure this application could be improved in many ways and I would be happy to receive some help in doing so. If you want to contribute to this project, please go to https://github.com/bNobo/needabreak and read [contributing.md](https://github.com/bNobo/needabreak/blob/root/contributing.md) file.

Every kind of contribution is welcome, it includes, but is not limited to:
* Add new functionalities
* Improve translations
* Improve design
* Fix bugs
* Test to find issues
* Fix typos

## Points of interest in code

### P/Invoke

Use of P/Invoke to lock workstation:

```csharp
[DllImport("user32.dll", SetLastError = true)]
[return: MarshalAs(UnmanagedType.Bool)]
static extern bool LockWorkStation();
```

Use of P/Invoke to check current user notification state in order to automatically suspend the application if a notification would not be appropriate:

```csharp
[DllImport("shell32.dll")]
static extern int SHQueryUserNotificationStte(outUserNotificationStateuserNotificationState);
```

Use of P/Invoke to ensure user is idle before poping-up the countdown window:

```csharp
[DllImport("user32.dll")]
static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

[DllImport("user32.dll")]		
static extern ushort GetAsyncKeyState(ushort virtualKeyCode);
```

### System events

Use of system events to detect session switch:

```csharp
Microsoft.Win32.SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
```

### Mutex

Use of Mutex class to prevent application from beeing launched twice:

```csharp
mutex = new System.Threading.Mutex(false, "Local\\NeedABreakInstance");
if (!mutex.WaitOne(0, false))
{
    Logger.Info("Application already running");
    Current.Shutdown();
    return;
}
```

### Translations

Use of [Multilingual App Toolkit](https://marketplace.visualstudio.com/items?itemName=MultilingualAppToolkit.MultilingualAppToolkit-18308) extension to handle translations. RESX files are automatically generated from translations made in XLF files.

Use of custom markup extension to handle translations in XAML files like this:

```XAML
<TextBlock Margin="0 210 0 0"
           Text="{utils:TextResource warranty}" />
```

### Adorner

Use of Adorner class to surround selected tile on the settings window.



## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FbNobo%2Fneedabreak.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FbNobo%2Fneedabreak?ref=badge_large)
