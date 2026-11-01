# Personal Space in VR

**When does an approaching stranger feel too close, and does it depend on how fast they walk?**

A small virtual-reality experiment built in Unity. A virtual person walks toward you; you pull the
trigger the moment they feel uncomfortably close. The distance at that moment is your
*comfort distance*. Walking speed and walking path are varied across trials.

▶ **[Try the browser version](https://shabnamshirdel.github.io/PersonalSpaceVR/)** (flat-screen preview of the same design, no headset needed)

<!-- Replace with a GIF of the headset view: drag the file into this README on GitHub -->
<!-- ![Headset view](media/demo.gif) -->

## Why

How much space people want around them (*proxemics*) matters for anything that moves among
people, including social robots, which have to decide how close to pass someone in a hallway.
VR is a natural way to measure this: the approaching person is fully controlled, repeatable and
safe, yet in a headset it still *feels* like someone is walking up to you.

## Design

| | |
|---|---|
| **Task** | Pull the trigger when the walker feels too close |
| **Walking speed** | 0.6, 1.0, 1.4 m/s |
| **Path** | Head-on, or passing 0.6 m to the left/right |
| **Trials** | 3 speeds × 2 paths × 4 repetitions = 24, shuffled |
| **Start distance** | 6 m |
| **Measure** | Horizontal head-to-walker distance at response (m), response time (s) |

Trials are self-paced (A button to start) with a random 0.5–1.5 s onset delay so the start of
each approach can't be anticipated.

## Results

<!-- Add your plot once you have pilot data:
![Comfort distance by walking speed](media/comfort_distance.png) -->
*Pilot data coming soon.*

## Running it

**Requirements:** Unity 6 (6000.x) or 2022.3 LTS with the High Definition 3D template,
XR Interaction Toolkit, and an OpenXR/Oculus headset (tested setup: Meta Quest).

1. Clone the repo and open the folder in Unity Hub (**Add → Add project from disk**).
2. Open the experiment scene, then set **Edit → Project Settings → Player → Active Input Handling → Both**.
3. Select **ExperimentManager** in the Hierarchy and set **Participant ID**.
4. Press Play.

| Action | Quest controller | Keyboard (testing) |
|---|---|---|
| Start trial | A button | Space |
| Too close | Right trigger | Enter |

Data are saved as one CSV per session in `Application.persistentDataPath` (the full path is printed
in the Console). Plot them with:

```bash
python analysis/analyze.py path/to/P01_*.csv
```

## Repository layout

```
Assets/Scripts/ExperimentManager.cs   trial loop, controller input, CSV logging
Assets/Scripts/Walker.cs              moves the virtual person
analysis/analyze.py                   comfort distance by speed and path
docs/index.html                       browser (Three.js) version of the experiment
```

## Next steps

- **Human vs. robot:** swap the walker for a robot model. Do people give robots the same space?
- **Gaze:** walker looks at you vs. looks away.
- **Active walking:** the participant walks past a standing person, and their path is recorded.

## Acknowledgements

Built after the IS&T CIC34 short course *SC02: Creating Perceptual and Behavioral Experiments with
Unity and VR* by Richard Murray (York University).

---
Shabnam Shirdel
