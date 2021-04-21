# Scp012
# Config
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
  # Should blood decals be spawnd underneath a player?
  spawn_blood: true
  # Should SCP-012 be respawned if it is too far from proper position? (SCP-012 can be moved by using grenades)
  allow_item_respawn: false
  # List of effects given to player, when they are in AffectDistance to SCP-012:
  affect_effects:
  - Disabled
  # If distance between a player and SCP-012 is less than this number, the Bad Composition will start killing affected player:
  no_return_distance: 2.5
  # List of effects given to player when they are in NoReturnDistance to SCP-012:
  no_return_effects:
  - Ensnared
  # List of effects given to player, when they begin to die because of SCP-012:
  dying_effects:
  - Bleeding
  # Should damage-dealing effects hurt affected player?
  effects_damage: false
  # List of items which may be spawned inside SCP-012 to bait player to come closer: (valid formating: - ItemType: chance)
  bait_items:
  - Medkit: 100
  # Should bait items that are weapons or ammo be fully loaded?
  loaded_bait_weapons: true
  # List of roles, that will be ignored by SCP-012:
  ignored_roles:
  - Scp173
  # SCP termination cassie message: (leave empty to disable)
  cassie_message: '{scp} terminated by SCP 0 1 2'
  # Should players drop their items, while interacting with SCP-012 (if set to false, the items will be deleted)
  drop_items: true
  # After what time (in seconds) from player death, should bodies near SCP-012 be cleaned up? (set 0 to disable)
  ragdoll_cleanup_delay: 10
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

# Translations
```yml
  translations:
    i_have_to: I have to... I have to finish it.
    i_dont_think: I don't... think... I can do this.
    i_must: I... I... must... do it.
    no_choice: I-I... have... no... ch-choice!
    no_sense: This....this makes...no sense!
    is_impossible: No... this... this is... impossible!
    cant_be_completed: It can't... It can't be completed!
 ```
