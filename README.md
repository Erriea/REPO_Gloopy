# Gloopy Goes Home

Gloopy Goes Home is a Unity game jam project about a stranded alien parasite that must steal bodies to survive, traverse a dangerous planet, and reach its spaceship.

## Overview
You play as Gloopy, a small alien that cannot move in its base form. To get home, you must infect nearby hosts, move through the world using their abilities, and keep swapping before each host dies. Different host types are needed for different terrain, and police become a threat whenever Gloopy is exposed.

## Features
- body-hopping infection mechanic
- three host types: walker, swimmer, flyer
- water, obstacles, and terrain-based traversal
- police chase and arrest loss condition
- drowning loss condition
- randomized host spawning
- randomized ship spawn location
- tutorial, result screens, and main menu
- custom simple shaders for water, grass, and rock

## Controls
- `WASD`: move current host
- `Mouse Left Click`: target host
- `Space`: infect selected host

## Scenes
- `MainMenu`: title screen and entry point
- `SampleScene`: main gameplay scene

## Installation / Run
1. Open the project in Unity `6000.0.44f1`.
2. Load the `MainMenu` scene.
3. Press Play in the Unity Editor.

## Dependencies
Main packages in use:
- Universal Render Pipeline
- Input System
- Unity UI / TextMeshPro
- AI Navigation

See [requirements.txt](/C:/Code/2026%20Unity/REPO_Gloopy/requirements.txt) for the package list captured from the project manifest.

## Credits
- Game concept, design direction, scene setup, art selection, and final creative decisions: project author
- AI-assisted coding and iteration support: Cursor / Codex

## AI Use Note
AI was used to help design systems, generate and revise gameplay code, create simple custom shaders, debug implementation issues, and draft supporting documentation.
