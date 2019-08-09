using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RuntimeUnityEditor.Core;

using VRCModLoader;

using UnityEngine;

using Harmony;

using VRCTestingKit.VRChat;
using VRCTestingKit.Hooks;

namespace VRCTestingKit
{
    [VRCModInfo("VRCTestingKit", "0.1.0", "AtiLion")]
    internal class VRCTestingKitMod : VRCMod
    {
        #region VRCTestingKit Properties
        public static GameObject ScriptHandler { get; private set; }
        #endregion

        #region Editor Properties
        public static RuntimeUnityEditorCore RuntimeEditor { get; private set; }
        #endregion

        #region GUI Properties
        public static TestingGUI GUI { get; private set; }
        #endregion

        #region VRCMod Functions
        void OnApplicationStart()
        {
            LoggerKit.Log("Starting VRCTestingKit...");

            // Setup Scripts
            ScriptHandler = new GameObject("TestingKitScriptHandler");
            GameObject.DontDestroyOnLoad(ScriptHandler);

            // Setup RuntimeUnityEditor
            RuntimeEditor = new RuntimeUnityEditorCore(ScriptHandler, new RUELogger());
            RuntimeEditor.ShowHotkey = KeyCode.None;

            // Setup GUI
            GUI = ScriptHandler.AddComponent<TestingGUI>();

            // Setup harmony
            HarmonyInstance hiVRCApplicationSetup_IsEditor = HarmonyInstance.Create("vrctestingkit.VRCApplicationSetup.IsEditor");

            // Patch harmony
            hiVRCApplicationSetup_IsEditor.Patch(
                    typeof(VRCApplicationSetup).GetMethod("IsEditor", BindingFlags.Public | BindingFlags.Static),
                    new HarmonyMethod(typeof(VRCTestingKitMod).GetMethod("_VRCApplicationSetup_IsEditor", BindingFlags.NonPublic | BindingFlags.Static)));

            // Setup loggers
            ModManager.StartCoroutine(Hook_NetworkManager());

            LoggerKit.Log("VRCTestingKit has been started!");
        }

        void OnLevelWasLoaded(int level)
        {
            if (level == 0 || level == 1)
            {
                GUI._locked = true;
                return;
            }
            else
            {
                GUI._locked = false;
            }
            KitServer._Players.Clear();
        }

        void OnUpdate()
        {
            RuntimeEditor.Update();
        }
        void OnGUI()
        {
            RuntimeEditor.OnGUI();
        }
        #endregion

        #region Initializers
        private IEnumerator Hook_NetworkManager()
        {
            yield return NetworkManager_Hook.WaitForNetworkManager();

            LoggerKit.Log("Hooking logger to NetworkManager...");
            NetworkManager_Hook.OnConnectedEvent(NetworkManager_Hook.LOG_OnConnectedEvent);
            NetworkManager_Hook.OnConnectedToMasterEvent(NetworkManager_Hook.LOG_OnConnectedToMasterEvent);
            NetworkManager_Hook.OnCreatedRoomEvent(NetworkManager_Hook.LOG_OnCreatedRoomEvent);
            NetworkManager_Hook.OnJoinedLobbyEvent(NetworkManager_Hook.LOG_OnJoinedLobbyEvent);
            NetworkManager_Hook.OnJoinedRoomEvent(NetworkManager_Hook.LOG_OnJoinedRoomEvent);
            NetworkManager_Hook.OnLeftLobbyEvent(NetworkManager_Hook.LOG_OnLeftLobbyEvent);
            NetworkManager_Hook.OnLeftRoomEvent(NetworkManager_Hook.LOG_OnLeftRoomEvent);
            NetworkManager_Hook.OnMasterClientSwitchedEvent(NetworkManager_Hook.LOG_OnMasterClientSwitchedEvent);
            NetworkManager_Hook.OnPlayerJoinedEvent.Add(NetworkManager_Hook.LOG_OnPlayerJoinedEvent);
            NetworkManager_Hook.OnPlayerLeftEvent.Add(NetworkManager_Hook.LOG_OnPlayerLeftEvent);
            LoggerKit.Log("Logger hooked to NetworkManager!");

            LoggerKit.Log("Hooking server to NetworkManager...");
            NetworkManager_Hook.OnPlayerJoinedEvent.Add(KitServer.PlayerJoined);
            NetworkManager_Hook.OnPlayerLeftEvent.Add(KitServer.PlayerLeft);
            LoggerKit.Log("Server hooked to NetworkManager!");
        }
        #endregion

        #region Harmony Patches
        private static bool _VRCApplicationSetup_IsEditor(ref bool __result)
        {
            __result = GUI.ShowMouse;
            return false;
        }
        #endregion
    }
}
