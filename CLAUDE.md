# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A 3D gravity-manipulation puzzle game built in Unity 6 (6000.0.66f2). The player navigates a stage by changing the direction of gravity to reach a goal while avoiding hazards.

## Development Environment

- **Unity Version**: 6000.0.66f2
- Open and run the project through the Unity Editor — there are no CLI build commands.
- All game logic is in `Assets/Scripts/`. Scenes are in `Assets/Scenes/`.

## Architecture

### Singleton Managers (scene-persistent)

- **`GravityManager`** — Central authority for gravity direction. Holds the global `GravityDirection` (one of the 6 axis-aligned unit vectors). Exposes `OnGravityChanged` event that other components subscribe to. Changes gravity via mouse buttons (left = player-left, right = player-right relative to the `target` transform). Applies changes via `Physics.gravity`.

- **`GameFlowManager`** — State machine with three states: `Playing`, `Cleared`, `GameOver`. Tracks elapsed time. Exposes `OnGameOverOccurred` and `OnGameClearOccurred` events. Handles scene reloading (`RetrySceneRoutine`) and next-scene loading (`LoadNextSceneRoutine`).

### Event-Driven Pattern

Scripts communicate through C# events rather than direct references:
- `GravityManager.OnGravityChanged` → subscribed by `PlayerController`, `AntiGravityObject`, `UIManager`
- `GameFlowManager.OnGameOverOccurred` / `OnGameClearOccurred` → subscribed by `UIManager`
- All subscribers unsubscribe in `OnDestroy()` to prevent memory leaks.

### Game Object Scripts

| Script | Purpose |
|---|---|
| `PlayerController` | WASD/arrow movement, mouse-X for horizontal rotation. Uses Quaternion.Slerp to smoothly align player's up-axis to inverse gravity direction. |
| `CameraFollow` | Third-person camera with SphereCast collision avoidance and mouse-Y vertical orbit. Runs in `LateUpdate`. |
| `AntiGravityObject` | Rigidbody that moves opposite to global gravity (used for the goal object). Can be toggled to normal gravity via `normalGravity()` / `revesalGravity()`. |
| `GoalArea` | Collision-based trigger that calls `GameFlowManager.StageClear()` when the Player tag enters. |
| `KillZone` | Trigger that calls `GameFlowManager.GameOver()` when Player enters. |
| `OutStage` | Calls `GameOver()` when Player exits its trigger bounds (stage boundary). |
| `PressureSwitch` | Trigger-based switch with `UnityEvent` hooks (`onActivate`, `onDeactivate`) wired in the Inspector — used to open/close doors. |
| `Door` | Lerp-based door animation; `OpenDoor()` / `CloseDoor()` called by PressureSwitch events. |
| `DroneAI` | State machine enemy (Idle → Chase). Uses distance + field-of-view + Raycast line-of-sight to detect the Player tag. |

### UI

`UIManager` uses TextMeshPro for HUD text (gravity direction, timer). `gameOverPanel` and `gameClearPanel` are GameObjects toggled on state change events. Retry/Exit buttons call `GameFlowManager` methods directly.

### Required Tags

Objects must have the `Player` tag for `GoalArea`, `KillZone`, `OutStage`, and `DroneAI` detection to work correctly.

### Input

Uses the legacy `Input` class (mouse buttons via `Input.GetMouseButtonDown`, axes via `Input.GetAxis`). `UnityEngine.InputSystem` is imported in some files but not fully adopted.
