using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace Scp012
{
    public class Config : IConfig
    {
        [Description("Should plugin be enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should debug messages be shown?")]
        public bool ShowDebugMessages { get; set; } = false;

        
        [Description("How strongly the SCP-012 will attract plyaers to itself?")]
        public float AttractionForce { get; set; } = 0.1f;

        [Description("If distance between a player and SCP-012 is less then this number, the Bad Compostion will start attracting a player:")]
        public float AffectDistance { get; set; } = 7.5f;

        [Description("List of effects given to player, when ther are in AffectDistance to SCP-012:")]
        public List<string> AffectEffects { get; set; } = new List<string>()
        {
            "Disabled",
        };

        [Description("If distance between a player and SCP-012 is less than this number, the Bad Composition will start killing affected player:")]
        public float NoReturnDistance { get; set; } = 2.5f;

        [Description("List of effects given to player when they are in NoReturnDistance to SCP-012:")]
        public List<string> NoReturnEffects { get; set; } = new List<string>()
        {
            "Ensnared",
        };

        [Description("List of effects given to player, when they begin to die because of SCP-012:")]
        public List<string> DyingEffects { get; set; } = new List<string>()
        {
            "Amnesia",
        };

        [Description("Should SCP-012 affect other playable SCPs?")]
        public bool AllowScps { get; set; } = true;

        [Description("SCP termination cassie message: (leave empty to disable)")]
        public string CassieMessage { get; set; } = "{scp} terminated by SCP 0 1 2";

        [Description("Should players drop their items, while interacting with SCP-012 (if set to false, after the players dies, they won't drop any items, so they are technically destroyed)")]
        public bool DropItems { get; set; } = true;

        [Description("Should 012_BOTTOM door close, when someone interacts with SCP-012?")]
        public bool AutoCloseDoor { get; set; } = true;
        [Description("Should 012_BOTTOM door lock, when someone interacts with SCP-012?")]
        public bool AutoLockDoor { get; set; } = true;

        [Description("Texts shown to player that is interacting with SCP-012. Default lines are taken from SCP:CB wiki:")]
        public string IHaveTo { get; set; } = "I have to... I have to finish it.";
        public string IDontThink { get; set; } = "I don't... think... I can do this.";
        public string IMust { get; set; } = "I... I... must... do it.";
        public string NoChoice { get; set; } = "I-I... have... no... ch-choice!";
        public string NoSense { get; set; } = "This....this makes...no sense!";
        public string IsImpossible { get; set; } = "No... this... this is... impossible!";
        public string CantBeCompleted { get; set; } = "It can't... It can't be completed!";
    }
}
