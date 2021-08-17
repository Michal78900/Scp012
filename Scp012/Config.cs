﻿namespace Scp012
{
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class Config : IConfig
    {
        [Description("Should plugin be enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should debug messages be shown.")]
        public bool Debug { get; set; } = false;

        [Description("How strongly the SCP-012 will attract players to itself.")]
        public float AttractionForce { get; set; } = 0.1f;

        [Description("If distance between a player and SCP-012 is less then this number, the Bad Compostion will start attracting a player.")]
        public float AffectDistance { get; set; } = 7.5f;

        [Description("Should blood decals be spawned underneath a player.")]
        public bool SpawnBlood { get; set; } = true;

        [Description("List of effects given to player, when they are in AffectDistance to SCP-012:")]
        public List<EffectType> AffectEffects { get; set; } = new List<EffectType>()
        {
            EffectType.Disabled,
        };

        [Description("If distance between a player and SCP-012 is less than this number, the Bad Composition will start killing affected player.")]
        public float NoReturnDistance { get; set; } = 2.5f;

        [Description("List of effects given to player when they are in NoReturnDistance to SCP-012:")]
        public List<EffectType> NoReturnEffects { get; set; } = new List<EffectType>()
        {
            EffectType.Ensnared,
        };

        [Description("List of effects given to player, when they begin to die because of SCP-012:")]
        public List<EffectType> DyingEffects { get; set; } = new List<EffectType>()
        {
            EffectType.Bleeding,
        };

        [Description("Should damage-dealing effects hurt affected player.")]
        public bool EffectsDamage { get; set; } = false;

        [Description("List of items which may be spawned inside SCP-012 to bait player to come closer: (valid formating: - ItemType: chance)")]
        public List<Dictionary<ItemType, int>> BaitItems { get; set; } = new List<Dictionary<ItemType, int>>
        {
            new Dictionary<ItemType, int> { { ItemType.Medkit, 100 } },
        };

        [Description("Should bait items that are weapons or ammo be fully loaded.")]
        public bool LoadedBaitWeapons { get; set; } = true;

        [Description("List of roles, that will be ignored by SCP-012:")]
        public List<RoleType> IgnoredRoles { get; set; } = new List<RoleType>
        {
            RoleType.Scp173
        };

        [Description("SCP termination cassie message: (leave empty to disable)")]
        public string CassieMessage { get; set; } = "{scp} terminated by SCP 0 1 2";

        [Description("Should players drop their items, while interacting with SCP-012 (if set to false, the items will be deleted)")]
        public bool DropItems { get; set; } = true;

        [Description("After what time (in seconds) from player death, should bodies near SCP-012 be cleaned up. (set 0 to disable)")]
        public float RagdollCleanupDelay { get; set; } = 10;

        [Description("Should 012_BOTTOM door close, when someone interacts with SCP-012.")]
        public bool AutoCloseDoor { get; set; } = true;

        [Description("Should 012_BOTTOM door lock, when someone interacts with SCP-012.")]
        public bool AutoLockDoor { get; set; } = true;
    }
}
