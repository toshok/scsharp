Bomb Run
Copyright (c) 2003 CL Game Stuidios (Sijmen Mulder)
This program (game) is published under the terms of the GNU General Public Licence version 2.

The goal of the game is to avoid as many bombs as possible, before being hit by one.

Features:
 - New original 2D graphics technology developped by CL Game Studios (thats me :P)
 - Varying bomb speeds
 - Challenging bomb speed algoritm
 - Jump ability

The bomb speed algoritm is as follows:

* new bomb speed is incremented by 3 pixels/sec each second
* once the bomb speed reaches MAXSPEED, the speed is halved and MAXSPEED multiplied by two

This technology creates 'waves like' speed bursts while the upward difficulty line is maintained.

The graphics technology is as follows. The illusion is given to the player that two images are behind each other, and that the objects are cutouts in the front image.

The truth is, that from a black/white image the black part is blitted on a temporary surface that already contains the back image part of that spot, and then that temporary image is blitted with a black colorkey.

Anyways, have fun!

--
Sijmen Mulder

This game is entirely (except from the current background images) made by me, Sijmen Mulder.

I am currently 16 years old and live in the Netherlands, you can find more information about me on my website (www.wpws.nl/~sijmen).

My email address is sjmulder at sourceforge dot net.
