using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace ShmupEditor
{

    /// <summary>
    /// Wrapper for unity object.
    /// </summary>
    [System.Serializable]
    public class ObjectContainer
    {
        /// <summary>
        /// the Object.
        /// </summary>
        public Object O;

        public ObjectContainer(Object o)
        {
            O = o;
        }

    }

    /// <summary>
    /// provides an easy way to select prefab by their icon.
    /// </summary>
    public class PickByIcon : EditorWindow
    {
        
        /// <summary>
        /// the space between he box and the icons.
        /// </summary>
        private const float BoxSpace = 5;
        /// <summary>
        /// the space between Assets icons.
        /// </summary>
        private const float IconSpace = 5;
        /// <summary>
        /// the slider width.
        /// </summary>
        private const float SliderSpace = 15;
        
        /// <summary>
        /// the ID for the selected icon.
        /// </summary>
        int IconID
        {
            get
            {
                return _iconID;
            }
            set
            {
                if (value != -1)
                {
                    _targetContainer.O = _icons[value].Pref;
                    if (_inspectorWindow != null)
                        _inspectorWindow.Repaint();
                    this.Close();
                }
                _iconID = -1;
            }
        }

        /// <summary>
        /// Assets directory.
        /// </summary>
        private static string dir;

        /// <summary>
        /// the prefab icons.
        /// </summary>
        private List<AssetToggle> _icons = new List<AssetToggle>();
        /// <summary>
        /// the container for the selected prefab.
        /// </summary>
        private ObjectContainer _targetContainer;
        /// <summary>
        /// the editor that creates this window.
        /// </summary>
        private Editor _inspectorWindow;
        /// <summary>
        /// the width of this window.
        /// </summary>
        private float _windowWidth;
        /// <summary>
        /// the height of this window.
        /// </summary>
        private float _windowHight;
        /// <summary>
        /// the Assets icon width.
        /// </summary>
        private float _iconWidth;
        /// <summary>
        /// the current scroll position.
        /// </summary>
        private Vector2 _scrollPosition = new Vector2(BoxSpace, BoxSpace);
        /// <summary>
        /// the back-end field for IconID.
        /// </summary>
        private int _iconID = -1;

        /// <summary>
        /// Initializes the Windows.
        /// </summary>
        /// <param name="field">the container that will be filled with the selected prefab.</param>
        /// <param name="directory">the directory name.</param>
        /// <param name="extension">the extension of the file</param>
        /// <param name="yourWindow">reference of the editor that creates a call for this constructor.</param>
        /// <param name="width">the width of the window</param>
        /// <param name="height">the height of the window.</param>
        public void WindowsInitialize(ObjectContainer field, string directory, string extension, Editor yourWindow, float width, float height)
        {
            if (directory != null)
                Refresh(directory, extension);

            GetFilesUnderDirectory("Bullets", "mat");

            //sets the window to the mouse position and sets itss size
            Vector2 mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            this.position = new Rect(mousePosition.x, mousePosition.y, width, height);
            this.maxSize = new Vector2(width, height);
            this.minSize = new Vector2(width, height);
            _windowWidth = width;
            _windowHight = height;

            //calculates the width of the icons
            _iconWidth = _windowWidth - IconSpace * 2 - BoxSpace * 2 - SliderSpace;

            _targetContainer = field;

            _inspectorWindow = yourWindow;
        }

        /// <summary>
        /// Initializes the Windows.
        /// </summary>
        /// <param name="field">the container that will be filled with the selected prefab.</param>
        /// <param name="directory">the directory name.</param>
        /// <param name="extension">the extension of the file</param>
        /// <param name="yourWindow">reference of the editor that creates a call for this constructor.</param>
        public void WindowsInitialize(ObjectContainer field, string directory, string extension, Editor yourWindow)
        {
            WindowsInitialize(field, directory, extension, yourWindow, 100, 300);
        }

        /// <summary>
        /// Use OnGUI to draw all the controls of your window.
        /// </summary>
        private void OnGUI()
        {

            RefreshIcon();

            DrawIconScroll();

        }

        /// <summary>
        /// draw the Assets icon in a scroll view.
        /// </summary>
        private void DrawIconScroll()
        {
            //background box.
            GUI.Box(new Rect(BoxSpace, BoxSpace, _windowWidth - BoxSpace * 2, _windowHight - BoxSpace * 2), GUIContent.none, EditorStyles.textArea);

            _scrollPosition = GUI.BeginScrollView(new Rect(BoxSpace + IconSpace, BoxSpace + IconSpace, _iconWidth + SliderSpace, _windowHight - (BoxSpace + IconSpace) * 2), _scrollPosition, new Rect(0, 0, _iconWidth, _icons.Count * _iconWidth));

            for (int i = 0; i < _icons.Count; i++)
            {
                //change the icon id if it's selected.
                if (i == IconID)
                {
                    GUI.Toggle(new Rect(0, i * _iconWidth, _iconWidth, _iconWidth), true, _icons[i].Preview, GUI.skin.button);
                }
                else if (GUI.Toggle(new Rect(0, i * _iconWidth, _iconWidth, _iconWidth), false, _icons[i].Preview, GUI.skin.button))
                {

                    IconID = i;
                }
            }

            GUI.EndScrollView();
        }

        /// <summary>
        /// populate the _icons with all assets that match the conditions.
        /// </summary>
        /// <param name="directory">the directory of the assets</param>
        /// <param name="extension">the extension of the assets</param>
        void Refresh(string directory, string extension)
        {

            _icons.Clear();

            IEnumerable<string> targetPaths = GetFilesUnderDirectory(directory, extension);

            foreach (string path in targetPaths)
            {

                string pathFromAssets = RemoveAssetsFromPath(path);

                Object prefab = AssetDatabase.LoadAssetAtPath<Object>(path);

                EditorUtility.SetDirty(prefab);

                Texture2D previewImage = AssetPreview.GetAssetPreview(prefab);

                _icons.Add(new AssetToggle(pathFromAssets, prefab, new GUIContent(previewImage)));

            }

        }

        /// <summary>
        /// refresh the texture of the assets icons
        /// </summary>
        private void RefreshIcon()
        {

            foreach (AssetToggle assetToggle in _icons)
            {
                assetToggle.Preview.image = AssetPreview.GetAssetPreview(assetToggle.Pref);
            }

        }

        /// <summary>
        /// get the paths for all files that are lying under the same directory folder.
        /// </summary>
        /// <param name="directory">the name of the common folder</param>
        /// <param name="extension">the extension required to filter the result</param>
        /// <returns></returns>
        private static string[] GetFilesUnderDirectory(string directory, string extension)
        {
            string[] targetDirectory = Directory.GetDirectories(@"Assets/", directory, SearchOption.AllDirectories);

            if (targetDirectory.Length == 0)
            {
                Debug.LogError("please move your prefab under folder named ( " + directory + " )");
                return targetDirectory;
            }

            List<string> targetFiles = new List<string>();

            foreach (string s in targetDirectory)
            {
                targetFiles.AddRange(Directory.GetFiles(s, "*." + extension, SearchOption.TopDirectoryOnly));
            }

            if (targetFiles.Count == 0)
                Debug.LogError("no prefab where found under ( " + directory + " ) make sure that they have the following extension ( ." + extension + " )");

            return targetFiles.ToArray();

        }

        /// <summary>
        /// remove the assets directory from a given path,
        /// making it local to the assets folder.
        /// </summary>
        /// <param name="path">The path that needs to change.</param>
        /// <returns>The path after editing</returns>
        static string RemoveAssetsFromPath(string path)
        {
            path = path.Substring("Assets".Length).Replace("\\", "/");
            return path;
        }

        /// <summary>
        /// The object that represents the assets in the PickByIcon window
        /// </summary>
        public class AssetToggle
        {
            /// <summary>
            /// the assets path relative to the assets folder
            /// </summary>
            public string Path;
            /// <summary>
            /// the assets Object.
            /// </summary>
            public Object Pref;
            /// <summary>
            /// the assets GUI
            /// </summary>
            public GUIContent Preview;

            /// <summary>
            /// AssetToggle constructor
            /// </summary>
            /// <param name="path">the assets path relative to the assets folder</param>
            /// <param name="pref">the assets Object.</param>
            /// <param name="preview">he assets GUI.</param>
            public AssetToggle(string path, Object pref, GUIContent preview)
            {
                Path = path;
                Pref = pref;
                Preview = preview;
            }

        }

    }

}