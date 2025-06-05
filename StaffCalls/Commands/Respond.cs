using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using UnityEngine;
using Rocket.Unturned.Chat;

namespace StaffCalls.Commands
{
    internal class Respond : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "respond";

        public string Help => "Respond to an active call.";

        public string Syntax => "/respond";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "StaffCalls.StaffMember" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;

            if (StaffCalls.Instance.LastCall == null)
            {
                UnturnedChat.Say(caller, StaffCalls.Instance.Translate("Staff_CallNone"), Color.green);
            }

            switch (uPlayer.Stance)
            {
                case SDG.Unturned.EPlayerStance.DRIVING:
                    UnturnedChat.Say(caller, StaffCalls.Instance.Translate("Staff_CantTeleport"), Color.yellow);
                    return;

                case SDG.Unturned.EPlayerStance.SITTING:
                    UnturnedChat.Say(caller, StaffCalls.Instance.Translate("Staff_CantTeleport"), Color.yellow);
                    return;
            }

            uPlayer.Teleport(StaffCalls.Instance.LastCall);
        }
    }
}
