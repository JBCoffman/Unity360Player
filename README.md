# Unity360Player

A real-time 360° video player built in Unity. Renders equirectangular video onto an inverted sphere with a first-person camera, hardware-accelerated decoding, and a native file browser for loading local content at runtime.

I shoot family trips and everyday moments with a **Kandao Qoocam 8K** — a consumer 360° camera that captures full spherical footage. This project started as a way to watch those videos back the way they were meant to be experienced: fully immersive, looking around the room or landscape as if I'm back in that moment. Think reliving a family vacation or a kid's birthday party from inside the memory itself.

What began as a personal tool turned into a solid R&D exercise in Unity's rendering pipeline and real-time video playback architecture.

---

## Sample Footage

Don't have 360° footage handy? Download this sample clip and try it out:

**[Busy Night in Shinjuku, Tokyo](https://github.com/JBCoffman/Unity360Player/releases/tag/sample-footage)** — Shot on a Kandao Qoocam 8K. Open it in the player using the `B` key.

---

## What It Does

- Loads and plays local 360° video files (equirectangular format) at runtime
- Renders video to the inside of a sphere so the viewer is immersed in the footage
- First-person mouse-look lets you look around the full 360° environment
- Scroll-to-zoom, play/pause, and seek controls — all keyboard/mouse driven
- Native OS file picker (no UI hacks) for selecting video files

---

## Tech Stack

| Component | Tool | Notes |
|---|---|---|
| Engine | Unity 2022.3.62f1 LTS | |
| Video Decoding | AVPro Video v3.2.8 | Hardware-accelerated; supports HLS, DASH, 360°/stereo |
| File Browser | UnityStandaloneFileBrowser | Native OS picker via plugin |
| Custom Code | `SimpleMouseLook.cs` | Mouse-look, zoom, playback controls |

---

## Architecture

The scene is structured around three responsibilities:

1. **Rendering** — An inverted sphere with a custom AVPro material shader. The `ApplyToMaterial` component feeds the decoded video texture into the sphere's inner surface each frame.
2. **Playback** — AVPro's `MediaPlayer` component handles decoding, format support, and timing. Wired to the sphere material and controlled via `SimpleMouseLook.cs`.
3. **Input** — `SimpleMouseLook.cs` on the Camera handles all user input: mouse-look (±90° vertical clamp), cursor lock/unlock, scroll-wheel FOV zoom, and keyboard shortcuts for playback.

---

## Getting Started

### Prerequisites

- Unity 2022.3.62f1 LTS ([download](https://unity.com/releases/editor/archive))
- Visual Studio 2022 (or Rider)

### Setup

1. Clone the repo:
   ```bash
   git clone https://github.com/JBCoffman/Unity360Player.git
   ```

2. Open the `Unity360Player/` folder as a Unity project (not the repo root).

3. In the Unity Editor, open `Assets/Scenes/SampleScene.unity`.

4. Press **Play** — the scene is pre-wired. Use **B** to load a local video file.

> **Note:** The project uses AVPro Video in trial mode, which adds a watermark overlay during playback. A licensed version removes this.

---

## Controls

| Key / Input | Action |
|---|---|
| `B` | Browse and open a local video file |
| `Space` | Play / Pause |
| `←` / `→` | Seek ±3 seconds |
| `Mouse` | Look around |
| `Scroll Wheel` | Zoom in / out |
| `ESC` | Unlock cursor |
| `Click` | Lock cursor |

---

## Building

No CLI build scripts — all builds go through the Unity Editor:

**File → Build Settings → Select Platform → Build**

---

## Project Structure

```
Unity360Player/
├── Assets/
│   ├── 360PlayerCore/
│   │   ├── SimpleMouseLook.cs   # All custom input + playback logic
│   │   └── SphereVidMat.mat     # AVPro video material (monoscopic)
│   ├── AVProVideo/              # Third-party: AVPro Video v3.2.8
│   ├── Plugins/                 # Third-party: UnityStandaloneFileBrowser
│   └── Scenes/
│       └── SampleScene.unity    # Main scene
```

---

## License

Personal / educational project. AVPro Video is subject to its own commercial license.
