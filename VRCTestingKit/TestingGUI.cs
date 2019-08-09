using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using RuntimeUnityEditor.Core.Utils;
using RuntimeUnityEditor.Core.Inspector.Entries;

using VRCTestingKit.VRChat;

using VRC;

namespace VRCTestingKit
{
    internal class TestingGUI : MonoBehaviour
    {
        #region Constants
        public const string Title = "VRCTestingKit by AtiLion";
        public const KeyCode ToggleKey = KeyCode.F1;
        public const KeyCode MouseKey = KeyCode.Tab;
        #endregion

        #region Movement Variables
        private static FieldInfo _allowMovement;
        private static NeckMouseRotator _neckMouseRotator;
        #endregion

        #region Movement Properties
        public static NeckMouseRotator NeckMouseRotator
        {
            get
            {
                if(_neckMouseRotator == null)
                {
                    _neckMouseRotator = GameObject.FindObjectOfType<NeckMouseRotator>();
                    LoggerKit.Log("Found NeckMouseRotator!");
                }
                return _neckMouseRotator;
            }
        }
        public static bool AllowMovement
        {
            get
            {
                if (_allowMovement == null)
                {
                    _allowMovement = typeof(NeckMouseRotator).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(a => a.FieldType == typeof(bool));
                    LoggerKit.Log("Found allowMovement: " + _allowMovement.Name);
                }
                return (bool)_allowMovement.GetValue(NeckMouseRotator);
            }
            set
            {
                if (_allowMovement == null)
                {
                    _allowMovement = typeof(NeckMouseRotator).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(a => a.FieldType == typeof(bool));
                    LoggerKit.Log("Found allowMovement: " + _allowMovement.Name);
                }
                _allowMovement.SetValue(NeckMouseRotator, value);
            }
        }
        #endregion

        #region Main Variables
        private bool _show = false;
        private bool _showMouse = false;
        internal bool _locked = false;
        
        private Rect _rectMainWindow;
        private Vector2 _vecMainScroll;

        private GameObject _selectedGameObject;
        #endregion

        #region Editor Variables
        private bool _showEditor = false;
        #endregion

        #region Log Variables
        private bool _showLog = false;

        private Rect _rectLogWindow;
        private Vector2 _vecLogScroll;
        #endregion

        #region Users Variables
        private bool _showUsers = false;

        private KitPlayer _selectedPlayer;
        private Shader[] _playerShaders;
        #endregion

        #region UI Variables
        private MethodInfo _get_quickmenu_instance;
        #endregion

        #region UI Properties
        public QuickMenu QuickMenu_Instance
        {
            get
            {
                if (_get_quickmenu_instance == null)
                    _get_quickmenu_instance = typeof(QuickMenu).GetMethod("get_Instance", BindingFlags.Public | BindingFlags.Static);
                return (QuickMenu)_get_quickmenu_instance.Invoke(null, new object[0]);
            }
        }
        #endregion

        #region ContextMenu Variables
        private GameObject _contextGameObject;
        private KitPlayer _contextPlayer;
        private Vector3? _mouseLocation;
        #endregion

        #region Main Properties
        public bool Show
        {
            get => _show;
            set
            {
                _show = value;

                if (!value)
                    VRCTestingKitMod.RuntimeEditor.Show = false;
                else
                    VRCTestingKitMod.RuntimeEditor.Show = _showEditor;
            }
        }
        public bool ShowMouse
        {
            get => _showMouse;
            set
            {
                _showMouse = value;
                VRCUiCursorManager.SetUiActive(value);

                _contextGameObject = null;
                _mouseLocation = null;
            }
        }
        #endregion

        #region Unity Functions
        void Start()
        {
            Rect rectScreen = new Rect(0f, 0f, Screen.width, Screen.height);

            _rectMainWindow = new Rect(1f, 1f, 350f, 500f);
            _rectLogWindow = new Rect(Mathf.Floor(rectScreen.xMax / 2f) - 400f, rectScreen.yMin, 800f, 200f);
        }
        void OnGUI()
        {
            if (!_show || _locked)
                return;

            EditorUtilities.DrawSolidWindowBackground(_rectMainWindow);
            _rectMainWindow = GUILayout.Window(6969, _rectMainWindow, MainWindowContent, Title);

            if(_showLog)
            {
                EditorUtilities.DrawSolidWindowBackground(_rectLogWindow);
                _rectLogWindow = GUILayout.Window(6970, _rectLogWindow, LogWindowContent, "Logger");
            }

            if(_mouseLocation != null && _contextGameObject != null)
            {
                float itemCount = 3f;

                float height = ((GUI.skin.button.CalcSize(new GUIContent(" ")).y + 4f) * itemCount) + 4f;
                Rect rect = new Rect(((Vector3)_mouseLocation).x, Screen.height - ((Vector3)_mouseLocation).y, 200f, height);

                EditorUtilities.DrawSolidWindowBackground(rect);
                GUILayout.BeginArea(rect);
                ContextMenuContent();
                GUILayout.EndArea();
            }
        }
        void Update()
        {
            if (_locked)
                return;

            if (Input.GetKeyDown(ToggleKey))
                Show = !Show;
            if (Input.GetKeyDown(MouseKey))
                ShowMouse = !ShowMouse;
            if (Input.GetKeyDown(KeyCode.Escape))
                ShowMouse = false;
            if (Input.GetMouseButtonDown(1))
            {
                if(_selectedGameObject != null && _contextGameObject == null)
                {
                    _contextGameObject = _selectedGameObject;
                    _mouseLocation = Input.mousePosition;

                    Player player = _contextGameObject.transform.parent.GetComponent<Player>();

                    if (player != null)
                        _contextPlayer = KitPlayer.Create(player);
                    else
                        _contextPlayer = null;
                }
                else
                {
                    _contextGameObject = null;
                    _mouseLocation = null;
                    _contextPlayer = null;
                }
            }
        }
        void LateUpdate()
        {
            if (ShowMouse)
                AllowMovement = false;
        }
        void FixedUpdate()
        {
            if(ShowMouse && Show && !_locked)
            {
                RaycastHit hit;

                if (_contextGameObject != null)
                    return;
                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();

                    if (renderer == null)
                        return;
                    if (_selectedGameObject != null)
                        HighlightsFX.EnableObjectHighlight(_selectedGameObject.GetComponent<Renderer>(), false);
                    HighlightsFX.EnableObjectHighlight(renderer, true);

                    _selectedGameObject = hit.collider.gameObject;
                }
                else
                    _selectedGameObject = null;
            }
            else if(_selectedGameObject != null)
            {
                HighlightsFX.EnableObjectHighlight(_selectedGameObject.GetComponent<Renderer>(), false);
                _selectedGameObject = null;
            }
        }
        #endregion

        #region GUI Functions
        private void MainWindowContent(int id)
        {
            _vecMainScroll = GUILayout.BeginScrollView(_vecMainScroll);

            // General
            if (GUILayout.Button((_showEditor ? "Close" : "Open") + " Editor"))
            {
                VRCTestingKitMod.RuntimeEditor.Show = !_showEditor;
                _showEditor = !_showEditor;
            }
            if(GUILayout.Button((_showLog ? "Close" : "Open") + " Logger"))
                _showLog = !_showLog;

            // Users
            GUILayout.BeginVertical(GUI.skin.box);
            {
                GUILayout.Label("User Manager");
                if (_selectedPlayer == null)
                {
                    if (GUILayout.Button("Show Self in Inspector"))
                    {
                        if (!_showEditor)
                        {
                            VRCTestingKitMod.RuntimeEditor.Show = true;
                            _showEditor = true;
                        }

                        VRCTestingKitMod.RuntimeEditor.Inspector.InspectorClear();
                        VRCTestingKitMod.RuntimeEditor.Inspector.InspectorPush(new InstanceStackEntry(KitPlayer.Instance.Player, KitPlayer.Instance.DisplayName + "[" + KitPlayer.Instance.Username + "]"));
                    }
                    if (GUILayout.Button("Navigate to Self"))
                    {
                        if (!_showEditor)
                        {
                            VRCTestingKitMod.RuntimeEditor.Show = true;
                            _showEditor = true;
                        }

                        VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(KitPlayer.Instance.Player.transform);
                    }
                    if (GUILayout.Button("Navigate to Own Avatar"))
                    {
                        if (!_showEditor)
                        {
                            VRCTestingKitMod.RuntimeEditor.Show = true;
                            _showEditor = true;
                        }

                        VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(KitPlayer.Instance.AvatarObject.transform);
                    }
                    if (GUILayout.Button((_showUsers ? "Hide" : "Show") + " Users"))
                        _showUsers = !_showUsers;
                    if (_showUsers)
                        foreach (KitPlayer player in KitServer._Players)
                            if (GUILayout.Button(player.DisplayName))
                                _selectedPlayer = player;
                }
                else if(_playerShaders != null)
                {
                    if (GUILayout.Button("Back"))
                        _playerShaders = null;
                    foreach(Shader shader in _playerShaders)
                        if (GUILayout.Button(shader.name))
                            shader.name.CopyToClipboard();
                }
                else
                {
                    if (GUILayout.Button("Back"))
                        _selectedPlayer = null;
                    GUILayout.Label("Display Name: " + _selectedPlayer.DisplayName);
                    GUILayout.Label("Username: " + _selectedPlayer.Username);
                    GUILayout.Label("ID: " + _selectedPlayer.ID);
                    if (GUILayout.Button("Copy ID"))
                        _selectedPlayer.ID.CopyToClipboard();
                    if (GUILayout.Button("Copy Link"))
                        ("https://www.vrchat.net/home/user/" + _selectedPlayer.ID).CopyToClipboard();
                    if (GUILayout.Button("Open Link"))
                        Process.Start("https://www.vrchat.net/home/user/" + _selectedPlayer.ID);
                    if(GUILayout.Button("Open in Inspector"))
                    {
                        if(!_showEditor)
                        {
                            VRCTestingKitMod.RuntimeEditor.Show = true;
                            _showEditor = true;
                        }

                        VRCTestingKitMod.RuntimeEditor.Inspector.InspectorClear();
                        VRCTestingKitMod.RuntimeEditor.Inspector.InspectorPush(new InstanceStackEntry(_selectedPlayer.Player, _selectedPlayer.DisplayName + "[" + _selectedPlayer.Username + "]"));
                    }
                    if(GUILayout.Button("Navigate to GameObject"))
                    {
                        if (!_showEditor)
                        {
                            VRCTestingKitMod.RuntimeEditor.Show = true;
                            _showEditor = true;
                        }

                        VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(_selectedPlayer.Player.transform);
                    }
                    if(GUILayout.Button("Navigate to Avatar"))
                    {
                        if (!_showEditor)
                        {
                            VRCTestingKitMod.RuntimeEditor.Show = true;
                            _showEditor = true;
                        }

                        VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(_selectedPlayer.AvatarObject.transform);
                    }
                    if(GUILayout.Button("Show Shaders"))
                    {
                        List<Shader> shaders = new List<Shader>();

                        foreach (Renderer renderer in _selectedPlayer.AvatarObject.GetComponentsInChildren<Renderer>(true))
                            foreach(Material material in renderer.materials)
                                if (material.shader != null && !shaders.Contains(material.shader))
                                    shaders.Add(material.shader);
                        _playerShaders = shaders.ToArray();
                    }
                    if(GUILayout.Button("Dump Shaders to Console"))
                    {
                        LoggerKit.LogMessage("START OF SHADER DUMP FOR USER " + _selectedPlayer.DisplayName);
                        foreach (Renderer renderer in _selectedPlayer.AvatarObject.GetComponentsInChildren<Renderer>(true))
                            foreach (Material material in renderer.materials)
                                if (material.shader != null)
                                    LoggerKit.LogMessage(material.shader.name);
                        LoggerKit.LogMessage("END OF SHADER DUMP");
                    }
                }
            }
            GUILayout.EndVertical();

            // UI
            GUILayout.BeginVertical(GUI.skin.box);
            {
                GUILayout.Label("UI Manager");

                // Quick Menu
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    GUILayout.Label("Quick Menu");
                    if(GUILayout.Button("Open Quick Menu in Inspector"))
                    {
                        if (!_showEditor)
                        {
                            VRCTestingKitMod.RuntimeEditor.Show = true;
                            _showEditor = true;
                        }

                        VRCTestingKitMod.RuntimeEditor.Inspector.InspectorClear();
                        VRCTestingKitMod.RuntimeEditor.Inspector.InspectorPush(new InstanceStackEntry(QuickMenu_Instance, "Quick Menu"));
                    }
                    if(GUILayout.Button("Navigate to Quick Menu"))
                    {
                        if (!_showEditor)
                        {
                            VRCTestingKitMod.RuntimeEditor.Show = true;
                            _showEditor = true;
                        }

                        VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(QuickMenu_Instance.transform);
                    }
                }
                GUILayout.EndVertical();

                // Avatar Menu
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Avatar Menu");
                if (GUILayout.Button("Navigate to Avatar Menu"))
                {
                    if (!_showEditor)
                    {
                        VRCTestingKitMod.RuntimeEditor.Show = true;
                        _showEditor = true;
                    }

                    VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(GameObject.Find(QuickMenu_Instance.avatarScreenPath).transform);
                }
                GUILayout.EndVertical();

                // Details Menu
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Details Menu");
                if (GUILayout.Button("Navigate to Details Menu"))
                {
                    if (!_showEditor)
                    {
                        VRCTestingKitMod.RuntimeEditor.Show = true;
                        _showEditor = true;
                    }

                    VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(GameObject.Find(QuickMenu_Instance.detailsScreenPath).transform);
                }
                GUILayout.EndVertical();

                // Playlists Menu
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Playlists Menu");
                if (GUILayout.Button("Navigate to Playlists Menu"))
                {
                    if (!_showEditor)
                    {
                        VRCTestingKitMod.RuntimeEditor.Show = true;
                        _showEditor = true;
                    }

                    VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(GameObject.Find(QuickMenu_Instance.playlistsScreenPath).transform);
                }
                GUILayout.EndVertical();

                // Social Menu
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Social Menu");
                if (GUILayout.Button("Navigate to Social Menu"))
                {
                    if (!_showEditor)
                    {
                        VRCTestingKitMod.RuntimeEditor.Show = true;
                        _showEditor = true;
                    }

                    VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(GameObject.Find(QuickMenu_Instance.socialScreenPath).transform);
                }
                GUILayout.EndVertical();

                // Settings Menu
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Settings Menu");
                if (GUILayout.Button("Navigate to Settings Menu"))
                {
                    if (!_showEditor)
                    {
                        VRCTestingKitMod.RuntimeEditor.Show = true;
                        _showEditor = true;
                    }

                    VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(GameObject.Find(QuickMenu_Instance.settingsScreenPath).transform);
                }
                GUILayout.EndVertical();

                // Safety Menu
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Safety Menu");
                if (GUILayout.Button("Navigate to Safety Menu"))
                {
                    if (!_showEditor)
                    {
                        VRCTestingKitMod.RuntimeEditor.Show = true;
                        _showEditor = true;
                    }

                    VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(GameObject.Find(QuickMenu_Instance.safetyScreenPath).transform);
                }
                GUILayout.EndVertical();

                // User Info Menu
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("User Info Menu");
                if (GUILayout.Button("Navigate to User Info Menu"))
                {
                    if (!_showEditor)
                    {
                        VRCTestingKitMod.RuntimeEditor.Show = true;
                        _showEditor = true;
                    }

                    VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(GameObject.Find(QuickMenu_Instance.userInfoScreenPath).transform);
                }
                GUILayout.EndVertical();

                // Worlds Menu
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Worlds Menu");
                if (GUILayout.Button("Navigate to Worlds Menu"))
                {
                    if (!_showEditor)
                    {
                        VRCTestingKitMod.RuntimeEditor.Show = true;
                        _showEditor = true;
                    }

                    VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(GameObject.Find(QuickMenu_Instance.worldsScreenPath).transform);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();

            // Finalize
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
        private void LogWindowContent(int id)
        {
            _vecLogScroll = GUILayout.BeginScrollView(_vecLogScroll);

            GUI.skin.box.wordWrap = true;
            GUILayout.BeginVertical(GUI.skin.box);
            {
                foreach(string log in LoggerKit.Logs)
                    GUILayout.Label(log);
            }
            GUILayout.EndVertical();
            GUI.skin.box.wordWrap = false;

            // Finalize
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        private void ContextMenuContent()
        {
            GUILayout.BeginVertical(GUI.skin.box);

            if(_contextPlayer != null)
            {
                if(GUILayout.Button("Open User"))
                {
                    _selectedPlayer = _contextPlayer;

                    _contextGameObject = null;
                    _mouseLocation = null;
                    _contextPlayer = null;
                }
            }
            else
            {
                if(GUILayout.Button("Clone"))
                {
                    GameObject newObj = GameObject.Instantiate(_contextGameObject);

                    if (!_showEditor)
                    {
                        VRCTestingKitMod.RuntimeEditor.Show = true;
                        _showEditor = true;
                    }

                    VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(newObj.transform);

                    _contextGameObject = null;
                    _mouseLocation = null;
                    _contextPlayer = null;
                }
            }
            if(GUILayout.Button("Open in Inspector"))
            {
                if (!_showEditor)
                {
                    VRCTestingKitMod.RuntimeEditor.Show = true;
                    _showEditor = true;
                }

                VRCTestingKitMod.RuntimeEditor.Inspector.InspectorClear();
                VRCTestingKitMod.RuntimeEditor.Inspector.InspectorPush(new InstanceStackEntry(_contextGameObject, _contextGameObject.name));

                _contextGameObject = null;
                _mouseLocation = null;
                _contextPlayer = null;
            }
            if (GUILayout.Button("Navigate To"))
            {
                if (!_showEditor)
                {
                    VRCTestingKitMod.RuntimeEditor.Show = true;
                    _showEditor = true;
                }

                VRCTestingKitMod.RuntimeEditor.TreeViewer.SelectAndShowObject(_contextGameObject.transform);

                _contextGameObject = null;
                _mouseLocation = null;
                _contextPlayer = null;
            }

            GUILayout.EndVertical();
        }
        #endregion
    }
}
