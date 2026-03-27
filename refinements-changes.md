# Refinements / Changes Log

This log summarizes important scope shifts, implementation decisions, and refinement choices made during development.

## Initial Direction
- Started as a small prototype focused only on proving the body-hopping loop.
- The first goal was not the full game, but a test of infection, host control, camera follow, timer, host death, and return to base form.

## Major Scope Decisions
- Kept the early prototype intentionally narrow before adding police, menus, and win/lose flow.
- Built the game in slices: prototype first, terrain second, threats third, UI and presentation afterward.
- Chose to keep systems editable through the Inspector for balancing rather than hard-coding values.

## Core Gameplay Expansions
- Expanded from one host to multiple wandering hosts.
- Added direct host-to-host switching instead of forcing a return to base form between swaps.
- Added walker, swimmer, and flyer host roles to support terrain-based routing.
- Added drowning when a swimmer dies over water.
- Added flyer drop handling so Gloopy falls to ground or water correctly when a flyer expires.

## Threat / Goal Decisions
- Police only react to Gloopy in exposed base form.
- Spaceship win condition was extended to randomize between multiple possible goal locations.
- Result screens were separated from raw game-state changes so audio/visual feedback could be timed more cleanly.

## Scene / Spawning Decisions
- Replaced many manually placed hosts with runtime host spawning to reduce scene clutter.
- Added randomized host spawn groups with reusable spawn points and horizontal spread.
- Added invalid-spawn cleanup so walkers in water and swimmers on land are removed immediately.

## UI / Flow Decisions
- Added a main menu scene and result screen flow.
- Tutorial now shows when starting from the main menu, but not when restarting after a result screen.
- Tutorial ordering was changed to begin with story/context before controls and threats.
- Lose screen visuals now branch by cause: drowning vs arrest.

## Visual / Feel Decisions
- Used procedural squash/stretch rather than Animator-based character animation for early feel polish.
- Added simple shader-driven materials for water, grass, and rock to quickly improve scene readability.
- Added death squash and particle feedback for host expiration.

## Audio Decisions
- Added a central audio manager instead of one-off audio sources on many objects.
- Background music flow was split by scene/state:
  - main menu music on title screen
  - silence during tutorial
  - gameplay music restarting when play begins
  - music stops on win or loss

## AI Contribution Note
- Cursor / Codex assisted with system design iteration, C# scripting, shader creation, bug fixing, flow logic, and documentation drafting.
