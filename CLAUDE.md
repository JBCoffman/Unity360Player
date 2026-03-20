# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity360Player is a lightweight Unity project for testing and experimenting with 360° video playback. It renders equirectangular video onto an inverted sphere, with a first-person camera the user can look around using mouse input.

**Unity Version:** 2022.3.62f1 LTS

## Building

This is a Unity project with no CLI build scripts. All building is done through the Unity Editor:
- Open `Unity360Player/` as the Unity project folder
- Build via **File > Build Settings**

There are no automated tests, linting tools, or CI pipelines configured.

## Architecture

### Scene Setup (`Assets/Scenes/SampleScene.unity`)

The scene has these key GameObjects:
- **CameraRig / Main Camera** — the player's viewpoint, controlled by `SimpleMouseLook.cs`
- **Sphere** — an inverted sphere with `SphereVidMat.mat` applied; the 360° video renders onto its inner surface
- **MediaPlayer** — AVPro Video `MediaPlayer` component that drives video playback
- **FileBrowserManager** — wraps `UnityStandaloneFileBrowser` to let the user open local video files at runtime

### Core Script (`Assets/360PlayerCore/SimpleMouseLook.cs`)

The single custom script, attached to the camera. It handles:
- Mouse-look rotation (with vertical clamp ±90°)
- Cursor lock/unlock (click to lock, ESC to unlock)
- Scroll-wheel zoom (adjusts Camera FOV between `minFOV` and `maxFOV`)
- Spacebar play/pause via `AVPro MediaPlayer`
- Left/Right arrow keys to seek ±3 seconds

The `mediaPlayer` field must be wired in the Inspector to the scene's **MediaPlayer** GameObject.

### Key Third-Party Dependencies

- **AVPro Video v3.2.8** (`Assets/AVProVideo/`) — professional video plugin for Unity. Currently using the trial (watermarked) version. Handles hardware-accelerated decoding, HLS/DASH streaming, and 360°/stereo formats. The `MediaPlayer`, `DisplayUGUI`, and `ApplyToMaterial` components come from this package.
- **UnityStandaloneFileBrowser** (`Assets/Plugins/`) — native OS file picker. Excluded from git tracking due to long path issues on Windows.

### Material (`Assets/360PlayerCore/SphereVidMat.mat`)

Uses AVPro's video material shader. Configured for monoscopic (single-eye) layout. The `ApplyToMaterial` component on the Sphere feeds the decoded video texture into this material each frame.
