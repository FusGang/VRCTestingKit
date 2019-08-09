using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRCTestingKit.VRChat
{
    public static class KitVRCUiManager
    {
        #region Reflection Variables
        private static MethodInfo _get_Instance;
        #endregion

        #region VRCUiManager Properties
        public static VRCUiManager Instance
        {
            get
            {
                if (_get_Instance == null)
                    _get_Instance = typeof(VRCUiManager).GetMethod("get_Instance", BindingFlags.Public | BindingFlags.Static);
                return _get_Instance.Invoke(null, new object[0]) as VRCUiManager;
            }
        }
        #endregion
    }
}
