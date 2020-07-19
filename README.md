# Curved-UI-Utility
A Unity package that makes it easy to curve a Screen-Space Canvas and give a more immersive first person experience.

## Why?
The idea for this project came from an effect from the Halo games, notibly *Halo 3* and *Halo: Reach*.

In those games, your HUD in first person is curved inwards a bit, further driving the feeling that you are playing as a Spartan. When going into third person, the HUD animates and curves back out to its true flat appearance.

To get a better idea of what I'm talking about, which this clip of the Master Chief entering and exiting a vehicle. Pay attention to the UI elements such as the Shield bar at the top center, and the motion tracker at the bottom left.

![Halo's HUD Effect](https://i.imgur.com/OruQrQA.gif)

For some odd reason, I am *really* interested in this effect, and wanted to recreate it in the Unity engine.

### My Solution
![Curved UI Utility](https://i.imgur.com/UuftlAc.gif)

The basis of this project is a Shader Graph that takes the UV for an image, and moves each pixel towards the center to achieve the curved effect.

This effect relies on a `RenderTexture` rendering from an outside Canvas, which is then displayed over the main Canvas through use of a `RawImage`.

If this solution is too bothersome, another potential method to look into would be a Render Feature. 

# Releases and Installation
A Unity Asset Package (`.unitypackage`) will be provided soon, and a GitHub Action to create them automatically. I'm still getting the foundations for this project up and running, so please sit tight!.

**EARLIEST UNITY VERSION TESTED:** 2019.3.15f1

This project requires *Shader Graph*, which I do not believe works with Unity's built-in renderer. I would recommend using a Scriptable Render Pipeline, such as the Universal Render Pipeline (URP), or High-Definition Render Pipeline (HDRP).

It could totally be possible that Shader Graph is unnecessary for the shader, and if that is the case, please submit a Pull Request with that change; I'm terrible at making HLSL shaders.

## Github Source
This repository, along with the source code for Curved UI Utility, also contains a demo scene that gives example implementations in code, and setups in the scene hierarchy. Feel free to clone the source and look at the provided demos to get a feel for Curved UI Utility before adding it to your own project.

## Setup
Setup is more complicated than I'd like at this point in development, but if you are interested in setting this up for your project, here's the setup process:

0. Grab a copy of Curved UI Utility.
  - If available, grab a `.unitypackage` from Releases.
  - If available, grab a `.unitypackage` from the Actions tab.
    - **WARNING!** These can be less stable than stable releases!
  - If all else fails, clone the repository and copy/paste the `Assets/Curved UI Utility` folder into your project.
1. Create a Camera designed to render the curved UI.
2. Add the `CurvedUIRenderer` component to this camera.
  - This will change the camera settings to only render UI, so you do not need to do that manually.
3. Ensure that the Canvas you want to curve has the following requirements met:
  - The canvas `Render Mode` setting is set to `Screen Space - Camera`.
    - `Screen Space - Overlay` and `World Space` will not work with Curved UI Utility.
  - The canvas `Render Camera` setting is set to the designated UI rendering camera from Step 1.
  - The layer for this Canvas object is set to `UI`.
    - Be sure to set it for all children too.
  - If need be, create a new Canvas and drag the UI you want curved over to this one.
4. Setup a Canvas that will be used to render the curved UI, as well as adding non-curved UI if you wish.
  - This can be one that already exists, but *cannot* be the same Canvas used for rendering the curved UI.
  - The `Render Mode` setting for this Canvas do not matter.
  - If not already, treat this as your new "main" UI canvas.
  - Ensure that, if present, the settings for the Canvas Scaler component are the same in both this Canvas and the curved Canvas.
5. Add the `CurvedUIController` component to this Canvas, and set the values accordingly.
  - `Starting Zoom` controls the zoom/curvature of your UI when loading the scene.
  - `Radial Scale` controls the area of effect for the curved UI. Recommended to keep as is.
  - `Curved UI Renderer` should point to the `CurvedUIRenderer` component added in Step 2.
6. Press play. If done correctly, your curved UI will show up in the Canvas created in Step 4.
  - Setup is complete, and you may now write scripts that take advantage of `CurvedUIController`.

Unfortunately, with the current setup, you lose the ability to preview your UI changes via the Game tab while in Edit mode. This is something that I will hopefully address with later iterations.

### Scripting
The `CurvedUIController` component exposes the following function that should handle all your UI curving needs:

```cs
public void SetUICurve(float curve, float transitionTime = 1f)
```

`curve` specifies the amount of curvature the UI should have, and `transitionTime` controls the transition duration (in seconds) through use of a Coroutine. A `transitionTime` of less than or equal to 0 will result in an instantaneous switch and completely bypasses the Coroutine.

You can then easily change the curvature of the UI through various events in your game. For a more gradual curve similar to the Halo games, I recommend keeping the curve value between 0.1 and 0.3.

# TODO
- Find a way to reduce initial setup for the user
  - Potential paths to look into include the use of a Render Feature.
- Test Curved UI Utility in earlier Unity versions
- Create more Demo games/scenes to further test Curved UI Utility

## License
Curved UI Utility and the Demo assets included in this project are released under the [MIT License](https://github.com/Caeden117/Curved-UI-Utility/blob/master/LICENSE).

While not a requirement by any means, I do request that attribution/credit be given in some form if Curved UI Utility is used in another Unity project.