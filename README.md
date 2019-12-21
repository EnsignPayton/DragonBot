# DragonBot

Yet another Discord bot

## Requirements

* .NET Core SDK 3.1
* A discord bot token, provided through either `Config.json` or the environmental variable `DISCORD_TOKEN`.
* Windows. NAudio doesn't appear to be cross-platform, and we have some Windows binaries committed for voice support. Full cross-platform support is not impossible by any means, but this project isn't set up for it at the moment and probably won't be while Windows 10 is my daily driver.

## What Does It Do?

At this point, it's basically a soundboard. You place media files in the directory it's configured to scan, and it will play them in a voice channel.
