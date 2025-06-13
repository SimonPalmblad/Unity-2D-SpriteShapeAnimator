# Unity 2D SpriteShapeAnimator
An extension of the Unity 2D Sprite Shape package that allows for animation Spline points in the timeline.


<details open>
<summary>Instead of a boring static square...</summary>
<img src="image.png" alt="drawing" width="300"/>
</details>
<br>

<details open>
<summary><b>You can now do stuff like this!</b></summary>
<img src="https://www.dropbox.com/scl/fi/7j0tmtz0s0tky173pwqj2/Animator_Example_1.gif?rlkey=nnclrvgg2qu74piyflm7b2jzu&dl=1" alt="drawing" width="300"/>
</details>

<br>

In the current implementation of the *2D Sprite Shape*, it's impossible to animate a point moving using the Animation timeline. This package fixes that by adding a dummy `GameObject` on each spline point which can be moved via the animation timeline and updates the position of the spline.

 > [!IMPORTANT]
 > The package does currently not have support for changing spline tangents. This may or may not be included in a future update.

## How it works
Setting up animated sprite shapes is relatively straight forward. 
1. You will need a `SpriteShapeAnimatorController` on a GameObject. 
2. Create a Sprite Shape via the Unity right-click context menu: 
   - `2D Object` -> `Sprite Shape` -> `Open Shape` or `Closed Shape`.
3. Set your SpriteShapeAnimatorController as the parent of your newly created Sprite Shape.
4. 

SpriteShapeController
 -> 2DSpriteShape 