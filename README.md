# RF.AutoMatchRecorder
 A Rhythm Festival mod that record any match communicating with OBS through OBS web socket
 
 <a href="taikomodmanager:insertGithubLinkhereAndReplaceWithUrlShortener"> <img src="Resources/InstallButton.png" alt="One-click Install using the Taiko Mod Manager" width="256"/> </a>
 
# Requirements
 Visual Studio 2022 or newer\
 Taiko no Tatsujin: Rhythm Festival
 
 
# How to use
1. Install OBS
2. Setup "Web socket" option in OBS
3. Copy OBS Web Socket password and paste it into mod configuration file
4. Open the game and play

# Build
 Install [BepInEx 6.0.0-pre.2](https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.2) into your Rhythm Festival directory and launch the game.\
 This will generate all the dummy dlls in the interop folder that will be used as references.\
 Make sure you install the Unity.IL2CPP-win-x64 version.\
 Newer versions of BepInEx could have breaking API changes until the first stable v6 release, so those are not recommended at this time.
 
 Attempt to build the project, or copy the .csproj.user file from the Resources file to the same directory as the .csproj file.\
 Edit the .csproj.user file and place your Rhythm Festival file location in the "GameDir" variable.
