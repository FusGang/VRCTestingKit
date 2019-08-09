using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC;

using VRCTestingKit.VRChat;

namespace VRCTestingKit.Hooks
{
    public static class NetworkManager_Hook
    {
        #region Reflection Variables
        private static Type _type;
        private static FieldInfo _instance;
        private static Type _unityActionInternalType;

        private static MethodInfo _add_OnLeftLobbyEvent;
        private static MethodInfo _add_OnJoinedLobbyEvent;
        private static MethodInfo _add_OnConnectedToMasterEvent;
        private static MethodInfo _add_OnLeftRoomEvent;
        private static MethodInfo _add_OnCreatedRoomEvent;
        private static MethodInfo _add_OnMasterClientSwitchedEvent;
        private static MethodInfo _add_OnJoinedRoomEvent;
        private static MethodInfo _add_OnConnectedEvent;

        private static UnityActionInternal<Player> _onPlayerJoinedEvent;
        private static UnityActionInternal<Player> _onPlayerLeftEvent;
        #endregion

        #region Reflection Properties
        public static Type Type
        {
            get
            {
                if(_type == null)
                    _type = typeof(PlayerModManager).Assembly.GetType("NetworkManager");
                return _type;
            }
        }
        public static FieldInfo Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Type.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
                return _instance;
            }
        }

        public static MethodInfo add_OnLeftLobbyEvent
        {
            get
            {
                if (_add_OnLeftLobbyEvent == null)
                    _add_OnLeftLobbyEvent = Type.GetMethod("add_OnLeftLobbyEvent", BindingFlags.Public | BindingFlags.Instance);
                return _add_OnLeftLobbyEvent;
            }
        }
        public static MethodInfo add_OnJoinedLobbyEvent
        {
            get
            {
                if (_add_OnJoinedLobbyEvent == null)
                    _add_OnJoinedLobbyEvent = Type.GetMethod("add_OnJoinedLobbyEvent", BindingFlags.Public | BindingFlags.Instance);
                return _add_OnJoinedLobbyEvent;
            }
        }
        public static MethodInfo add_OnConnectedToMasterEvent
        {
            get
            {
                if (_add_OnConnectedToMasterEvent == null)
                    _add_OnConnectedToMasterEvent = Type.GetMethod("add_OnConnectedToMasterEvent", BindingFlags.Public | BindingFlags.Instance);
                return _add_OnConnectedToMasterEvent;
            }
        }
        public static MethodInfo add_OnLeftRoomEvent
        {
            get
            {
                if (_add_OnLeftRoomEvent == null)
                    _add_OnLeftRoomEvent = Type.GetMethod("add_OnLeftRoomEvent", BindingFlags.Public | BindingFlags.Instance);
                return _add_OnLeftRoomEvent;
            }
        }
        public static MethodInfo add_OnCreatedRoomEvent
        {
            get
            {
                if (_add_OnCreatedRoomEvent == null)
                    _add_OnCreatedRoomEvent = Type.GetMethod("add_OnCreatedRoomEvent", BindingFlags.Public | BindingFlags.Instance);
                return _add_OnCreatedRoomEvent;
            }
        }
        public static MethodInfo add_OnMasterClientSwitchedEvent
        {
            get
            {
                if (_add_OnMasterClientSwitchedEvent == null)
                    _add_OnMasterClientSwitchedEvent = Type.GetMethod("add_OnMasterClientSwitchedEvent", BindingFlags.Public | BindingFlags.Instance);
                return _add_OnMasterClientSwitchedEvent;
            }
        }
        public static MethodInfo add_OnJoinedRoomEvent
        {
            get
            {
                if (_add_OnJoinedRoomEvent == null)
                    _add_OnJoinedRoomEvent = Type.GetMethod("add_OnJoinedRoomEvent", BindingFlags.Public | BindingFlags.Instance);
                return _add_OnJoinedRoomEvent;
            }
        }
        public static MethodInfo add_OnConnectedEvent
        {
            get
            {
                if (_add_OnConnectedEvent == null)
                    _add_OnConnectedEvent = Type.GetMethod("add_OnConnectedEvent", BindingFlags.Public | BindingFlags.Instance);
                return _add_OnConnectedEvent;
            }
        }

        public static UnityActionInternal<Player> OnPlayerJoinedEvent
        {
            get
            {
                if(_onPlayerJoinedEvent == null)
                {
                    FieldInfo onPlayerJoinedEvent = Type.GetField("OnPlayerJoinedEvent", BindingFlags.Public | BindingFlags.Instance);

                    if (_unityActionInternalType == null)
                        _unityActionInternalType = onPlayerJoinedEvent.FieldType;
                    _onPlayerJoinedEvent = new UnityActionInternal<Player>(_unityActionInternalType, onPlayerJoinedEvent.GetValue(GetInstance()));
                }
                return _onPlayerJoinedEvent;
            }
        }
        public static UnityActionInternal<Player> OnPlayerLeftEvent
        {
            get
            {
                if (_onPlayerLeftEvent == null)
                {
                    FieldInfo onPlayerLeftEvent = Type.GetField("OnPlayerLeftEvent", BindingFlags.Public | BindingFlags.Instance);

                    if (_unityActionInternalType == null)
                        _unityActionInternalType = onPlayerLeftEvent.FieldType;
                    _onPlayerLeftEvent = new UnityActionInternal<Player>(_unityActionInternalType, onPlayerLeftEvent.GetValue(GetInstance()));
                }
                return _onPlayerLeftEvent;
            }
        }
        #endregion

        #region NetworkManager Functions
        public static object GetInstance() =>
            Instance.GetValue(null);

        public static void OnLeftLobbyEvent(Action action) =>
            add_OnLeftLobbyEvent.Invoke(GetInstance(), new object[] { action });
        public static void OnJoinedLobbyEvent(Action action) =>
            add_OnJoinedLobbyEvent.Invoke(GetInstance(), new object[] { action });
        public static void OnConnectedToMasterEvent(Action action) =>
            add_OnConnectedToMasterEvent.Invoke(GetInstance(), new object[] { action });
        public static void OnLeftRoomEvent(Action action) =>
            add_OnLeftRoomEvent.Invoke(GetInstance(), new object[] { action });
        public static void OnCreatedRoomEvent(Action action) =>
            add_OnCreatedRoomEvent.Invoke(GetInstance(), new object[] { action });
        public static void OnMasterClientSwitchedEvent(Action<Player> action) =>
            add_OnMasterClientSwitchedEvent.Invoke(GetInstance(), new object[] { action });
        public static void OnJoinedRoomEvent(Action action) =>
            add_OnJoinedRoomEvent.Invoke(GetInstance(), new object[] { action });
        public static void OnConnectedEvent(Action action) =>
            add_OnConnectedEvent.Invoke(GetInstance(), new object[] { action });
        #endregion

        #region NetworkManager Coroutines
        public static IEnumerator WaitForNetworkManager()
        {
            LoggerKit.Log("Waiting for NetworkManager...");
            while (GetInstance() == null)
                yield return null;
        }
        #endregion

        #region NetworkManager Logger
        internal static void LOG_OnLeftLobbyEvent() =>
            LoggerKit.Log("Event NetworkManager::OnLeftLobby executed!");
        internal static void LOG_OnJoinedLobbyEvent() =>
            LoggerKit.Log("Event NetworkManager::OnJoinedLobby executed!");
        internal static void LOG_OnConnectedToMasterEvent() =>
            LoggerKit.Log("Event NetworkManager::OnConnectedToMaster executed!");
        internal static void LOG_OnLeftRoomEvent() =>
            LoggerKit.Log("Event NetworkManager::OnLeftRoom executed!");
        internal static void LOG_OnCreatedRoomEvent() =>
            LoggerKit.Log("Event NetworkManager::OnCreatedRoom executed!");
        internal static void LOG_OnMasterClientSwitchedEvent(Player player) =>
            LoggerKit.Log("Event NetworkManager::OnMasterClientSwitched executed with " + player.ToString());
        internal static void LOG_OnJoinedRoomEvent() =>
            LoggerKit.Log("Event NetworkManager::OnJoinedRoom executed!");
        internal static void LOG_OnConnectedEvent() =>
            LoggerKit.Log("Event NetworkManager::OnConnected executed!");

        internal static void LOG_OnPlayerJoinedEvent(Player player)
        {
            LoggerKit.LogMessage("Event NetworkManager::OnPlayerJoined executed with " + KitPlayer.Create(player).ToString());
        }
        internal static void LOG_OnPlayerLeftEvent(Player player) =>
            LoggerKit.LogMessage("Event NetworkManager::OnPlayerLeft executed with " + KitPlayer.Create(player).ToString());
        #endregion
    }
}
