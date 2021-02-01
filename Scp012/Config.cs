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

        [Description("Should SCP-012 affect other playable SCPs?")]
        public bool AllowScps { get; set; } = true;
    }
}
