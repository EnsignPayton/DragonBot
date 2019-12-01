#!/usr/bin/env bash
# Custom cross-platform bash Cake bootstrapper

# Check for Windows x64 (Git Bash)
if [ "$(expr substr $(uname -s) 1 10)" == "MINGW64_NT" ]; then
	powershell -NoProfile -File build.ps1 "$@"
	exit 0
fi

# For now, the only other supported target is Linux
if [ "$(expr substr $(uname -s) 1 5)" != "Linux" ]; then
	echo "The target platform is not supported."
	exit 1
fi

# Manual Cake.CoreCLR bootstrapper for Linux
TOOLS_DIR=tools
CAKE_CORECLR=$TOOLS_DIR/Cake.CoreCLR
CAKE_DLL=$CAKE_CORECLR/Cake.dll

if [ ! -d "$TOOLS_DIR" ]; then
	mkdir -p "$TOOLS_DIR"
fi

if [ ! -d "$CAKE_CORECLR" ]; then
	CAKE_CORECLR_URL=https://www.nuget.org/api/v2/package/Cake.CoreCLR/
	echo "Downloading Cake.CoreCLR from $CAKE_CORECLR_URL ..."
	curl -Lsfo Cake.CoreCLR.zip $CAKE_CORECLR_URL && unzip -q Cake.CoreCLR.zip -d "$CAKE_CORECLR" && rm -f Cake.CoreCLR.zip
	if [ $? -ne 0 ]; then
		echo "An error occurred while fetching Cake.CoreCLR from nuget."
		exit 1
	fi
fi

if [ ! -f "$CAKE_DLL" ]; then
	echo "Cound not find $CAKE_DLL"
	exit 1
fi

exec dotnet "$CAKE_DLL" "$@"
