# ZP2KXPC (A ZP2KX PC Port)
A port of the Xbox Live Indie Game ZP2KX by Ska Studios to Monogame (DesktopGL) with LAN play and Steamworks Multiplayer Integration.
As this is not listed on Steam it uses the public Space Wars test servers.

## Who Made This

ZP2KX was made completely by Ska Studios, this is a port I made with AI assistance from Claude. Due to the end of Xbox Live Indie Games I have gotten permission from Ska Studios to share this recompilation. It will be taken down upon their request, or whenever an official PC port comes around.

 ## Requirements
 
- Windows 10/11 (64-bit)
- [.NET 9 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/9.0): If the game won't launch with a ".NET runtime not found" error, this is almost always why.
- [Steam](https://store.steampowered.com/about/): Must be installed and running (signed in) but it's only needed for online multiplayer. Practice mode and Lan work without it.

## Downloading and Running

1. Get the latest build from the [Releases page](../../releases) (newest version on top).
2. Unzip it anywhere on your PC.
3. Run `ZP2KXPC.exe`.

No installer or admin rights are needed as it's a portable folder. To update, download the new release and unzip over (or replace) the old folder.

## Online Requirements

Once again, Steam needs to be running and signed in. Other than that, you need Space War installed for it's test servers (it is less than 2 MB).

To Install Space War on Steam

1. Press Windows Key + R to open the Microsoft Run menu
2. Paste (without quotes) "steam://install/480"
3. Run it
4. Enjoy!

## Playing online with friends

- **Host a public game:** Server Setup → set "Status: Public" → Start. It'll show up for anyone browsing for games.
- **Host a private game:** same as above, but set "Status: Private" - the game won't show up in the public browser, so the only way in is an invite.
- **Invite a friend:** from the pause menu (in a game you're hosting or already in), choose "Invite Friends" to open Steam's normal invite overlay to pick who to send it to.
- **Join a public game:** from the main menu's server browser, pick a listed game and join.

## Where saves are stored

Your save (settings + profile/progression) lives at:

```
<game folder>\Saves\zp2k5\settings.sav
```

Back up that file (or the whole `Saves` folder) before updating if you want to be safe. Update should never effect this.

## Building from source

- Requires Visual Studio 2022 (or the .NET 9 SDK + your editor of choice) and the MonoGame framework/content pipeline tooling.
- Open `ZP2KXPC.slnx`, restore/build `ZP2KXPC.csproj`.
- `steam_api64.dll` is included in the repo and copies automatically on build/publish.
- To produce a distributable build, use `dotnet publish` (or Visual Studio's Publish flow) - the output folder is what gets zipped up for a Release.

## Status

Actively being playtested - multiplayer (LAN and Steam) is working but still being shaken out. If you hit a bug, please open an issue with what happened and, if possible, a copy of the console/log output.

## Note On AI Usage

This port was made with assistance from and uses some written code by Claude. AI should never be used in place of creativity, but for the sake of porting it can be a valuable tool.

Thank you.
