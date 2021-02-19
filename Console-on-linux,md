### Using ConsoleSync on Ubuntu / Linux
To be able to use the ConsoleSync program on Ubuntu you will need to install the .Net Core Runtime 3.1, available [here](https://dotnet.microsoft.com/download/dotnet-core/3.1). Just the runtime (third from the top, on the right) will be enough for our purposes. You can also use snap for that: `sudo snap install dotnet-sdk --classic --channel=3.1/stable`

After installing .Net Core 3.1, you can download the ConsoleSync program [here](https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelper.ConsoleSync.exe.zip) and navigate to the extracted zip folder.

You will need to put the "Player.log" file inside the "MTGAHelper.ConsoleSync.exe" folder as the argument only works relative to path. So this is the way to call the program:
```dotnet MTGAHelper.ConsoleSync.dll "MtgaHelperUserId" "Player.log"```
(replacing `MTGAHelperuserID` with your id

A trick is to simply make a symlink though, which works. So symlinking "Player.log" inside the "MTGAHelper.ConsoleSync.exe" folder to the actual location of the "Player.log" in the wine location.
