using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC;

namespace VRCTestingKit.VRChat
{
    public static class KitServer
    {
        #region Server Variables
        internal static List<KitPlayer> _Players = new List<KitPlayer>();
        #endregion

        #region Server Properties
        public static KitPlayer[] Players => _Players.ToArray();
        #endregion

        #region Server Handlers
        internal static void PlayerJoined(Player player) => KitPlayer.Create(player);
        internal static void PlayerLeft(Player player)
        {
            KitPlayer kitPlayer = _Players.FirstOrDefault(a => a.Player == player);

            if (kitPlayer == null)
                return;
            _Players.Remove(kitPlayer);
        }
        #endregion
    }
}
