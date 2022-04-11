# Far Cry 3: Silent Menu patcher

Patcher that modifies `FC3.dll`**/**`FC3_d3d11.dll` to remove the awful background noise played within pause screens (a very loud electronic hum).

This solution was provided by **koorashi** in [this reddit post](https://old.reddit.com/r/farcry/comments/15q4en/goddamn_that_bassy_repetitive_noise_on_the_pause/c7ozuna/) from December 2012 <sup>([web archive](http://web.archive.org/web/20220411160830/https://old.reddit.com/r/farcry/comments/15q4en/goddamn_that_bassy_repetitive_noise_on_the_pause/))</sup>.


## Instructions

Drag `FC3.dll` or `FC3_d3d11.dll` onto this program to apply the patch. A backup copy will be created with `_orig` added to the end of the file name.

Which DLL to patch depends on your video settings in-game:
* `FC3.dll` if you are using **DirectX9**
* `FC3_d3d11.dll` if you are using **DirectX11** (default)
* *(If you're unsure, then just patch both DLLs)*

### Location

The DLL file can be found in: `<FC3 INSTALL FOLDER>/bin/`.

For example: `C:/Program Files (x86)/Steam/steamapps/common/Far Cry 3/bin/FC3.dll`

***

## Command line usage

```
usage: TriggersTools.FarCry3.SilentMenuPatcher.exe [-h|--help] [-T|--test] <FC3DLL>

arguments:
  FC3DLL     FC3.dll or FC3_d3d11.dll file to patch.

optional arguments:
  -h/--help  Show this help message.
  -T/--test  Test program execution without writing to the DLL file.