### Using ConsoleSync on MacOS
To be able to use the ConsoleSync program on MacOS you will need to install the .Net Core Runtime 3.1, available [here](https://dotnet.microsoft.com/download/dotnet-core/3.1).
Just the runtime (third from the top, on the right) will be enough for our purposes.
After installing .Net Core 3.1, you can download the ConsoleSync program [here](https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelper.ConsoleSync.exe.zip) and run the program by navigating to the extracted zip folder in a terminal and typing:

```dotnet MTGAHelper.ConsoleSync.dll MTGAHelperuserID "/Users/username/Library/Logs/Wizards of the Coast/MTGA/Player.log"```

replacing above

- `MTGAHelperuserID` with your User ID that you can find on the profile webpage at https://mtgahelper.com/profile
- and `username` with your (MacOS) username
