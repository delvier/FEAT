# FEAT

![UI](example_pictures/UI_6.png)

Fire Emblem Archive Tool (A tool to automatically extract data from 3DS Fire Emblem archives)

### Usage:
Drag and Drop files to decompress of extract them<br />
Drag and Drop a **file** while holding CTRL to compress it to lz13<br />
Drag and Drop a **folder** while holding CTRL to turn it into an ARC<br />
**Auto Extract** option will automatically extract files, when possible, after decompression.<br />
**Build Textures** enables BCH texture importing and CTPK rebuilding.<br />
Drag and Drop and folder with a bch in the same directory to import replace the files in the bch with the matching ones in the folder.<br />
Drag and Drop an Extract CTPK folder to rebuild the CTPK.<br />
**Arc Padding** adds the padding for arc files used by Fates.<br />
**Arc File Alignment** adjust the byte alignment for files inside the arc. Fates uses 128 while Awakening usually uses 0<br />

### Credit to:<br /> 
[ctpktool](https://github.com/polaris-/ctpktool) for ctpk packing and unpacking,<br />
[DSDecmp](https://github.com/einstein95/dsdecmp) for LZ decompression,<br />
[SPICA](https://github.com/gdkchan/SPICA) for bch/gfx Parsing,<br />
[FEIF_Arc](https://github.com/GovanifY/FEIF_ARC) for Arc packing and unpacking<br />
[SciresM](https://github.com/SciresM) for the original FEAT code

## Velouria Forked FEAT v2.2
- UI Overhual
- Improved Arc building code
- Fixed ctpk building bug
- Fixed .bcmdl files not extracting

## Veloruia Forked FEAT v2.1
- Fixed Arc rebuilding code

## Velouria Forked FEAT v2.0
- Added Arc rebuilding
- Expanded list of options
- Packed .dll into exe

## Velouria Forked FEAT v1.9
- Added option to disable automatic texture extracting
- Added support for gfx texture containers
- Added basic texture importing for bch
- Added ctpk rebuilding

## Velouria Forked FEAT v1.8
- Replaced FEAT's bch parsing with Spica's

## Velouria Forked FEAT v1.7
- Added lz13 Header check 
- Adjusted UI

## Velouria Forked FEAT v1.6
- Split old lz13 and  lz13 fix compression to Normal and Extended
- Changed Extended compression to Alt instead of Ctrl
- Added more info to starting message

## Velouria Forked FEAT v1.5
- Fixed lz13 Header size
- Removed confirm prompt when compressing files

![newicon](example_pictures/Vel_Icon.png)
