# DragonBot

Yet another Discord bot

## Setup

You'll need the .NET Core SDK 2.2 installed. Also, you'll need to either host `DragonStallion.Common` yourself or configure your nuget to pull from my package feed at https://www.myget.org/F/dragon-stallion/api/v3/index.json

For now, start it with the environmental variable `DISCORD_TOKEN` set to your bot token. I'll probably use this as an opportunity to learn how to set up a .NET Core app as a Linux daemon, so token loading may change depending on whatever's most convenient for that use case.
