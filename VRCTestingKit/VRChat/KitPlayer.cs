using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC;
using VRC.Core;

using UnityEngine;

namespace VRCTestingKit.VRChat
{
    public class KitPlayer
    {
        #region Reflection Variables
        private static MethodInfo _player_get_user;
        private static MethodInfo _player_get_Instance;

        private static MethodInfo _vrcplayer_get_AvatarManager;
        #endregion

        #region KitPlayer Properties
        public Player Player { get; private set; }
        public VRCPlayer VRCPlayer => Player.vrcPlayer;
        public APIUser APIUser
        {
            get
            {
                if (_player_get_user == null)
                    _player_get_user = typeof(Player).GetMethod("get_user", BindingFlags.Public | BindingFlags.Instance);
                return _player_get_user.Invoke(Player, new object[0]) as APIUser;
            }
        }
        public static KitPlayer Instance
        {
            get
            {
                if (_player_get_Instance == null)
                    _player_get_Instance = typeof(Player).GetMethod("get_Instance", BindingFlags.Public | BindingFlags.Static);
                return Create(_player_get_Instance.Invoke(null, new object[0]) as Player);
            }
        }

        public VRCAvatarManager AvatarManager
        {
            get
            {
                if (_vrcplayer_get_AvatarManager == null)
                    _vrcplayer_get_AvatarManager = typeof(VRCPlayer).GetMethod("get_AvatarManager", BindingFlags.Public | BindingFlags.Instance);
                return _vrcplayer_get_AvatarManager.Invoke(VRCPlayer, new object[0]) as VRCAvatarManager;
            }
        }
        #endregion

        #region User Properties
        public string DisplayName => APIUser.displayName;
        public string Username => APIUser.username;
        public string ID => APIUser.id;

        public GameObject AvatarObject => AvatarManager.currentAvatarObject;
        #endregion

        private KitPlayer(Player player)
        {
            Player = player;

            KitServer._Players.Add(this);
        }

        #region KitPlayer Functions
        public static KitPlayer Create(Player player)
        {
            KitPlayer kitPlayer = KitServer._Players.FirstOrDefault(a => a.Player == player);

            if (kitPlayer == null)
                kitPlayer = new KitPlayer(player);
            return kitPlayer;
        }
        #endregion

        #region .NET Functions
        public override string ToString() =>
            "(" + DisplayName + ")[" + Username + "] - " + ID;
        #endregion
    }
}
