using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;

namespace StaffCalls
{
    public class StaffCallsConfig : IRocketPluginConfiguration
    {
        public uint NotificationLength;

        public uint StaffCallCooldown;

        public bool UseUI;

        public ushort SoundEffectID;

        public void LoadDefaults()
        {
            NotificationLength = 15;

            StaffCallCooldown = 300;

            UseUI = true;

            SoundEffectID = 32842;
        }
    }
}
