# Inputs Recheck
Hollow Knight mod that rechecks input states on state changes. Requires [Vasi](https://github.com/fifty-six/HollowKnight.Vasi/tree/master/Vasi).

The mod provides toggles (in Mod Menu) for re-checking the following inputs:
- Jump
- Dash
- Attack
- Cast
- Quick Cast
- Focus
- Dream Nail
- Superdash

There is also a toggle to auto-release superdash. When the setting is on, you need to hold the direction you are facing.

# How it works
The mod checks at every Knight's state change (idle, falling, etc) for what actions does it allow, and stores it internally. And when the state
allows for something that was previously not allowed, it checks if the button that that should trigger the action is held down, and if it is,
triggers the action.
Say, you hit jump a millisecond too early, before you landed on the surface — vanilla would just ignore it until you release the button and press
it again, while this mod would trigger the jump as soon as your feet touch the ground.

# Why it doesn't work like that in vanilla
In vanilla the game doesn't care at all for what is the state of you buttons, be them held or released, it only reacts to the events of push and
release, which would be fine if it won't just ignore those when the action is not available at that exact moment, even if it will become available
the very next frame. And when the action becomes available, the game still doesn't care that you're still pushing the button with all your force,
for no keyboard state change = no reaction in its logic.
