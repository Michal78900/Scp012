# Scp012
# Default config
```yml
scp012:
# Should plugin be enabled?
  is_enabled: true
  # Should debug messages be shown?
  show_debug_messages: false
  # How strongly the SCP-012 will attract plyaers to itself?
  attraction_force: 0.100000001
  # If distance between a player and SCP-012 is less then this number, the Bad Compostion will start attracting a player:
  affect_distance: 7.5
  # List of effects given to player, when ther are in AffectDistance to SCP-012:
  affect_effects:
  - Disabled
  # If distance between a player and SCP-012 is less than this number, the Bad Composition will start killing affected player:
  no_return_distance: 2.5
  # List of effects given to player when they are in NoReturnDistance to SCP-012:
  no_return_effects:
  - Ensnared
  # List of effects given to player, when they begin to die because of SCP-012:
  dying_effects:
  - Amnesia
  # Should SCP-012 affect other playable SCPs?
  allow_scps: true
  # SCP termination cassie message: (leave empty to disable)
  cassie_message: '{scp} terminated by SCP 0 1 2'
  # Should players drop their items, while interacting with SCP-012 (if set to false, after the players dies, they won't drop any items, so they are technically destroyed)
  drop_items: true
  # Should 012_BOTTOM door close, when someone interacts with SCP-012?
  auto_close_door: true
  # Should 012_BOTTOM door lock, when someone interacts with SCP-012?
  auto_lock_door: true
  # Texts shown to player that is interacting with SCP-012. Default lines are taken from SCP:CB wiki:
  i_have_to: I have to... I have to finish it.
  i_dont_think: I don't... think... I can do this.
  i_must: I... I... must... do it.
  no_choice: I-I... have... no... ch-choice!
  no_sense: This....this makes...no sense!
  is_impossible: No... this... this is... impossible!
  cant_be_completed: It can't... It can't be completed!
```
