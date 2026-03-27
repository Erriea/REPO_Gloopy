# Development Plan

## Project Goal
Build a complete, playable game loop for **Gloopy Goes Home** in Unity, beginning from a small prototype and expanding it into a polished game jam submission.

## Current Status
Core gameplay is implemented and playable:
- main menu flow
- tutorial flow
- body-hopping / infection mechanic
- walker, swimmer, and flyer host types
- timed host death
- drowning loss
- police arrest loss
- spaceship win condition
- result screens
- randomized host and ship spawn setup
- basic audio hooks
- simple stylized environment shaders

## Completed Milestones
1. Prototype body-hopping loop
2. Multiple hosts and host wandering
3. Target feedback and range feedback
4. Terrain restrictions and water gameplay
5. Flying hosts and vertical behavior
6. Police threat and arrest loss
7. Spaceship win condition
8. Result UI and restart/menu flow
9. Tutorial onboarding
10. Runtime spawning for hosts and ship
11. Audio system hooks
12. Basic visual shader pass

## Remaining Work
1. Final polish and tuning
2. Audio balancing and verification
3. Art consistency pass
4. Final map readability improvements
5. Build testing and bug fixing
6. Submission packaging

## Suggested Final Task List
- test all win/lose conditions end-to-end
- verify tutorial, restart, and main menu flow
- tune host counts, movement speeds, and timer duration
- verify audio clips and volumes
- replace temporary art/UI where needed
- test on target build platform
- capture screenshots / footage if needed for submission

## AI Tools Used
- Cursor / Codex for gameplay scripting, shader setup, debugging, and iteration support
- AI-assisted drafting for documentation and development tracking

## Working Approach
- build one slice at a time
- test after each major feature
- prefer simple systems over over-engineering
- keep Inspector values editable for balancing
