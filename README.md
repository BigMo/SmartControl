# SmartControl
SmartControl allows for easy automation of `screen`-instances and is meant to make setting up screen for (game-)servers easy. Simply add a configuration for each server you'd like to automate and you're good to go!

**Note:** This started as a personal project for a specific need which is why it is lacking some sanity-checks and doesn't come with code-documentation *yet*. 

## Features
* ServerConfiguration lets you specify both basic information about servers as well as commands to send to them on startup or shutdown
* Simple usage!
* Detailed logging: In case you run into any problems, check the logs!
* Detects whether a server is already running or not
* Can easily be used with and invoked by shell-scripts

## Usage
`mono SmartControl.exe METHOD NAME` or `mono SmartControl.exe METHOD all` (not implemented yet), where METHOD is either `Start`, `Stop` or `Restart`.
Also, there are some basic commands:
* `-h` or `--help`: Print usage
* `-v` or `--version`: Print current version
* `-l` or `--list`: Print current configurations

Adding configurations is done by adding new entries to the `serverconfigurations`-Array in the config-file, as shown [here](#example).

## Requirements
* mono, runtime-version 4.0

## Example
Setting up a minecraft-server is rather simple with this tool. Since minecraft-servers often come with a shell-script, we can simply invoke it:
* Copy `SmartControl.exe`, `SmartControl.exe.config`, `log4net.dll`, `Newtonsoft.Json.dll` to a folder of your choice
* Create `SmartControlConfigs.json`, fill in:
```
{
  "serverconfigurations": [
    {
      "name": "Minecraft",
      "restartDelay": 15,
      "allowMultipleInstances": false,
      "filepath": "/bin/sh",
      "workingdirectory": "/var/gameservers/minecraft/",
      "arguments": "ServerStart.sh",
      "sendOnStart": [],
      "sendOnStop": [
        "^M",
        "save-all^M",
        "stop^M"
      ]
    }
  ]
}
```
  * `Name`: This name will be used for both automation and screen-names.
  * `RestartDelay`: Seconds between stopping and starting the server back up again.
  * `AllowMultipleInstances`: Whether or not to allow for spawning multiple instances.
    * We don't want to run the gameserver twice in this case.
  * `FilePath`: The path of the program to execute.
    * We'll let sh interpret the given script-file.
  * `WorkingDirectory`: The directory to execute the program in.
    * Make sure this is the gameserver's root-directory.
  * `Arguments`: Arguments you want to pass to the program.
    * Since we want to have sh interpret the script, pass the script-file.
  * `SendOnStop`: An array of messages that are sent to the screen-instance after starting it up.
    * We don't need to supply anything upon startup.
  * `SendOnStop`: An array of messages that are sent to the screen-instance to shut it down.
    * In order to stop the server, make sure that the current input-buffer is cleared (`^M`/`Enter`), save the gamestate (`save-all^M`) and halt the server (`stop^M`).
* Run `mono SmartControl.exe Start Minecraft` to start up the gameserver, `mono SmartControl.exe Stop Minecraft` will stop it.
* You're good to go! :sparkles:
