namespace Scp012
{
    using System;
    using Exiled.API.Features;
    using System.Collections.Generic;
    using MEC;
    using UnityEngine;
    using Interactables.Interobjects.DoorUtils;
    using Mirror;

    using Object = UnityEngine.Object;
    using System.Linq;

    public partial class Handler
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
                        if (!ply.IsAlive || ply.IsGodModeEnabled || (Config.IgnoredRoles.Contains(ply.Role))) continue;

                        if (Scp012.IsGS)
                        {
                            if (IsGhost(ply))
                                continue;
                        }

                        float DistanceToScp012 = Vector3.Distance(Scp012Item.Networkposition, ply.Position);

                        if (DistanceToScp012 < Config.AffectDistance)
                        {
                            foreach (string EffectName in Config.AffectEffects)
                            {
                                ply.ReferenceHub.playerEffectsController.EnableByString(EffectName, 2f, false);
                            }
                        }

                        if (DistanceToScp012 < Config.NoReturnDistance)
                        {
                            foreach (string EffectName in Config.NoReturnEffects)
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
                    foreach (Player ply in Player.List)
                    {
                        if (!ply.IsAlive || ply.IsGodModeEnabled || Config.IgnoredRoles.Contains(ply.Role)) continue;

                        if (Scp012.IsGS)
                        {
                            if (IsGhost(ply))
                                continue;
                        }

                        if (Vector3.Distance(Scp012Item.Networkposition, ply.Position) < Config.AffectDistance && Vector3.Distance(Scp012Item.Networkposition, ply.Position) > Config.NoReturnDistance - 0.1f)
                            ply.Position = Vector3.MoveTowards(ply.Position, Scp012Item.Networkposition, Config.AttractionForce);
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
            bool blood = Config.SpawnBlood;

            ply.ShowHint(Config.Translations.IHaveTo);

            yield return Timing.WaitForSeconds(5f);

            if (Config.DropItems)
            {
                ply.DropItems();
            }

            foreach (string EffectName in Config.DyingEffects)
            {
                ply.ReferenceHub.playerEffectsController.EnableByString(EffectName, 15f, false);
            }

            if (rng.Next(0, 2) == 0) ply.ShowHint(Config.Translations.IDontThink);

            else ply.ShowHint(Config.Translations.IMust);

            if (blood) ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 1f);



            yield return Timing.WaitForSeconds(5f);

            if (rng.Next(0, 2) == 0) ply.ShowHint(Config.Translations.NoChoice);

            else ply.ShowHint(Config.Translations.NoSense);

            if (blood) ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 2f);



            yield return Timing.WaitForSeconds(5f);

            if (rng.Next(0, 2) == 0) ply.ShowHint(Config.Translations.IsImpossible);

            else ply.ShowHint(Config.Translations.CantBeCompleted);

            if (blood) ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 3f);



            yield return Timing.WaitForSeconds(5f);

            PlayersInteracting.Remove(ply);

            if (Vector3.Distance(Scp012Item.Networkposition, ply.Position) < 7.5f)
            {
                if (blood)
                    ply.ReferenceHub.characterClassManager.RpcPlaceBlood(ply.Position, 0, 5f);

                if (!Config.DropItems)
                    ply.ClearInventory();

                if (ply.IsScp)
                {
                    scp012death = true;
                    Log.Debug($"Bool is {scp012death} (before killing)", Config.Debug);
                }

                ply.Kill(DamageTypes.Bleeding);

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

        private bool IsGhost(Player player)
        {
            return GhostSpectator.API.IsGhost(player);
        }
    }
}
