using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace VRCTestingKit
{
    public static class ExtensionKit
    {
        #region String Extensions
        public static void CopyToClipboard(this string s)
        {
            TextEditor te = new TextEditor();
            te.text = s;
            te.SelectAll();
            te.Copy();
        }
        #endregion
    }
}
