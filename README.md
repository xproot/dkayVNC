# ![image](https://github.com/xproot/dkayVNC/assets/49620652/2b9b7c09-d046-4ff1-8e46-f482e4f31eff) dkayVNC
Discord bot which allows you to control VNC servers

**⚠ WARNUNG: this is very very very very very very beta, it kinda sucks**

![image](https://github.com/xproot/dkayVNC/assets/49620652/357bc7d4-434c-4aa2-b089-f14aec5ef7fd)
![ss](https://github.com/xproot/dkayVNC/assets/49620652/ce31a495-d533-4d38-89b0-2c0d6ecf0c07)

## Current Goals
- [x] Mouse movement
- [ ] Better mouse movement
- [ ] SendKeys-like keyboard command
- [X] Permission system
- [ ] Fix up permission system
- [ ] Migrate to [MarcusW.VncClient](https://github.com/MarcusWichelmann/MarcusW.VncClient)
- [ ] Crossplatform (so I can't use AnimatedGif and System.Drawing ;-;)

## Configuration 
dkayVNC will make a blank configuration on the first startup, that will look something like this
```json
{
  "Prefixes": [
    "d!",
    "dkay!"
  ],
  "Token": "BOT_TOKEN_HERE",
  "OwnerId": [
    366188463198044162,
    831176009507536937
  ],
  "EnforceDefaultChannel": true,
  "DefaultChannelId": 0,
  "DefaultVNCServerHostname": "",
  "DefaultVNCServerPort": 5900,
  "DefaultVNCServerPassword": "",
  "GraphicalError": 1
}
```
*P.S. this might change as it updates*

### Prefixes
I use an array as I like to have multiple prefixes, and you can remove these entirely as you can mention the bot alternatively, adding new prefixes is basically adding another element to the array
```json
"Prefixes": [
    "d!",
    "dkay!",
    "vnc!"
  ],
```
If you wish to only have one prefix then remove every other one
```json
"Prefixes": [
    "d!",
],
```

### OwnerId
Owners bypass the permission system, so they can control the bot even if they are blacklisted, or not whitelisted, or running commands outside the enforced channel. As an avid alt user myself, I made these arrays, it works the same way as prefixes, the more the merrier!

### EnforceDefaultChannel
This will make the bot FIXED to the default channel, when this is ticked on nobody except the owners can use VNC commands outside the default channel, **though the owners can reconnect on another channel and it'll work mostly the same but with that new channel bound.**

### DefaultChannelId
This is the default channel the bot will bind to. If EnforceDefaultChannel or DefaultVNCServer aren't set, this is as good as useless.

### DefaultVNCServer*
If these are all correctly set along with DefaultChannelId, the bot will attempt a connection to the server on startup.

### GraphicalError
To add goofyness to the bot, this is how the default error handler works, set it to 0 to disable it.

![image](https://github.com/xproot/dkayVNC/assets/49620652/dfaafd22-bdc4-4be8-a647-f977b04fc9ae)

## Commands
TODO
### keycodes
It uses keycodes separated by spaces, a keycode converter has been included in dkayVNC. Once the sendkeys-like keyboard command is done, this is not going to be needed.

![image](https://github.com/xproot/dkayVNC/assets/49620652/cee13b3b-5262-417b-a70b-2d11bbb3e657)

## FAQ 
- **Q:** why?
- **A:** A server I deployed this on spent the entire night trying to install Windows 7 through this, basically, it's fun.
