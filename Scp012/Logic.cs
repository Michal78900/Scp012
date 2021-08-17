namespace Scp012
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Features;
    using System.Collections.Generic;
    using MEC;
    using UnityEngine;
    using Interactables.Interobjects.DoorUtils;
    using Mirror;

    using Random = UnityEngine.Random;
    using Object = UnityEngine.Object;
    using Exiled.API.Enums;

    public partial class Handler
    {
        private IEnumerator<float> ScpMechanic()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(0.25f);

                try
                {
                    foreach (Player player in Player.List)
                    {
                        if (!player.IsAlive || player.IsGodModeEnabled || (Config.IgnoredRoles.Contains(player.Role))) continue;

                        if (IsGhost(player))
                            continue;

                        float DistanceToScp012 = Vector3.Distance(Scp012Item.Networkposition, player.Position);

                        if (DistanceToScp012 < Config.AffectDistance)
                        {
                            foreach (EffectType effectType in Config.AffectEffects)
                            {
                                player.EnableEffect(effectType, 2f, false);
                            }
                        }

                        if (DistanceToScp012 < Config.NoReturnDistance)
                        {
                            foreach (EffectType effectType in Config.NoReturnEffects)
                            {
                                player.EnableEffect(effectType, 2f, false);
                            }

                            if (!PlayersInteracting.Contains(player))
                            {
                                PlayersInteracting.Add(player);

                                Timing.RunCoroutine(VoiceLines(player));
                            }
                        }
                    }

                    if (PlayersInteracting.Count > 0 && (Config.AutoCloseDoor || Config.AutoLockDoor))
                    {
                        if (Config.AutoLockDoor) Scp012BottomDoor.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                        if (Config.AutoCloseDoor) Scp012BottomDoor.NetworkTargetState = false;
                    }
                    else
                    {
                        if (Config.AutoLockDoor) Scp012BottomDoor.ServerChangeLock(DoorLockReason.SpecialDoorFeature, false);
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
                    foreach (Player player in Player.List)
                    {
                        if (!player.IsAlive || player.IsGodModeEnabled || Config.IgnoredRoles.Contains(player.Role)) continue;

                        if (IsGhost(player))
                            continue;

                        if (Vector3.Distance(Scp012Item.Networkposition, player.Position) < Config.AffectDistance && Vector3.Distance(Scp012Item.Networkposition, player.Position) > Config.NoReturnDistance - 0.1f)
                            player.Position = Vector3.MoveTowards(player.Position, Scp012Item.Networkposition, Config.AttractionForce);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        private IEnumerator<float> VoiceLines(Player player)
        {
            bool blood = Config.SpawnBlood;

            player.ShowHint(Translation.IHaveTo);

            yield return Timing.WaitForSeconds(5f);

            if (Config.DropItems)
            {
                player.DropItems();
            }

            foreach (EffectType effectType in Config.DyingEffects)
            {
                player.EnableEffect(effectType, 15f, false);
            }

            if (Random.Range(0, 2) == 0) player.ShowHint(Translation.IDontThink);

            else player.ShowHint(Translation.IMust);

            if (blood) player.ReferenceHub.characterClassManager.RpcPlaceBlood(player.Position, 0, 1f);



            yield return Timing.WaitForSeconds(5f);

            if (Random.Range(0, 2) == 0) player.ShowHint(Translation.NoChoice);

            else player.ShowHint(Translation.NoSense);

            if (blood) player.ReferenceHub.characterClassManager.RpcPlaceBlood(player.Position, 0, 2f);



            yield return Timing.WaitForSeconds(5f);

            if (Random.Range(0, 2) == 0) player.ShowHint(Translation.IsImpossible);

            else player.ShowHint(Translation.CantBeCompleted);

            if (blood) player.ReferenceHub.characterClassManager.RpcPlaceBlood(player.Position, 0, 3f);



            yield return Timing.WaitForSeconds(5f);

            PlayersInteracting.Remove(player);

            if (Vector3.Distance(Scp012Item.Networkposition, player.Position) < 7.5f)
            {
                if (blood)
                    player.ReferenceHub.characterClassManager.RpcPlaceBlood(player.Position, 0, 5f);

                if (!Config.DropItems)
                    player.ClearInventory();

                if (player.IsScp)
                {
                    scp012death = true;
                    Log.Debug($"Bool is {scp012death} (before killing)", Config.Debug);
                }

                player.Kill(DamageTypes.Bleeding);

                var scps = Player.Get(Team.SCP);
                if (scps.Count(scp => scp.Role == RoleType.Scp079) > 0 && scps.Count() == 1)
                {
                    Recontainer079.BeginContainment(true);
                }

                scp012death = false;
                Log.Debug($"Bool is {scp012death} (after killing)", Config.Debug);
            }


            if (Config.RagdollCleanupDelay > 0)
            {
                yield return Timing.WaitForSeconds(Config.RagdollCleanupDelay);

                foreach (Ragdoll ragdoll in Object.FindObjectsOfType<Ragdoll>())
                {
                    if (Vector3.Distance(ragdoll.transform.position, Scp012Item.transform.position) < 5f)
                    {
                        NetworkServer.Destroy(ragdoll.gameObject);
                    }
                }
            }
        }

        // Original code from the Scanner plugin made by Thunder
        private bool IsGhost(Player player)
        {
            Assembly assembly = Scp012.GhostSpectator;
            if (assembly == null)
                return false;

            return ((bool)assembly.GetType("GhostSpectator.API")?.GetMethod("IsGhost")?.Invoke(null, new object[] { player })) == true;
        }
    }
}
