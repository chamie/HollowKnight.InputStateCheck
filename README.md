# Inputs Recheck

A Hollow Knight mod that fixes missed inputs caused by the game's edge-triggered input system.

Requires [Vasi](https://github.com/fifty-six/HollowKnight.Vasi/tree/master/Vasi).

## The Problem

Hollow Knight's input system only reacts to button *press* and *release* events — it ignores the held state of a button entirely. This means that if you press jump a frame before you land, or dash a frame before your cooldown expires, the game discards the input completely. You have to release the button and press it again. This affects jump, dash, attack, spells, dream nail, and superdash.

Input buffers (as attempted by other mods) work around this by storing inputs for a short time window and replaying them. This works, but it's an approximation — the buffer window is a guess, and it can fire inputs at unintended times.

## The Solution

Instead of buffering, this mod takes a snapshot of which actions are available each fixed update. When an action transitions from *unavailable* to *available*, it checks whether the corresponding button is currently held — and if so, triggers the action immediately. No guessing, no timing windows. The input fires exactly when it becomes possible, and only if you're still holding the button.

## Features

All of the following re-check behaviours can be toggled individually in the Mod Menu:

- **Jump** (including wall jump and double jump)
- **Dash**
- **Attack**
- **Cast / Focus**
- **Quick Cast**
- **Dream Nail**
- **Superdash**

There is also an optional **Superdash Auto-Release** toggle: when enabled, holding the direction you are already facing while fully charged will automatically release the superdash, so you don't have to release and re-press the button.

## Installation

Install [Lumafly](https://themulhima.github.io/Lumafly/) or another mod manager, it will be in the list of available mods

Or install manually:
1. Ensure [Vasi](https://github.com/fifty-six/HollowKnight.Vasi/tree/master/Vasi) is installed.
2. Drop `InputsRecheck.dll` into your `Mods` folder.
