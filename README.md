![MTGAHelper](http://www.mtgahelper.com/images/hero1-bg.jpg)

[MTGAHelper](http://www.mtgahelper.com) is an application used along with the game Magic: The Gathering Arena.

### MTGAHelper-Windows-Client

This project contains source code for the local program that can be installed on your computer known as a *Tracker*  and is used to communicate with the MTGAHelper servers.

You can then go to the [MTGAHelper website](http://www.mtgahelper.com)  at any time to see your cards collection statistics and a variety of information tracked for you while you enjoy playing Magic: The Gathering Arena.

Please note that this tracker runs only on Windows for now.

### Features

- Automatically synchronizes your log files data with the MTGAHelper servers

-----

### Installation

##### Microsoft .NET Core 3.0 framework
The application uses the very recent framework known as Microsoft .NET Core 3.0.

You will require to install:
- [This version if you have a 64-bit Operating System (x64-based processor)](https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-3.0.100-preview7-windows-x64-installer)
- [This version if you have a 32-bit Operating System (x86-based processor)](https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-3.0.100-preview7-windows-x86-installer)

##### MTGAHelper Tracker Installer

Once the framework installed, [download the Tracker Installer](https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelperTracker.msi) and after the very simple setup, the tracker will be ready to run from your Desktop where a shortcut will be created.

### Configuration

The first time you run the tracker, you will have to provide your MTGAHelper user id to be able to synchronize your data with our servers.

You can find your user id on your [MTGAHelper website profile page](https://mtgahelper.com/profile).

### Usage

You will notice activity in the status bar any time the tracker is communicating with the servers. For example the status bar will blink and indicate `Uploading log file to server...` while it is uploading your data, which can take some time. You are NOT blocked by this activity and can continue playing the game normally.

Once your data is uploaded successfully, you can simply refresh the MTGAHelper website to see it updated.

-----

### Troubleshooting

If you start the tracker and nothing happens, it's probably because you don't have the .NET Core 3.0 framework installed. Please try installing it before re-launching the tracker, to see if it fixes the problem.

If you are stuck at any moment, you can always reach us directly through our [Discord server](https://discord.gg/GTd3RMd) for the fastest response time or by using the [contact form](https://www.mtgahelper.com/contact) on the MTGAHelper website.

-----

###### All art is property of their respective artists and/or Wizards of the Coast Inc. This website is not produced, endorsed, supported, or affiliated with Wizards of the Coast. MTGAHelper is unofficial Fan Content permitted under the Fan Content Policy.
