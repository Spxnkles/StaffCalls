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
    internal class CallStaff : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "callstaff";

        public string Help => "Call a staff member to help you.";

        public string Syntax => "/callstaff";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "StaffCalls.Call" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if ((new RocketPlayer(player.Id)).HasPermission("StaffCalls.Blocked"))
            {
                UnturnedChat.Say(caller, StaffCalls.Instance.Translate("Caller_Blocked"), Color.red);
                return;
            }

            if (StaffCalls.Instance.Cooldowns.Contains(player))
            {
                    UnturnedChat.Say(caller, StaffCalls.Instance.Translate("Caller_Cooldown"), Color.yellow);
                    return;
            }

            StaffCalls.Instance.StaffCall(player);
            UnturnedChat.Say(caller, StaffCalls.Instance.Translate("Caller_CalledStaffMember"), Color.green);

            StaffCalls.Instance.Cooldown(player);
        }
    }
}
