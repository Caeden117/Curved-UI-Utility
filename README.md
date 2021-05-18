# Curved-UI-Utility
A Unity package that makes it easy to curve your HUD and give a more immersive first person experience.

## What It Is / What It's Not
If you do a quick Google search for "unity curved ui", for the most part you will see Unity assets and packages that curve your UI in *world space*, like a cylinder around the player. These kinds of solutions are great in Virtual Reality.

However, Curved UI Utility curves UI elements in *canvas space*, and is meant for overlay canvases. While your UI will indeed be curved, it will still be in the same plane that the Canvas is on.

__**In short, if you want UI that wraps around the player in world space, this is not the package for you.**__

## Why?
The idea for this project came from an effect from the Halo games, notibly *Halo 3* and *Halo: Reach*.

In those games, your HUD in first person is curved inwards a bit, further driving the feeling that you are playing as a Spartan. When going into third person, the HUD animates into its true flat appearance.

To get a better idea of what I'm talking about, which this clip of the Master Chief entering and exiting a vehicle. Pay attention to the UI elements, such as the Shield bar at the top center, and the motion tracker at the bottom left.

![Halo's HUD Effect](https://i.imgur.com/0prewAj.gif)

For some odd reason, I am *really* interested in this effect, and wanted to recreate it in the Unity engine.

### My Solution
![Curved UI Utility](https://i.imgur.com/8YRuTBa.gif)

Curved UI Utility can easily recreate the HUD curve found in games like *Destiny*, *Halo*, and *Cyberpunk 2077*, as well as being configurable enough to create your own. You do not need to recreate your entire UI to take advantage of Curved UI Utility; just slap some components and you are good to go.

# Releases and Installation

**EARLIEST UNITY VERSION TESTED:** 2019.3.15f1

I currently do not have a lot of time to test Curved UI Utility with earlier versions of Unity. As such, minimum Unity versions are also missing from the package manifest. If you test Curved UI Utility on earlier Unity versions, then please let me know whether or not you succeeded.

## Unity Package Manager

### Manual
To manually add Curved UI Utility, copy and paste this code into your project's `manifest.json` file, in the `dependencies` object:

```json
"com.caeden117.curved-ui-utility": "https://github.com/Caeden117/Curved-UI-Utility.git?path/Assets/com.caeden117.curved-ui-utility"
```

### OpenUPM
Have OpenUPM? Cool. You can head over to the [OpenUPM page](https://openupm.com/packages/com.caeden117.curved-ui-utility) and install the `installer.unitypackage` file, and import that directly into your Unity project. The installer will download the contents and automatically install the package.

If you happen to prefer the CLI, then you can execute this command: `openupm add com.caeden117.curved-ui-utility`

## GitHub Releases
If that doesn't suit your fancy, you can go to the [Releases page](https://github.com/Caeden117/Curved-UI-Utility/releases) and download the latest `Curved.UI.Utility.zip` file. From there, you can add it to your Unity project as a local package.

## GitHub Actions
You can also add Curved UI Utility by going into the [Actions page](https://github.com/Caeden117/Curved-UI-Utility/actions) and downloading the latest successful artifact. From there, you can add it to your Unity project as a local package.

## GitHub Source
This repository is actually the project that I use to develop Curved UI Utility, which explains the long Git URL for the Package Manager.

With the project, the repository also contains a demo scene that gives an example implementation of Curved UI Utility. Feel free to clone the source and browse through the project before adding it to your own.

# Setup
1. Add the `CurvedUIController` component to the Canvas that you wish to curve.
    - `Settings Source` determines the initial curve settings.
        - `From Scriptable Object` will inherit settings from a `CurvedUISettingsObject` asset. A couple of these are included in the package, and you can also create your own.
        - `From Starting Settings` will give you basic HUD settings to play around with.
    - `Curve Transition` affects the transition between different HUD settings.
2. Add/replace various components to child UI objects you wish to curve.
    - You can right-click the Hierarchy and add `Curved UI Utility` objects via the `UI` context submenu.
    - If you need to replace existing components
        - `Image`s should be replaced with `CurvedImage`, which increases the mesh detail for a smoother curve.
        - TextMeshPros should be replaced with `CurvedTextMeshPro`.
    - **`CurveComponent` should be added to every component except `CurvedTextMeshPro`, including `CurvedImage`.**
    - For masking to work properly, all `RectMask2D`s should be replaced with `Mask`s with curved images.
3. Press play. If done correctly, your curved UI will show up.
    - Setup is complete, and you may now write scripts that take advantage of `CurvedUIController`.

# Known Issues
### Mouse input will never properly hit/trigger Curved UI
This is because Curved UI Utility is purely visual. Unity's mouse collision system makes the assumption that *all* UI elements are flat.

The closest workaround available is to hide the user's mouse cursor, and use a `CurvedImage` that matches the position of the cursor.

### Certain UI elements are not compatbile with Curved UI Utility
I do not currently have the time to implement, test, and maintain compatibility with anything outside of basic Unity UI and TextMeshPro components. `CurveComponent` should hopefully be compatible with a lot of existing UI elements, as well as a handful of external ones.

If you need to add special compatibility for your own custom UI, consider forking this repository, and making a pull request.

# Documentation

Currently WIP. I plan on utilizing the GitHub Wiki to document the various components and features of Curved UI Utility.

For the time being, most of what you would want comes from the `CurvedUIController` class.

# TODO
- Add some editor tooling to speed up initial setup.
- Test Curved UI Utility in earlier Unity versions
- Create more Demo games/scenes to further test Curved UI Utility

## License
Curved UI Utility and the Demo assets included in this project are released under the [MIT License](https://github.com/Caeden117/Curved-UI-Utility/blob/master/LICENSE).

While not a requirement by any means, I do request that attribution/credit be given in some form if Curved UI Utility is used in another Unity project.
