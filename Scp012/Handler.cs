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

    public partial class Handler
    {
        public static readonly Config Config = Scp012.Singleton.Config;

        public List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

        public List<Player> PlayersInteracting = new List<Player>();

        private DoorVariant Scp012BottomDoor;
        private Vector3 doorPos;

        private Pickup Scp012Item;
        private Vector3 itemSpawnPos;
        private Vector3 itemRotation;

        private Vector3 baitItemSpawnPos;

        private bool scp012death = false;

        private readonly System.Random rng = new System.Random();

        private readonly Dictionary<RoleType, string> RoleToString = new Dictionary<RoleType, string>
        {
            {RoleType.Scp049, "SCP 0 4 9" },
            {RoleType.Scp096, "SCP 0 9 6" },
            {RoleType.Scp106, "SCP 1 0 6" },
            {RoleType.Scp173, "SCP 1 7 3" },
            {RoleType.Scp93953, "SCP 9 3 9" },
            {RoleType.Scp93989, "SCP 9 3 9" }
        };

        internal void OnWaitingForPlayers()
        {
            Scp012BottomDoor = Map.GetDoorByName("012_BOTTOM");
            doorPos = Scp012BottomDoor.transform.position;

            Log.Debug($"Door rotation: {Scp012BottomDoor.transform.rotation}", Config.Debug);
            Log.Debug($"Door position: {doorPos}", Config.Debug);

            switch (Scp012BottomDoor.transform.rotation.ToString())
            {
                case "(0.0, 1.0, 0.0, 0.0)":
                    {
                        itemSpawnPos = new Vector3(doorPos.x - 2.05f, doorPos.y + 0.8f, doorPos.z - 8.5f);
                        itemRotation = new Vector3(-90f, -90f, 0f);

                        baitItemSpawnPos = new Vector3(doorPos.x - 2f, doorPos.y + 1f, doorPos.z - 6.5f);
                        break;
                    }

                case "(0.0, 0.0, 0.0, -1.0)":
                    {
                        itemSpawnPos = new Vector3(doorPos.x + 2.05f, doorPos.y + 0.8f, doorPos.z + 8.5f);
                        itemRotation = new Vector3(-90f, 90f, 0f);

                        baitItemSpawnPos = new Vector3(doorPos.x + 2f, doorPos.y + 1f, doorPos.z + 6.5f);
                        break;
                    }

                case "(0.0, 0.7, 0.0, -0.7)":
                    {
                        itemSpawnPos = new Vector3(doorPos.x - 8.5f, doorPos.y + 0.8f, doorPos.z + 2.05f);
                        itemRotation = new Vector3(-90f, 0f, 0);

                        baitItemSpawnPos = new Vector3(doorPos.x - 6.5f, doorPos.y + 1f, doorPos.z + 2f);
                        break;
                    }

                case "(0.0, 0.7, 0.0, 0.7)":
                    {
                        itemSpawnPos = new Vector3(doorPos.x + 8.5f, doorPos.y + 0.8f, doorPos.z - 2.05f);
                        itemRotation = new Vector3(-90f, 0f, 180f);

                        baitItemSpawnPos = new Vector3(doorPos.x + 6.5f, doorPos.y + 1f, doorPos.z - 2f);
                        break;
                    }
            }

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
            Log.Debug($"\nSCP died:\nReason: {ev.HitInfo.GetDamageName()}\nBool: {scp012death}", Config.Debug);

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
                            message += (" . . AllSecured . SCP 0 7 9 recontainment Sequence Commencing . Heavy containment zone over charge in t minus 1 minute");

                        Cassie.GlitchyMessage(message, 0.05f, 0.05f);
                    });
                }
            }
        }

        internal void SpawnScp012Item()
        {
            Scp012Item = Item.Spawn(ItemType.WeaponManagerTablet, 0, itemSpawnPos, Quaternion.Euler(itemRotation));

            Log.Debug($"Item pos: {itemSpawnPos}", Config.Debug);

            GameObject gameObject = Scp012Item.gameObject;

            gameObject.transform.localScale = new Vector3(0.5f, 2.5f, 2.5f);

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
                foreach (KeyValuePair<string, int> baitItem in Config.BaitItems[i])
                {
                    if (!Enum.TryParse(baitItem.Key, true, out ItemType item))
                    {
                        Log.Error($"\"{baitItem.Key}\" is not a valid ItemType name.");
                        continue;
                    }
                    else
                    {
                        float durability = 0;

                        if (Config.LoadedBaitWeapons)
                            durability = item.GetDefaultDurability();

                        int chance = baitItem.Value;

                        if (rng.Next(0, 101) < chance)
                        {
                            Item.Spawn(item, durability, new Vector3(baitItemSpawnPos.x + UnityEngine.Random.Range(-6.0f, 6.0f), baitItemSpawnPos.y, baitItemSpawnPos.z + UnityEngine.Random.Range(-6.0f, 6f)), RandomRotaion());

                            Log.Debug($"{item} bait item has been spawned inside of SCP-012 chamber!", Config.Debug);
                        }
                    }
                }
            }
        }

        private Quaternion RandomRotaion()
        {
            return Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f));
        }
    }
}
