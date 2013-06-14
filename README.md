FutileAdditionalClasses
=======================

Classes to do things I want in Futile, that may not yet be a part of the official repo. I arranged these files to match the Assets directory for a Unity project. Updated for Futile version 0.91b

Get Futile: https://github.com/MattRix/Futile
Get Unity: http://unity3d.com
Get started with Futile Tutorials: http://struct.ca/futile

=======================
The additions:

Camera/HUD
----------
FCamObject: Extends FContainer, use to display HUD elements and move the camera around. Follow a specific FNode object using follow(FNode). Shake screen with shake(float). HandleResize() is there to help you reposition elements on resize or orientation change. Use setWorldBounds(Rect) to limit the camera movement within your stage. Use setBounds(Rect) to create dead space around the FNode that it is following.

FParallaxContainer: Extends FContainer, use in same scene as FCamObject to give the illusion of depth. Set the cam equal to an FCamObject, size to the size of the layer you want to display. If the size is less than the screen FParallaxContainer will stay centered. If size greater than screen & less than worldBounds, FParallaxContainer will move slower than the FCamObject. If the size is greater than worldBounds, FParallaxContainer will move faster than FCamObject. The worldBounds of the FCamObject must be set for this to work.

Fonts
-----
I added three fonts for use in Futile projects. These are meant to be pixel fonts (be sure to set Fonts.png filtering to Point), drawn by myself. Feel free to use these in your projects, credit appreciated.

Animation
---------
FAnimation: Holds frames, timing, and looping information.

FAnimatingSprite: Extends FSprite to display animating image. Add multiple FAnimations, then call them with play(NAME). Expects individual images in your texture atlas in the format NAME_#.

Tilemaps
--------
FTilemap: Extends FContainer, creates an array of FSprites based on a comma separated text file (csv). Expects individual images in your texture atlas in the format NAME_#. # matches the frame number in the csv file. A frame of zero (0) will be skipped by default.

TilemapExample.cs is an example file for your Unity project which includes uses of FTilemap, FAnimatingSprite and all three fonts.


Tiled TMX Parsing
-----------------
FTmxMap: Extends FContainer, uses XMLReader, FTilemap and FSprite to add content via Tiled .tmx files (you'll need to change their extension to txt so Unity can read them). Tiled: http://www.mapeditor.org/ 

TmxExample.cs is an example file which includes use of FTmxMap, FAnimatingSprite and all three fonts.


XML
---
XMLReader and XMLNode classes by Nissen and gregorypierce at http://forum.unity3d.com/threads/38273-Lightweight-UnityScript-XML-parser/page2 with small modifications by me.

