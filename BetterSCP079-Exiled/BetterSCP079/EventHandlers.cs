﻿using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Grenades;
using MEC;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BetterSCP079
{
    public class EventHandlers
    {
        public int CooldownNukeOff = Plugin.Instance.Config.canceled_cooldown;
        public int CooldownNukeOn = Plugin.Instance.Config.activate_cooldown;
        public int CooldownLights = Plugin.Instance.Config.blackout_cooldown;
        public int CooldownFlash = Plugin.Instance.Config.flash_cooldown;

        public bool isCooldownNukeOff;
        public bool isCooldownNukeOn;
        public bool isCooldownLights;
        public bool isCooldownFlash;

        internal void PlayerSpawn(SpawningEventArgs ev)
        {
            if (ev.Player.Role == RoleType.Scp079)
            {
                ev.Player.ShowHint(Plugin.Instance.Config.scp_startmsg, Plugin.Instance.Config.scp_timestartmsg);
            }
        }

        public IEnumerator<float> NukeOff()
        {
            Warhead.IsLocked = false;
            Warhead.Stop();
            Cassie.Message(Plugin.Instance.Config.canceled_cassie);

            while (CooldownNukeOff > 0)
            {
                CooldownNukeOff--;
                isCooldownNukeOff = true;

                yield return Timing.WaitForSeconds(1f);
            }

            isCooldownNukeOff = false;
            CooldownNukeOff = Plugin.Instance.Config.canceled_cooldown;
        }

        public IEnumerator<float> Flash(Player plr)
        {

            var pos = plr.ReferenceHub.scp079PlayerScript.currentCamera.transform.position;
            GrenadeManager gm = plr.ReferenceHub.GetComponent<GrenadeManager>();
            GrenadeSettings settings = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.GrenadeFlash);
            FlashGrenade flash = GameObject.Instantiate(settings.grenadeInstance).GetComponent<FlashGrenade>();
            flash.fuseDuration = 0.5f;
            flash.InitData(gm, Vector3.zero, Vector3.zero, 1f);
            flash.transform.position = pos;
            NetworkServer.Spawn(flash.gameObject);

            while (CooldownFlash > 0)
            {
                CooldownFlash--;
                isCooldownFlash = true;

                yield return Timing.WaitForSeconds(1f);
            }

            isCooldownFlash = false;
            CooldownFlash = Plugin.Instance.Config.flash_cooldown;
    }

        public IEnumerator<float> NukeOn()
        {
            if (Plugin.Instance.Config.activate_alphalock == true)
            {
                Warhead.IsLocked = true;
            }
            Warhead.Start();
            Cassie.Message(Plugin.Instance.Config.activate_cassie);

            while (CooldownNukeOn > 0)
            {
                CooldownNukeOn--;
                isCooldownNukeOn = true;

                yield return Timing.WaitForSeconds(1f);
            }

            isCooldownNukeOn = false;
            CooldownNukeOn = Plugin.Instance.Config.activate_cooldown;
        }

        public IEnumerator<float> LightOff()
        {
            foreach (Player D in Player.List.Where(p => p.Role == RoleType.Scp079))
            {
                if (Plugin.Instance.Config.blackout_lvlup == true)
                {
                    if (Plugin.Instance.Config.blackout_lvl < D.ReferenceHub.scp079PlayerScript.NetworkcurLvl)
                    {
                        Plugin.Instance.Config.blackout_timeovercharge += Plugin.Instance.Config.blackout_timeinc;
                    }
                }
            }
            Generator079.Generators[0].ServerOvercharge(Plugin.Instance.Config.blackout_timeovercharge, false);
            Cassie.Message(Plugin.Instance.Config.blackout_cassie, true, true);

            while (CooldownLights > 0)
            {
                CooldownLights--;
                isCooldownLights = true;

                yield return Timing.WaitForSeconds(1f);
            }

            isCooldownLights = false;
            CooldownLights = Plugin.Instance.Config.blackout_cooldown;
        }
    }
}
