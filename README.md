# Erosion v4 ROM and file corrupter
A small file corrupter project from 2016 emerged from the need for an actual fast and advanced ROM and file corrupter. Only the binary was released, being this the first time the source code is available.

![screenshot](https://3.bp.blogspot.com/-MaWSUoXSnGE/V0IM5PWF8DI/AAAAAAAAAc4/FCD1kqlU9D0RH-p6O0LnG9mrczItZZ0eACLcB/s400/1.png)

## Getting Started
### What's the purpose of this program?
With this program you can change arbitrary bytes in ROMs, documents and files to see the corruption results.

Also can help you to know where are the contents of a file and find the offsets, for example:

If you want to know where are the graphics of a ROM, just try to erode the rom at different intervals of the file with the slider until the result is corrupt graphics

### Why Erosion v4?
At the time this tool was made, most ROM and file corrupters were all outdated.
The ROM corruptor I used was named Erosion v3 and was made by **_Syizm_** in 2003. This tool was a bit slow and improvable.

Erosion v4 tries to be a successor version of this tool respecting the style of the interface, beeing much faster, easy to use, and including some new options.

### How to use
The first of all is to load the file you want to erode(corrupt) then you can choose between making a backup of the file checking the checkbox (In the same folder of the program) or not. Remember that if you dont make a backup, eroded file will become permamently damaged.

Once the file is opened you can select the interval of the file which will be eroded inserting the offsets in hexadecimal or using the sliders.

For the corruption options, the program is divided into filters , intervals , and new values.

* **Filters:** Allows you to avoid eroding certain bytes. To do this you must select that unique bytes that will be changed. This allows you to for example avoid eroding sync bytes in a stream, or just erode That Kind of bytes.
* **Intervals:** Select how often the bytes are changed. Very low values ​​will make the program finish much later and make the file useless. We recommend testing with +-1000 and up or down depending on the results. This range can also be random if they want to see different results with the same level of erosion.
* **New values:** Allows you to choose the value of the byte that will replace the previous one. This can be fixed, random, or based on the previous one.

## Easter Egg
At the time Erosion v4 was first released in 2016, the source code wasn't available and the easter egg was a nice thing. Now with the source code beeing here, it has not much sense, and I didn't removed it because I wanted to upload the code as it was in 2016.

## License

The **Crc32.cs** class used to calculate file CRCs:

>
// Copyright (c) Damien Guard.  All rights reserved.
>
// Licensed under the Apache License, Version 2.0 (the "License");
>
// Originally published at http://damieng.com/blog/2006/08/08/calculating\_crc32\_in\_c\_and\_net

You can freely use the source and tool, but must contact me if you want to edit and/or distribute.

## Links

Full blog article:
<http://tragicomedy-hellin.blogspot.com/2016/05/ya-disponible-erosion-v4.html>