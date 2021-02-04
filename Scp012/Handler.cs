using System;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using Interactables.Interobjects.DoorUtils;
using Mirror;


namespace Scp012
{
    class Handler
    {
        private readonly Scp012 plugin;
        public Handler(Scp012 plugin) => this.plugin = plugin;


        public System.Random rng = new System.Random();

        public List<CoroutineHandle> coroutines = new List<CoroutineHandle>();

        public List<Player> PlayersInteracting = new List<Player>();

        private readonly Dictionary<RoleType, string> RoleToString = new Dictionary<RoleType, string>
        {
            {RoleType.Scp049, "SCP 0 4 9" },
            {RoleType.Scp096, "SCP 0 9 6" },
            {RoleType.Scp106, "SCP 1 0 6" },
            {RoleType.Scp173, "SCP 1 7 3" },
            {RoleType.Scp93953, "SCP 9 3 9" },
            {RoleType.Scp93989, "SCP 9 3 9" }
        };

        DoorVariant Scp012BottomDoor;

        Vector3 doorPos;

        Vector3 itemSpawnPos;
        Quaternion itemRotaion;

        Pickup Scp012Item;


        public void OnWaitingForPlayers()
        {
            Scp012BottomDoor = Map.GetDoorByName("012_BOTTOM");


            doorPos = Scp012BottomDoor.transform.position;

            Log.Debug($"Door rotation: {Scp012BottomDoor.transform.rotation}", plugin.Config.ShowDebugMessages);
            Log.Debug($"Door position: {doorPos}", plugin.Config.ShowDebugMessages);

            SpawnItem();
        }

        public void OnRoundStart()
        {
            foreach (CoroutineHandle coroutine in coroutines)
            {
                Timing.KillCoroutines(coroutine);
            }
            coroutines.Clear();

            coroutines.Add(Timing.RunCoroutine(ScpMechanic()));
            coroutines.Add(Timing.RunCoroutine(MovePlayer()));

            Log.Debug("Scp012 coroutines started!", plugin.Config.ShowDebugMessages);
        }



        public void OnItemPickup(PickingUpItemEventArgs ev)
        {
            if (Vector3.Distance(Scp012Item.Networkposition, ev.Pickup.transform.position) < 7.5f)
            {
                if (ev.Pickup.ItemId == ItemType.WeaponManagerTablet)
                {
                    ev.IsAllowed = false;
                }
            }

            if (PlayersInteracting.Contains(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }

        public void OnItemDrop(DroppingItemEventArgs ev)
        {
            if (PlayersInteracting.Contains(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }

        public void OnHurting(HurtingEventArgs ev)
        {
            if (PlayersInteracting.Contains(ev.Target) && !plugin.Config.EffectsDamage)
            {
                var dmgType = ev.HitInformations.GetDamageType();

                if (dmgType == DamageTypes.Asphyxiation || dmgType == DamageTypes.Bleeding || dmgType == DamageTypes.Poison)
                    ev.Amount = 0f;
            }
        }


        public void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            if (ev.HitInfo.GetDamageType() == DamageTypes.Bleeding)
            {
                ev.IsAllowed = false;

                if (!string.IsNullOrEmpty(plugin.Config.CassieMessage))
                {
                    StringBuilder message = new StringBuilder();
                    message.Append(plugin.Config.CassieMessage);
                    message.Replace("{scp}", $"{RoleToString[ev.Role.roleId]}");

                    Cassie.GlitchyMessage(message.ToString(), 0.05f, 0.05f);
                }
            }
        }

        public void SpawnItem()
        {
            switch (Scp012BottomDoor.transform.rotation.ToString())
            {
                    //OK
                case "(0.0, 1.0, 0.0, 0.0)":
                    {
                        itemSpawnPos = new Vector3(doorPos.x - 2f, doorPos.y + 1f, doorPos.z - 8.5f);
                        itemRotaion = new Quaternion(0.1f, 0.1f, 0.1f, -0.1f);
                        break;
                    } 
                    //OK
                case "(0.0, 0.0, 0.0, -1.0)":
                    {
                        itemSpawnPos = new Vector3(doorPos.x + 2f, doorPos.y + 1f, doorPos.z + 8.5f);
                        itemRotaion = new Quaternion(-0.05f, 0.1f, 0.1f, 0.1f);
                        break;
                    }
                    //OK
                case "(0.0, 0.7, 0.0, -0.7)":
                    {
                        itemSpawnPos = new Vector3(doorPos.x - 8.5f, doorPos.y + 1f, doorPos.z + 2f);
                        itemRotaion = new Quaternion(0.1f, 0.0f, 0.0f, -0.1f);
                        break;
                    }
                    //OK
                case "(0.0, 0.7, 0.0, 0.7)":
                    {
                        itemSpawnPos = new Vector3(doorPos.x + 8.5f, doorPos.y + 1f, doorPos.z - 2f);
                        itemRotaion = new Quaternion(0.0f, 1.0f, 1.0f, 0.1f);
                        break;
                    }
            }

            Scp012Item = Exiled.API.Extensions.Item.Spawn(ItemType.WeaponManagerTablet, 0, itemSpawnPos, itemRotaion);

            Log.Debug($"Item pos: {itemSpawnPos}", plugin.Config.ShowDebugMessages);

            GameObject gameObject = Scp012Item.gameObject;

            gameObject.transform.localScale = new Vector3(0.5f, 2.5f, 2.5f);

            NetworkServer.UnSpawn(gameObject);
            NetworkServer.Spawn(gameObject);

            Log.Debug("SCP-012 item spawnned successfully!", plugin.Config.ShowDebugMessages);

            if (plugin.Config.BaitItemsNumber > 0 && plugin.Config.BaitItems.Count > 0)
            {
                for (int i = 0; i < plugin.Config.BaitItemsNumber; i++)
                {
                    ItemType choosenBaitItem = plugin.Config.BaitItems[rng.Next(0, plugin.Config.BaitItems.Count)];

                    float durability = 0;

                    if (plugin.Config.LoadedBaitWeapons)
                    {
                        switch (choosenBaitItem)
                        {
                            case ItemType.GunCOM15: durability = 12; break;
                            case ItemType.GunUSP: durability = 18; break;
                            case ItemType.GunMP7: durability = 35; break;
                            case ItemType.GunProject90: durability = 50; break;
                            case ItemType.GunE11SR: durability = 40; break;
                            case ItemType.GunLogicer: durability = 75; break;
                            case ItemType.MicroHID: durability = 1; break;

                            default: durability = 0; break;
                        }
                    }

                    Exiled.API.Extensions.Item.Spawn(choosenBaitItem, durability, new Vector3(itemSpawnPos.x + UnityEngine.Random.Range(-4.0f, 4.1f), itemSpawnPos.y, itemSpawnPos.z + UnityEngine.Random.Range(-4.0f, 4.1f)), new Quaternion(
                                                                                                                                                                                                                               UnityEngine.Random.Range(0.0f, 1.0f),
                                                                                                                                                                                                                               UnityEngine.Random.Range(0.0f, 1.0f),
                                                                                                                                                                                                                               UnityEngine.Random.Range(0.0f, 1.0f),
                                                                                                                                                                                                                               UnityEngine.Random.Range(0.0f, 1.0f)));
                }

                Log.Debug("Bait items have been successfully spawned inside of SCP-012 chamber!", plugin.Config.ShowDebugMessages);
            }
        }


        private IEnumerator<float> ScpMechanic()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(0.25f);

                foreach (Player ply in Player.List)
                {
                    if (!ply.IsAlive) continue;

                    if (ply.IsScp && !plugin.Config.AllowScps) continue;


                    if (Vector3.Distance(Scp012Item.Networkposition, ply.Position) < plugin.Config.AffectDistance)
                    {
                        foreach (string EffectName in plugin.Config.AffectEffects)
                        {
                            ply.ReferenceHub.playerEffectsController.EnableByString(EffectName, 2f, false);
                        }
                    }

                    if (Vector3.Distance(Scp012Item.Networkposition, ply.Position) < plugin.Config.NoReturnDistance)
                    {
                        foreach (string EffectName in plugin.Config.NoReturnEffects)
                        {
                            ply.ReferenceHub.playerEffectsController.EnableByString(EffectName, 2f, false);
                        }

                        if (!PlayersInteracting.Contains(ply))
                        {
                            PlayersInteracting.Add(ply);

                            VoiceLines(ply);
                        }
                    }
                }

                if (PlayersInteracting.Any() && (plugin.Config.AutoCloseDoor || plugin.Config.AutoLockDoor))
                {
                    if (plugin.Config.AutoCloseDoor) Scp012BottomDoor.NetworkTargetState = false;
                    if (plugin.Config.AutoLockDoor) Scp012BottomDoor.NetworkActiveLocks = 1;
                }
                else
                {
                    if (plugin.Config.AutoCloseDoor) Scp012BottomDoor.NetworkTargetState = true;
                    if (plugin.Config.AutoLockDoor) Scp012BottomDoor.NetworkActiveLocks = 0;
                }

                if(plugin.Config.AllowItemRepsawn && Vector3.Distance(itemSpawnPos, Scp012Item.Networkposition) > 3f)
                {
                    Scp012Item.Delete();

                    Scp012Item = Exiled.API.Extensions.Item.Spawn(ItemType.WeaponManagerTablet, 0, itemSpawnPos, itemRotaion);

                    GameObject gameObject = Scp012Item.gameObject;
                    gameObject.transform.localScale = new Vector3(0.5f, 2.5f, 2.5f);

                    NetworkServer.UnSpawn(gameObject);
                    NetworkServer.Spawn(gameObject);
                }
            }
        }

        private IEnumerator<float> MovePlayer()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(0.1f);

                foreach (Player ply in Player.List)
                {
                    if (!ply.IsAlive) continue;

                    if (ply.IsScp && !plugin.Config.AllowScps) continue;


                    if (Vector3.Distance(Scp012Item.Networkposition, ply.Position) < plugin.Config.AffectDistance && Vector3.Distance(Scp012Item.Networkposition, ply.Position) > plugin.Config.NoReturnDistance - 0.1f)
                        ply.Position = Vector3.MoveTowards(ply.Position, Scp012Item.Networkposition, plugin.Config.AttractionForce);
                }
            }
        }

        public void VoiceLines(Player ply)
        {
            bool blood = plugin.Config.SpawnBlood;

            ply.ShowHint(plugin.Config.IHaveTo);

            if (plugin.Config.DropItems)
            {
                Timing.CallDelayed(5f, () =>
                {
                    ply.DropItems();
                });
            }

            Timing.CallDelayed(5.1f, () =>
            {
                foreach (string EffectName in plugin.Config.DyingEffects)
                {
                    ply.ReferenceHub.playerEffectsController.EnableByString(EffectName, 15f, false);
                }

                if (rng.Next(0, 2) == 0) ply.ShowHint(plugin.Config.IDontThink);

                else ply.ShowHint(plugin.Config.IMust);

                if (blood) ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 1f);
            });

            Timing.CallDelayed(10f, () =>
            {
                if (rng.Next(0, 2) == 0) ply.ShowHint(plugin.Config.NoChoice);

                else ply.ShowHint(plugin.Config.NoSense);

                if (blood) ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 2f);
            });

            Timing.CallDelayed(15f, () =>
            {
                if (rng.Next(0, 2) == 0) ply.ShowHint(plugin.Config.IsImpossible);

                else ply.ShowHint(plugin.Config.CantBeCompleted);

                if (blood) ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 3f);
            });

            Timing.CallDelayed(20f, () =>
            {
                PlayersInteracting.Remove(ply);

                if (Vector3.Distance(Scp012Item.Networkposition, ply.Position) < 7.5f)
                {
                    if (blood) ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 5f);

                    ply.Kill(DamageTypes.Bleeding);
                }
            });



            //I have to... I have to finish it...

            //I don't... think... I can do this.
            //I... I... must... do it.

            //I-I... have... no... ch-choice!
            //This....this makes...no sense!

            //No... this... this is... impossible!
            //It can't... It can't be completed!

        }
    }
}
