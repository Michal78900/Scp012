using System;
using Exiled.API.Enums;
using Exiled.API.Features;

using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;
using MapEvent = Exiled.Events.Handlers.Map;

namespace Scp012
{
    public class Scp012 : Plugin<Config>
    {
        public static Scp012 Singleton;

        public override string Author => "Michal78900";
        public override Version Version => new Version(2, 0, 0);
        public override Version RequiredExiledVersion => new Version(2, 1, 30);


        private Handler handler;

        public override void OnEnabled()
        {
            Singleton = this;

            handler = new Handler(this);

            ServerEvent.WaitingForPlayers += handler.OnWaitingForPlayers;
            ServerEvent.RoundStarted += handler.OnRoundStart;

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
