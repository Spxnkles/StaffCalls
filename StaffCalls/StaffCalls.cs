using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Core.Plugins;
using Rocket.Core.Logging;
using Rocket.Core;
using Rocket.API.Collections;
using Rocket.Unturned.Player;
using Rocket.Unturned.Events;
using SDG.Unturned;
using Rocket.Unturned;
using Rocket.API;
using Rocket.Unturned.Chat;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace StaffCalls
{
    public class StaffCalls : RocketPlugin<StaffCallsConfig>
    {
        public string Version = "1.0.3";
        public static StaffCalls Instance;
        public static StaffCallsConfig Config;

        public List<UnturnedPlayer> Cooldowns;

        protected override void Load()
        {
            Instance = this;
            Config = Instance.Configuration.Instance;

            Cooldowns = new List<UnturnedPlayer>();

            EffectManager.onEffectButtonClicked += ButtonClicked;

            Logger.Log("=======================================");
            Logger.Log("StaffCalls has been loaded.");
            Logger.Log($"Version: {Version}");
            Logger.Log("Created by Spinkles.");
            Logger.Log("=======================================");
        }

        protected override void Unload()
        {
            EffectManager.onEffectButtonClicked -= ButtonClicked;

            Logger.Log("=======================================");
            Logger.Log("StaffCalls has been unloaded.");
            Logger.Log($"Version: {Version}");
            Logger.Log("Created by Spinkles.");
            Logger.Log("=======================================");
        }

        public UnturnedPlayer LastCall;

        public void StaffCall(UnturnedPlayer player)
        {
            LastCall = player;

            if (Config.UseUI)
            {
                SendUINotifications();
            }
            else
            {
                SendChatNotifications();
            }
        }

        public void SendUINotifications()
        {
            var StaffMembers = GetStaffMembers();

            foreach (var StaffMember in StaffMembers)
            {
                UINotification(StaffMember);
            }
        }

        public async void UINotification(UnturnedPlayer StaffMember)
        {
            UnturnedPlayer callPlayer = LastCall;

            EffectManager.sendUIEffect(32840, 32, StaffMember.CSteamID, true, Translate("UI_Notification_TopText", callPlayer.CharacterName), Translate("UI_Notification_BottomText"));
            EffectManager.sendUIEffect(32842, 33, StaffMember.CSteamID, true);

            await Task.Delay(TimeSpan.FromSeconds(Config.NotificationLength));

            EffectManager.sendUIEffect(32841, 32, StaffMember.CSteamID, true, Translate("UI_Notification_TopText", callPlayer.CharacterName), Translate("UI_Notification_BottomText"));
        }

        public void SendChatNotifications()
        {
            var StaffMembers = GetStaffMembers();

            foreach (var StaffMember in StaffMembers)
            {
                ChatNotification(StaffMember);
            }
        }

        public void ChatNotification(UnturnedPlayer StaffMember)
        {
            UnturnedChat.Say(StaffMember, Translate("Staff_ReceiveCall", LastCall.CharacterName), Color.yellow);
        }

        public List<UnturnedPlayer> GetStaffMembers()
        {
            return Provider.clients.Select(UnturnedPlayer.FromSteamPlayer).Where(player => player.HasPermission("StaffCalls.StaffMember")).ToList();
        }

        public void ButtonClicked(Player player, string buttonName)
        {
            if (buttonName == "StaffCallButton")
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);

                if (StaffCalls.Instance.LastCall == null)
                {
                    UnturnedChat.Say(uPlayer, StaffCalls.Instance.Translate("Staff_CallNone"), Color.green);
                }

                switch (uPlayer.Stance)
                {
                    case SDG.Unturned.EPlayerStance.DRIVING:
                        UnturnedChat.Say(uPlayer, StaffCalls.Instance.Translate("Staff_CantTeleport"), Color.yellow);
                        return;

                    case SDG.Unturned.EPlayerStance.SITTING:
                        UnturnedChat.Say(uPlayer, StaffCalls.Instance.Translate("Staff_CantTeleport"), Color.yellow);
                        return;
                }

                uPlayer.Teleport(StaffCalls.Instance.LastCall);
            }
        }

        public async void  Cooldown(UnturnedPlayer player)
        {
            Cooldowns.Add(player);

            await Task.Delay(TimeSpan.FromSeconds(Configuration.Instance.StaffCallCooldown));

            Cooldowns.Remove(player);
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {"UI_Notification_TopText", "{0} needs help!"},
            {"UI_Notification_BottomText", "Click to teleport"},
            {"Caller_Cooldown", "Cooldown active. Try again later."},
            {"Caller_CalledStaffMember", "Staff call has been sent."},
            {"Staff_ReceiveCall", "{0} needs help! Do /respond to respon to their call."},
            {"Staff_CallNone", "There are no active help calls."},
            {"Staff_CantTeleport", "Couldn't teleport, please exit any vehicles."},
            {"Caller_Blocked", "You are blocked from calling staff."}
        };


    }
}
