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
            {RoleType.Scp93953, "SCP 9 3 9 . 5 3" },
            {RoleType.Scp93989, "SCP 9 3 9 . 8 9" }
        };

        DoorVariant Scp012BottomDoor;

        Vector3 doorPos;

        Vector3 itemPos;

        public void OnWaitingForPlayers()
        {
            Scp012BottomDoor = Map.GetDoorByName("012_BOTTOM");

            Scp012BottomDoor.NetworkTargetState = true;
            Scp012BottomDoor.NetworkActiveLocks = 1;

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

            Log.Debug("Scp012Mechanic coroutines started!", plugin.Config.ShowDebugMessages);
        }

        public void VoiceLines(Player ply)
        {
            ply.ShowHint("I have to... I have to finish it.");

            Timing.CallDelayed(5f, () =>
            {
                ply.DropItems();
            });

            Timing.CallDelayed(5.1f, () =>
            {
                ply.ReferenceHub.playerEffectsController.EnableEffect<CustomPlayerEffects.Amnesia>(15f, false);

                if (rng.Next(0, 2) == 0) ply.ShowHint("I... I... must... do it.");

                else ply.ShowHint("I don't... think... I can do this.");

                ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 1f);
            });

            Timing.CallDelayed(10f, () =>
            {
                if (rng.Next(0, 2) == 0) ply.ShowHint("I-I... have... no... ch-choice!");

                else ply.ShowHint("This....this makes...no sense!");

                ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 2f);
            });

            Timing.CallDelayed(15f, () =>
            {
                if (rng.Next(0, 2) == 0) ply.ShowHint("No... this... this is... impossible!");

                else ply.ShowHint("It can't... It can't be completed!");

                ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 3f);
            });

            Timing.CallDelayed(20f, () =>
            {
                PlayersInteracting.Remove(ply);

                if (Vector3.Distance(itemPos, ply.Position) < 7.5f)
                {
                    ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 5f);

                    ply.Kill(DamageTypes.Bleeding);
                }
            });



            //I have to... I have to finish it...

            //I don't... think... I can do this.
            //I... I... must... do it.

            //I-I... have... no... ch-choice!
            //This....this makes...no sense!

            //No... this... this is... impossible!
            //t can't... It can't be completed!

        }

        public void OnItemPickup(PickingUpItemEventArgs ev)
        {
            if (Vector3.Distance(itemPos, ev.Pickup.transform.position) < 7.5f)
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

        public void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            if (ev.HitInfo.GetDamageType() == DamageTypes.Bleeding)
            {
                ev.IsAllowed = false;

                Cassie.GlitchyMessage($"{RoleToString[ev.Role.roleId]} terminated by SCP 0 1 2", 0.05f, 0.05f);
            }
        }

        public void SpawnItem()
        {
            /*
            
            */

            foreach (Room room in Map.Rooms)
            {
                if (room.Type == Exiled.API.Enums.RoomType.Lcz012) itemPos = room.Position;
            }
            itemPos = new Vector3(itemPos.x, itemPos.y - 7f, itemPos.z);

            switch (Scp012BottomDoor.transform.rotation.ToString())
            {
                case "(0.0, 1.0, 0.0, 0.0)": itemPos = new Vector3(doorPos.x - 1.5f, doorPos.y + 1f, doorPos.z - 8f); break;
                case "(0.0, 0.0, 0.0, -1.0)": itemPos = new Vector3(doorPos.x + 1.5f, doorPos.y + 1f, doorPos.z + 8f); ; break;
                case "(0.0, 0.7, 0.0, -0.7)": itemPos = new Vector3(doorPos.x - 8f, doorPos.y + 1f, doorPos.z + 1.5f); break; //
                case "(0.0, 0.7, 0.0, 0.7)": itemPos = new Vector3(doorPos.x + 8f, doorPos.y + 1f, doorPos.z - 1.5f); break; //OK

                default: Exiled.API.Extensions.Item.Spawn(ItemType.Coin, 1, new Vector3(doorPos.x, doorPos.y + 5f, doorPos.z)); break;
            }

            Pickup Item = Exiled.API.Extensions.Item.Spawn(ItemType.WeaponManagerTablet, 0, itemPos);
            Log.Debug($"Item pos: {itemPos}", plugin.Config.ShowDebugMessages);
            GameObject gameObject = Item.gameObject;
            gameObject.transform.localScale = new Vector3(0.5f, 2f, 2f);
            NetworkServer.UnSpawn(gameObject);
            NetworkServer.Spawn(Item.gameObject);
        }
        //(0.0, 0.0, 0.0, -1.0) (118.5, -7.9, 168.4) 120, -7, 176 
        //(0.0, 0.7, 0.0, 0.7) (182.1, -7.9, 108.3) 190, -7 107


        private IEnumerator<float> ScpMechanic()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(1f);

                foreach (Player ply in Player.List)
                {
                    if (!ply.IsAlive) continue;

                    /*
                    switch (Scp012BottomDoor.transform.rotation.ToString())
                    {
                        case "(0.0, 1.0, 0.0, 0.0)": Exiled.API.Extensions.Item.Spawn(ItemType.Coin, 1, new Vector3(doorPos.x-1.5f, doorPos.y + 5f, doorPos.z - 8f)); break;
                        case "(0.0, 0.0, 0.0, -1.0)": Exiled.API.Extensions.Item.Spawn(ItemType.Coin, 1, new Vector3(doorPos.x + 1.5f, doorPos.y + 5f, doorPos.z + 8f)); ; break;
                        case "(0.0, 0.7, 0.0, -0.7)": Exiled.API.Extensions.Item.Spawn(ItemType.Coin, 1, new Vector3(doorPos.x - 8f, doorPos.y + 5f, doorPos.z + 1.5f)); break; //
                        case "(0.0, 0.7, 0.0, 0.7)": Exiled.API.Extensions.Item.Spawn(ItemType.Coin, 1, new Vector3(doorPos.x + 8f, doorPos.y + 5f, doorPos.z-1.5f)); break; //OK

                        default: Exiled.API.Extensions.Item.Spawn(ItemType.Coin, 1, new Vector3(doorPos.x, doorPos.y + 5f, doorPos.z)); break;
                    }
                    */

                    if (ply.Team == Team.SCP && !plugin.Config.AllowScps) continue;


                    if (Vector3.Distance(itemPos, ply.Position) < 7.5f)
                    {
                        ply.ReferenceHub.playerEffectsController.EnableEffect<CustomPlayerEffects.Disabled>(2f, false);
                    }

                    if (Vector3.Distance(itemPos, ply.Position) < 2.5f)
                    {
                        ply.ReferenceHub.playerEffectsController.EnableEffect<CustomPlayerEffects.Ensnared>(2f, false);

                        if (!PlayersInteracting.Contains(ply))
                        {
                            PlayersInteracting.Add(ply);

                            VoiceLines(ply);
                        }
                    }
                }

                if (PlayersInteracting.Any())
                {
                    Scp012BottomDoor.NetworkTargetState = false;
                    
                }
                else
                {
                    Scp012BottomDoor.NetworkTargetState = true;
                }
            }
        }

        private IEnumerator<float> MovePlayer()
        {
            while(Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(0.2f);

                foreach(Player ply in Player.List)
                {
                    if (Vector3.Distance(itemPos, ply.Position) < 7.5f && Vector3.Distance(itemPos, ply.Position) > 2.45f)
                        ply.Position = Vector3.MoveTowards(ply.Position, itemPos, 0.2f);
                }
            }
        }
    }
}
