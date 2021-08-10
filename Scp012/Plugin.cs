namespace Scp012
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Features;
    using Exiled.Loader;

    using PlayerEvent = Exiled.Events.Handlers.Player;
    using ServerEvent = Exiled.Events.Handlers.Server;
    using MapEvent = Exiled.Events.Handlers.Map;
    using Scp079Event = Exiled.Events.Handlers.Scp079;

    public class Scp012 : Plugin<Config>
    {
        public static Scp012 Singleton;
        public override string Author => "Michal78900";
        public override Version Version => new Version(2, 3, 0);
        public override Version RequiredExiledVersion => new Version(2, 10, 0);

        public static Assembly GhostSpectator;

        private Handler handler;

        public override void OnEnabled()
        {
            Singleton = this;

            handler = new Handler();

            ServerEvent.WaitingForPlayers += handler.OnWaitingForPlayers;
            ServerEvent.RoundStarted += handler.OnRoundStart;

            PlayerEvent.Destroying += handler.OnDestroy;
            Scp079Event.TriggeringDoor += handler.OnDoor;
            PlayerEvent.PickingUpItem += handler.OnItemPickup;
            PlayerEvent.DroppingItem += handler.OnItemDrop;
            PlayerEvent.Hurting += handler.OnHurting;

            MapEvent.AnnouncingScpTermination += handler.OnAnnouncingScpTermination;

            GhostSpectator = Loader.Plugins.FirstOrDefault(x => x.Name == "GhostSpectator")?.Assembly;

            if (GhostSpectator != null)
                Log.Debug("GhostSpectator plugin detected!", Config.Debug);

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ServerEvent.WaitingForPlayers -= handler.OnWaitingForPlayers;
            ServerEvent.RoundStarted -= handler.OnRoundStart;

            PlayerEvent.Destroying -= handler.OnDestroy;
            Scp079Event.TriggeringDoor -= handler.OnDoor;
            PlayerEvent.PickingUpItem -= handler.OnItemPickup;
            PlayerEvent.DroppingItem -= handler.OnItemDrop;
            PlayerEvent.Hurting -= handler.OnHurting;

            MapEvent.AnnouncingScpTermination -= handler.OnAnnouncingScpTermination;

            handler = null;
            Singleton = null;

            base.OnDisabled();
        }
    }
}
