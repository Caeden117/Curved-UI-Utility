# Curved-UI-Utility
A Unity package that makes it easy to curve your HUD and give a more immersive first person experience.

## Why?
The idea for this project came from an effect from the Halo games, notibly *Halo 3* and *Halo: Reach*.

In those games, your HUD in first person is curved inwards a bit, further driving the feeling that you are playing as a Spartan. When going into third person, the HUD animates and curves back out to its true flat appearance.

To get a better idea of what I'm talking about, which this clip of the Master Chief entering and exiting a vehicle. Pay attention to the UI elements such as the Shield bar at the top center, and the motion tracker at the bottom left.

![Halo's HUD Effect](https://i.imgur.com/0prewAj.gif)

For some odd reason, I am *really* interested in this effect, and wanted to recreate it in the Unity engine.

### My Solution
![Curved UI Utility](https://i.imgur.com/UuftlAc.gif)

The basis of this project is essentially a mesh modifier that distorts the UI mesh.

# Releases and Installation

**EARLIEST UNITY VERSION TESTED:** 2019.3.15f1

I currently do not have a lot of time to test Curved UI Utility with earlier versions of Unity. As such, minimum Unity versions are also missing from the Unity package manifest. If you are successfully able to get Curved UI Utility working with an earlier Unity version, please let me know and I will update the README.

## Unity Package Manager
To add Curved UI Utility as a dependency package via Git, please copy and paste the dependency code into your project's `manifest.json` file:

```json
{
  "dependencies": {
    "com.caeden117.curved-ui-utility": "https://github.com/Caeden117/Curved-UI-Utility.git?path/Assets/com.caeden117.curved-ui-utility"
  }
}
```

## GitHub Actions
You can also add Curved UI Utility as a dependency package by going into the [Actions page](https://github.com/Caeden117/Curved-UI-Utility/actions) and downloading the latest successful artifact. From there, you can add it to your Unity project as a local package.

## GitHub Source
This repository is actually the project that I use to develop Curved UI Utility. Furthermore, it contains a demo scene that gives example implementations in code. Feel free to clone the source and look at the provided demos to get a feel for Curved UI Utility before adding it to your own project.

#### Available Demos
- **First Person with Vehicle**
  - This demo includes a complete example of how to use Curved UI Utility.

# Setup
**Setup is more complicated than I'd like at this point in development**, but if you are interested in setting this up for your project, here's the setup process:

0. Grab a copy of Curved UI Utility.
1. Add the `CurvedUIController` component to the Canvas that you wish to curve.
    - `Settings Source` determines where exactly the initial curve settings will come from.
        - `From Scriptable Object` will inherit settings from an `CurvedUISettingsObject` asset. A couple of these are included in the package.
        - `From Starting Settings` will give you basic HUD settings to play around with.
    - `Curve Transition` affects the transition between different HUD settings.
2. Add/replace various components to child UI objects.
    - `Image`s should be replaced with `CurvedImage`, which increases the mesh detail for a smoother curve.
    - Any TextMeshPro or Unity UI text should be replaced with `CurvedTextMeshPro`.
    - `CurveComponent` should safely work with other misc. components and custom graphics.
3. Press play. If done correctly, your curved UI will show up.
    - Setup is complete, and you may now write scripts that take advantage of `CurvedUIController`. 

# Documentation

Currently WIP. I plan on utilizing the GitHub Wiki to document the various components and features of Curved UI Utility.

# TODO
- Add some editor tools to speed up setup on existing UI configurations.
- Test Curved UI Utility in earlier Unity versions
- Create more Demo games/scenes to further test Curved UI Utility

## License
Curved UI Utility and the Demo assets included in this project are released under the [MIT License](https://github.com/Caeden117/Curved-UI-Utility/blob/master/LICENSE).

While not a requirement by any means, I do request that attribution/credit be given in some form if Curved UI Utility is used in another Unity project.