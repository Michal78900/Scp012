namespace Scp012
{
    using System;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using System.Collections.Generic;
    using MEC;
    using UnityEngine;
    using Interactables.Interobjects.DoorUtils;
    using Mirror;
    using System.Linq;

    using Random = UnityEngine.Random;

    public partial class Handler
    {
        public static Pickup Scp012Item;

        private Vector3 itemSpawnPos;
        private Quaternion itemRotation;
        private Vector3 baitItemSpawnPos;

        private DoorVariant Scp012BottomDoor;

        private List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

        private List<Player> PlayersInteracting = new List<Player>();

        private bool scp012death = false;

        private readonly Dictionary<RoleType, string> RoleToString = new Dictionary<RoleType, string>
        {
            { RoleType.Scp049, "SCP 0 4 9" },
            { RoleType.Scp096, "SCP 0 9 6" },
            { RoleType.Scp106, "SCP 1 0 6" },
            { RoleType.Scp173, "SCP 1 7 3" },
            { RoleType.Scp93953, "SCP 9 3 9" },
            { RoleType.Scp93989, "SCP 9 3 9" },
        };

        internal void OnWaitingForPlayers()
        {
            Scp012BottomDoor = Map.GetDoorByName("012_BOTTOM");

            Log.Debug($"Door rotation: {Scp012BottomDoor.transform.eulerAngles}", Config.Debug);

            itemRotation = Quaternion.Euler(OffsetRotation(Scp012BottomDoor.transform, -90f, 90f, 0f));
            itemSpawnPos = OffsetPosition(Scp012BottomDoor.transform, 8.5f, 0.5f, 1.75f);
            baitItemSpawnPos = OffsetPosition(Scp012BottomDoor.transform, 6.5f, 1.8f, 1.75f);

            SpawnScp012Item();
        }

        internal void OnRoundStart()
        {
            foreach (CoroutineHandle coroutine in coroutines)
            {
                Timing.KillCoroutines(coroutine);
            }
            coroutines.Clear();

            coroutines.Add(Timing.RunCoroutine(ScpMechanic()));
            coroutines.Add(Timing.RunCoroutine(MovePlayer()));

            Log.Debug("Scp012 coroutines started!", Config.Debug);

            SpawnBaitItems();
        }

        internal void OnDestroy(DestroyingEventArgs ev)
        {
            if (PlayersInteracting.Contains(ev.Player))
            {
                PlayersInteracting.Remove(ev.Player);
            }
        }

        internal void OnDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Door == Scp012BottomDoor)
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnItemPickup(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup == Scp012Item || PlayersInteracting.Contains(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnItemDrop(DroppingItemEventArgs ev)
        {
            if (PlayersInteracting.Contains(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }

        internal void OnHurting(HurtingEventArgs ev)
        {
            if (PlayersInteracting.Contains(ev.Target) && !Config.EffectsDamage)
            {
                var dmgType = ev.HitInformations.GetDamageType();

                if (dmgType == DamageTypes.Asphyxiation || dmgType == DamageTypes.Bleeding || dmgType == DamageTypes.Poison)
                    ev.Amount = 0f;
            }
        }

        internal void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            if (scp012death && ev.HitInfo.GetDamageType() == DamageTypes.Bleeding)
            {
                ev.IsAllowed = false;

                if (!string.IsNullOrEmpty(Config.CassieMessage))
                {
                    string message = Config.CassieMessage;
                    message = message.Replace("{scp}", $"{RoleToString[ev.Role.roleId]}");

                    Timing.CallDelayed(0.5f, () =>
                    {
                        var scps = Player.Get(Team.SCP);
                        if (scps.Count(scp => scp.Role == RoleType.Scp079) > 0 && scps.Count() == 1)
                            message += (" . .  ALLSECURED . SCP 0 7 9 RECONTAINMENT SEQUENCE COMMENCING . FORCEOVERCHARGE");

                        Cassie.GlitchyMessage(message, 0.05f, 0.05f);
                    });
                }
            }
        }

        internal void SpawnScp012Item()
        {
            Scp012Item = Item.Spawn(ItemType.KeycardO5, 0, itemSpawnPos, itemRotation);

            Log.Debug($"Item pos: {itemSpawnPos}", Config.Debug);

            GameObject gameObject = Scp012Item.gameObject;

            gameObject.transform.localScale = new Vector3(0.01f, 175f, 15f);

            var rigidbody = gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.freezeRotation = true;
            rigidbody.mass = 100000;

            NetworkServer.UnSpawn(gameObject);
            NetworkServer.Spawn(gameObject);

            Log.Debug("SCP-012 item spawned successfully!", Config.Debug);
        }

        internal void SpawnBaitItems()
        {
            for (int i = 0; i < Config.BaitItems.Count; i++)
            {
                var baitItem = Config.BaitItems[i].First();

                ItemType item = baitItem.Key;

                float durability = 0;

                if (Config.LoadedBaitWeapons)
                    durability = item.GetDefaultDurability();

                int chance = baitItem.Value;

                if (Random.Range(0, 101) < chance)
                {
                    Item.Spawn(item, durability, new Vector3(baitItemSpawnPos.x + Random.Range(-6.0f, 6.0f), baitItemSpawnPos.y, baitItemSpawnPos.z + Random.Range(-6.0f, 6f)), Random.rotation);

                    Log.Debug($"{item} bait item has been spawned inside of SCP-012 chamber!", Config.Debug);
                }
            }
        }

        private Vector3 OffsetPosition(Transform location, float forward, float up, float right) =>
            location.position + (location.forward * forward) + (location.up * up) + (location.right * right);

        private Vector3 OffsetRotation(Transform location, float x, float y, float z) =>
            location.eulerAngles + (Vector3.right * x) + (Vector3.up * y) + (Vector3.forward * z);

        private static readonly Config Config = Scp012.Singleton.Config;
    }
}
