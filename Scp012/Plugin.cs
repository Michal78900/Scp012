using System;
using Exiled.API.Features;

using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;
using MapEvent = Exiled.Events.Handlers.Map;
using Scp079Event = Exiled.Events.Handlers.Scp079;

namespace Scp012
{
    public class Scp012 : Plugin<Config>
    {
        public static Scp012 Singleton;

        public override string Author => "Michal78900";
        public override Version Version => new Version(2, 1, 0);
        public override Version RequiredExiledVersion => new Version(2, 3, 4);

        private Handler handler;

        public override void OnEnabled()
        {
            Singleton = this;

            handler = new Handler(this);

            ServerEvent.WaitingForPlayers += handler.OnWaitingForPlayers;
            ServerEvent.RoundStarted += handler.OnRoundStart;

            PlayerEvent.Destroying += handler.OnDestroy;
            Scp079Event.InteractingDoor += handler.OnDoor;
            PlayerEvent.PickingUpItem += handler.OnItemPickup;
            PlayerEvent.DroppingItem += handler.OnItemDrop;
            PlayerEvent.Hurting += handler.OnHurting;

            MapEvent.AnnouncingScpTermination += handler.OnAnnouncingScpTermination;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ServerEvent.WaitingForPlayers -= handler.OnWaitingForPlayers;
            ServerEvent.RoundStarted -= handler.OnRoundStart;

            PlayerEvent.Destroying -= handler.OnDestroy;
            Scp079Event.InteractingDoor -= handler.OnDoor;
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
