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
    partial class Handler
    {
        private IEnumerator<float> ScpMechanic()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(0.25f);

                try
                {
                    foreach (Player ply in Player.List)
                    {
                        if (!ply.IsAlive || ply.IsGodModeEnabled) continue;

                        if (plugin.Config.IgnoredRoles.Contains(ply.Role)) continue;


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

                                Timing.RunCoroutine(VoiceLines(ply));
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

                    if (plugin.Config.AllowItemRespawn && Vector3.Distance(itemSpawnPos, Scp012Item.Networkposition) > 3f)
                    {
                        Scp012Item.Delete();

                        Scp012Item = Exiled.API.Extensions.Item.Spawn(ItemType.WeaponManagerTablet, 0, itemSpawnPos, itemRotaion);

                        GameObject gameObject = Scp012Item.gameObject;
                        gameObject.transform.localScale = new Vector3(0.5f, 2.5f, 2.5f);

                        NetworkServer.UnSpawn(gameObject);
                        NetworkServer.Spawn(gameObject);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        private IEnumerator<float> MovePlayer()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(0.1f);

                try
                {
                    foreach (Player ply in Player.List)
                    {
                        if (!ply.IsAlive || ply.IsGodModeEnabled) continue;

                        if (plugin.Config.IgnoredRoles.Contains(ply.Role)) continue;


                        if (Vector3.Distance(Scp012Item.Networkposition, ply.Position) < plugin.Config.AffectDistance && Vector3.Distance(Scp012Item.Networkposition, ply.Position) > plugin.Config.NoReturnDistance - 0.1f)
                            ply.Position = Vector3.MoveTowards(ply.Position, Scp012Item.Networkposition, plugin.Config.AttractionForce);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        private IEnumerator<float> VoiceLines(Player ply)
        {
            bool blood = plugin.Config.SpawnBlood;

            ply.ShowHint(plugin.Config.IHaveTo);


            yield return Timing.WaitForSeconds(5f);

            if (plugin.Config.DropItems)
            {
                ply.DropItems();
            }

            foreach (string EffectName in plugin.Config.DyingEffects)
            {
                ply.ReferenceHub.playerEffectsController.EnableByString(EffectName, 15f, false);
            }

            if (rng.Next(0, 2) == 0) ply.ShowHint(plugin.Config.IDontThink);

            else ply.ShowHint(plugin.Config.IMust);

            if (blood) ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 1f);



            yield return Timing.WaitForSeconds(5f);

            if (rng.Next(0, 2) == 0) ply.ShowHint(plugin.Config.NoChoice);

            else ply.ShowHint(plugin.Config.NoSense);

            if (blood) ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 2f);



            yield return Timing.WaitForSeconds(5f);

            if (rng.Next(0, 2) == 0) ply.ShowHint(plugin.Config.IsImpossible);

            else ply.ShowHint(plugin.Config.CantBeCompleted);

            if (blood) ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 3f);



            yield return Timing.WaitForSeconds(5f);

            PlayersInteracting.Remove(ply);

            if (Vector3.Distance(Scp012Item.Networkposition, ply.Position) < 7.5f)
            {
                if (blood) ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 5f);

                if (!plugin.Config.DropItems) ply.ClearInventory();

                ply.Kill(DamageTypes.Bleeding);
            }


            if (plugin.Config.RagdollCleanupDelay > 0)
            {
                yield return Timing.WaitForSeconds(5f);


                foreach (Ragdoll ragdoll in UnityEngine.Object.FindObjectsOfType<Ragdoll>())
                {
                    if (Vector3.Distance(ragdoll.transform.position, Scp012Item.transform.position) < 5f)
                    {
                        NetworkServer.Destroy(ragdoll.gameObject);
                    }
                }
            }
        }
    }
}
