### How to manually upload your log file after version 1.8.1 (2021-09-20)
Since WotC removed information about collection and inventory from the log file, third party trackers had to find other ways to get the data.
#### Impacts
- Full tracking is now Windows only because of technical limitations. Still working on all platforms: Matches tracking, Deck stats, Mastery pass calculator, Draft helper;
- A tiny program is required to be run while playing MTGA (it's automatically run behind the full GUI);
  - When not using the full GUI tracker, the `getFrontWindow.exe` console program needs to be running at the same time as the MTGA process. It will allow to output additional information in the log file;
#### Instructions from now on
First, be sure to download the latest version **[here](https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelper.ConsoleSync.exe.zip)**
1. Run the `getFrontWindow.exe` console program from the console tracker here.
2. Start the MTGA game and play normally. The program will output additional data in the log file.
3. After exiting the game, close the console program.
4. Upload your file manually like before with the method of your choice (console program or upload on website)

So yeah...it's not ideal, but for now the console program is required to write data in the player log file about collection and inventory. Without running it, the normal log files won't contain that information and these features won't work: Collection and Inventory tracking, Booster/Cards to craft advisors, Draft vs boosters tool.
