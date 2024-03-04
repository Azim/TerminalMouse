using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace TerminalMouse
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class TerminalMousePlugin : BaseUnityPlugin
    {

        public static ManualLogSource mls;

        CursorLockMode locked;
        bool wasVisible = false;

        private void Awake()
        {
            // Plugin startup logic
            mls = base.Logger;


            On.Terminal.BeginUsingTerminal += Terminal_BeginUsingTerminal;
            On.Terminal.QuitTerminal += Terminal_QuitTerminal;
            On.GameNetcodeStuff.PlayerControllerB.KillPlayer += PlayerControllerB_KillPlayer;

            mls.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }



        private void Terminal_BeginUsingTerminal(On.Terminal.orig_BeginUsingTerminal orig, Terminal self)
        {
            orig(self);
            locked = Cursor.lockState;
            wasVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        private void Terminal_QuitTerminal(On.Terminal.orig_QuitTerminal orig, Terminal self)
        {
            orig(self);
            Cursor.lockState = locked;
            Cursor.visible = wasVisible;
        }
        private void PlayerControllerB_KillPlayer(On.GameNetcodeStuff.PlayerControllerB.orig_KillPlayer orig, GameNetcodeStuff.PlayerControllerB self, Vector3 bodyVelocity, bool spawnBody, CauseOfDeath causeOfDeath, int deathAnimation)
        {
            orig(self, bodyVelocity, spawnBody, causeOfDeath, deathAnimation);
            if (self.IsLocalPlayer)
            {
                Cursor.lockState = locked;
                Cursor.visible = wasVisible;
            }
        }
    }
}