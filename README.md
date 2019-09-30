###### This account is for potential employers.
###### Potential employers are welcome to request game keys from boldbrushgames@gmail.com via their company's email address, with which they can try out the game on [Steam (for Windows OS)](https://store.steampowered.com/app/1128610/Affinity/) at no cost.
<p align="center">
<img src=/images/library_logo.png>
</p>

## Contents

- 1.0 About
- 1.1 Code Contributions by Nathan Grimes
- 1.2 Code Contributions by Hunter Duzac
### 1.0 About
_Affinity_ is a relaxing puzzle game with a minimalist aesthetic. Players piece together handmade jigsaw puzzles while listening to soothing music produced by their playing and the background theme.

<p align="center">
<img src=/images/image1.png>
<img src=/images/image2.png>
</p>

### Game / Project Features
* Scenes transition automatically with a fade in / fade out.
* Game functions in a synchronized multi-scene environment.
* Sound management system plays instrument chords via factory pattern. Played when pieces are placed where they belong.
* Sound effects trigger with a cycling randomized series of tones that never play more than once per cycle.
* Pieces have lift and lean when moved, simulating being lifted and moved from a tabletop.
* Piece together parts of the puzzle before putting them where they belong.
![Join pieces together, then place them!](/images/image3.gif)

* Automated asset management during level design process streamlined level creation.
* The player can select which level they want to start from after starting a new game.
* Pieces automatically layer atop one another based on when they were moved.
* Pieces dropped beyond the scope of the viewport are automatically shifted back into the play area.
* Pieces remain grayscale until the player places them where they belong.
* Save/Load

## 1.1 Code Contributions by Nathan Grimes
A scene manager acts as the primary entry point for the program. It includes all sound systems, foreground fade-in/fade-out systems, and scene management systems. The `SceneController.cs` script is built with modularity to allow any other script or unity component extend its functionality. If a specific scene needs to be loaded, a scene name can be provided, otherwise it automatically loads game levels incrementally by scene name "Level" + level value. The scene manager loads and unloads other scenes that run simultaneously, these scenes are built to communicate requests across this multi-scene environment.

The `SoundFactory.cs` script uses the factory design pattern to play the instrumental tones triggered by a player moving a piece or placing a piece in the correct location. A cycling randomized list with logic that asserts unique tones guarantees that the player will hear a different tone each time they trigger one.

When pieces are moved, they lean in a smooth motion based on the direction they moved relative to where they started, as shown in the `PuzzlePiece.cs` script. Puzzle pieces also store their neighbors during the level design process by searching for points on neighboring colliders, the game uses these neighbors to allow the player to join pieces together before setting the combined pieces where the belong, as shown in the gif above.

## 1.2 Code Contributions by Hunter Duzac
Coming soon...

