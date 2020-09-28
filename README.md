![MTGAHelper](http://www.mtgahelper.com/images/hero1-bg.jpg)

[MTGAHelper](http://www.mtgahelper.com) is a web application used along with the game Magic: The Gathering Arena.

### MTGAHelper-Windows-Client

This project contains the Installer and source code for the local program that can be installed on your computer known as a *Tracker*  and is used to communicate with the MTGAHelper server.

You can then use the **[MTGAHelper website](http://www.mtgahelper.com)**  at any time to see your cards collection statistics and a variety of information tracked for you while you enjoy playing Magic: The Gathering Arena.

See [this post](https://www.patreon.com/posts/how-to-make-best-29075781) for an overview on how to best use MTGAHelper.

> Please note that the Full Tracker runs only on Windows for now. However, you can upload your log file on any OS with this [portable console program](https://github.com/ibiza240/MTGAHelper-Windows-Client/raw/master/MTGAHelper.ConsoleSync.exe.zip) or manually on the website at any time.
See [this explanation page](Console-on-macos.md) for more info on how to run the console program on MacOS.

### Features

- Automatically synchronizes your log files data with the MTGAHelper servers
- In-match tracker to see your draw probabilities, sideboard, opponent cards played, etc.
- Draft helper when picking your cards during a Draft or with Sealed boosters:
   - Displays Limited ratings from your prefered source (your choice between MTG Arena Zone, Deathsie, Infinite Mythic Edition and others) to help you win more
   - Gives you Raredrafting information based on your collection to help you determine if it's worth it to raredraft
      - Number of copies currently owned
      - How many decks and sideboards the card is played in
      - A calculated score that shows its priority for Constructed

##### Check the [Patch notes](https://github.com/ibiza240/MTGAHelper-Windows-Client/blob/master/PatchNotes.md) to know about the latest changes.

-----

### Installation

Simply [download the Tracker Installer](https://mtgahelper.com/download/MTGAHelperTracker.msi) and after the very simple setup, the tracker will be ready to run from your Desktop where a shortcut will be created.

### Configuration

To use the tracker, you will need to have an MTGAHelper account. You can authenticate with Google or Facebook directly, or you may also use a local account but you will first have to sign-up for one on the website.

### Activate the detailed MTGA log <span style="color:red;">(important!)</span>

To be able to use the tracker, you need to do the following in the MTGA game client:

1. Press `Esc` to get the Options popup and click on **View account**
<img src="https://i.imgur.com/NpLkJzy.png" width="420" alt="MTGA Options">

2. Check the **Detailed Logs (Plugin Support)** option
<img src="https://i.imgur.com/pWJVc7J.png" width="420" alt="MTGA Profile">

3. Restart the game.

### Usage

You will notice activity in the status bar any time the tracker is communicating with the servers. For example the status bar will blink and indicate `Uploading log file to server...` while it is uploading your data, which can take some time. You are NOT blocked by this activity and can continue playing the game normally.

Once your data is uploaded successfully, you can simply refresh the MTGAHelper website to see it updated.

-----

## Troubleshooting

### Nothing happens when I run the MTGAHelper Tracker

1- The server might be unresponsive. Try to go on https://mtgahelper.com and if the site cannot load, the tracker won't be able to start. You can report this downtime on [Discord](https://discord.gg/GTd3RMd).

2- Also, in the `%AppData%/MTGAHelper` directory there might be some log files (e.g.: `log-202006.txt`). You can open these text files and see if there's any helpful information.


### I am connected but the tracker and website are showing no data / 0 cards in collection

Please be sure that you [activated the detailed logging in-game](#activate-the-detailed-mtga-log-important), otherwise there will be no data to parse in your log file.

### Questions or Feedback

If you are stuck at any moment, please reach us directly through our [Discord server](https://discord.gg/GTd3RMd) for the fastest response time or by using the [contact form](https://www.mtgahelper.com/contact) on the MTGAHelper website so we can assist you.

-----

###### All art is property of their respective artists and/or Wizards of the Coast Inc. This website is not produced, endorsed, supported, or affiliated with Wizards of the Coast. MTGAHelper is unofficial Fan Content permitted under the Fan Content Policy.
