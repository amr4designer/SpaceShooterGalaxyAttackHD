using UnityEditor;
using UnityEngine;
using ShmupBaby;

namespace ShmupEditor
{
    /// <summary>
    /// draws the inspector for PlayerMissileWeapon.
    /// </summary>
    [CustomEditor(typeof(PlayerMissileWeapon))]
    public class PlayerMissileWeaponInspector : Editor
    {

        #region Fields

        /// <summary>
        /// the selected PlayerMissileWeapon instance.
        /// </summary>
        PlayerMissileWeapon myScript
        {
            get { return ((PlayerMissileWeapon)target); }
        }
        /// <summary>
        /// the selected PlayerMissileWeapon instance gameObject.
        /// </summary>
        GameObject gameObject
        {
            get { return myScript.gameObject; }
        }

        //SerializedProperty for PlayerMissileWeapon fields
        SerializedProperty Initialized;
        SerializedProperty Stages;

        [SerializeField]
        bool RemoveTrail = false;

        /// <summary>
        /// instance for the selected stage.
        /// </summary>
        [SerializeField]
        MissileWeaponStageData ActiveStage;
        /// <summary>
        /// weapon stage settings SerializedProperty manager.
        /// </summary>
        [SerializeField]
        SerializedMissileWeaponData SerializedActiveStage;

        /// <summary>
        /// the time since the editor was started.
        /// </summary>
        float time
        {
            get { return ((float)EditorApplication.timeSinceStartup); }
        }

        /// <summary>
        /// disables the weapon simulation in the scene view.
        /// </summary>
        [SerializeField]
        private bool DisableShooting;

        [SerializeField]
        private bool ChangingInStages;

        /// <summary>
        /// container for the selected material of the bullet.
        /// </summary>
        [SerializeField]
        ObjectContainer MissileVisualCont = new ObjectContainer(null);

        /// <summary>
        /// the index for the selected stage.
        /// </summary>
        int ActiveTabIndex
        {
            get
            {
                return activeTabIndex;
            }
            set
            {
                if (activeTabIndex != value)
                {
                    OnChangeTab(value);
                }
                activeTabIndex = value;
            }
        }

        //[SerializeField]
        //GUISkin ShmupSkin;

        /// <summary>
        /// the back-end field for ActiveTabIndex.
        /// </summary>
        [SerializeField]
        static int activeTabIndex = -1;

        #endregion

        /// <summary>
        /// the method gets called when the index of the selected tab change.
        /// </summary>
        /// <param name="newTabIndex">the index for the new selected tab.</param>
        void OnChangeTab(int newTabIndex)
        {

            UpdateSerializedActiveStage(newTabIndex);

            SetShooting(newTabIndex);

            ResetStageGUI();

        }

        /// <summary>
        /// change the SerializedActiveStage to point to the new selected stage.
        /// </summary>
        /// <param name="Index">the index of the new selected stage.</param>
        void UpdateSerializedActiveStage(int Index)
        {

            if (Index >= Stages.arraySize || Index < 0)
            {
                SerializedActiveStage = null;
                ActiveStage = null;
            }
            else
            {
                SerializedActiveStage = new SerializedMissileWeaponData(Stages.GetArrayElementAtIndex(Index));
            }

        }

        /// <summary>
        /// disable the weapon simulation if there is no stage selected.
        /// </summary>
        /// <param name="value">the new selected stage</param>
        void SetShooting(int value)
        {
            if (value == -1)
                DisableShooting = true;
            else
                DisableShooting = false;

        }

        /// <summary>
        /// reset the stage GUI to default.
        /// </summary>
        void ResetStageGUI()
        {

            MissileWeaponInspector.RestGUI();

        }

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        void OnEnable()
        {

            Initialized = serializedObject.FindProperty("Initialized");

            Stages = serializedObject.FindProperty("Stages");

            //ShmupSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/ShmupSkin.guiskin");

            if (!EditorApplication.isPlaying)
                myScript.Initialize();

            //hook the Update to the editor update.
            EditorApplication.update += Update;

        }

        /// <summary>
        /// This function is called when the object is unloaded.
        /// </summary>
        void OnDisable()
        {

            EditorApplication.update -= Update;

        }

        /// <summary>
        /// called every frame in Unity editor.
        /// </summary>
        void Update()
        {

            if (EditorApplication.isPlaying || DisableShooting || ActiveStage == null)
                return;

        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawInspector();

            if (!EditorApplication.isPlaying)
            {

                if (!Initialized.boolValue)
                    myScript.Initialize();

                if (ActiveStage != null)
                    UpdateWeaponToActiveStage();
            }

            serializedObject.ApplyModifiedProperties();

            if (ChangingInStages)
            {
                myScript.Initialize();
                ChangingInStages = false;
            }

        }

        /// <summary>
        /// adjust the particle system to the active stage.
        /// </summary>
        void UpdateWeaponToActiveStage()
        {

            if (PrefabUtility.GetPrefabType(gameObject) != PrefabType.Prefab && !EditorApplication.isPlaying)
            {
                myScript.SetToStage(ActiveStage);
            }

        }

        void DrawInspector()
        {

            #region Add & Remove Stage Buttons

            GUILayout.BeginHorizontal();

            //draw the add stage and remove stage buttons, also removes and create the missile prefab for that stage.
            if (GUILayout.Button("Add Stage", GUILayout.Height(25)))
            {
                Stages.InsertArrayElementAtIndex(Stages.arraySize);
                Stages.GetArrayElementAtIndex(Stages.arraySize - 1).FindPropertyRelative("MissileObject")
                    .objectReferenceValue = null;
                Stages.GetArrayElementAtIndex(Stages.arraySize - 1).FindPropertyRelative("MissileScript")
                    .objectReferenceValue = null;
                ChangingInStages = true;
            }

            if (GUILayout.Button("Remove Stage", GUILayout.Height(25)) && Stages.arraySize > 0)
            {
                GameObject missile = Stages.GetArrayElementAtIndex(Stages.arraySize - 1).FindPropertyRelative("MissileObject").objectReferenceValue as GameObject;
                DestroyImmediate(missile);
                Stages.DeleteArrayElementAtIndex(Stages.arraySize - 1);
                ChangingInStages = true;
            }

            GUILayout.EndHorizontal();

            #endregion

            GUILayout.Space(10);

            #region Draw Stages

            //draw button for all stages
            for (int i = 0; i < Stages.arraySize; i++)
            {

                if (i == ActiveTabIndex)
                {

                    if (!GUILayout.Toggle(true, "Stage 0" + (i + 1).ToString(), GUI.skin.button, GUILayout.Height(40)))
                        ActiveTabIndex = -1;

                    //the selected stage need to be drawn open and ready to edit.
                    if (SerializedActiveStage != null)
                    {
                        ActiveStage = SerializedActiveStage.GetMissileWeaponStageData();

                        MissileWeaponInspector.DrawStage(ActiveStage, this);

                        SerializedActiveStage.UpdateSerializedProperty(ActiveStage);
                    }


                }
                else if (GUILayout.Toggle(false, "Stage 0" + (i + 1).ToString(), GUI.skin.button, GUILayout.Height(40)))

                    ActiveTabIndex = i;

                GUILayout.Space(5);
            }

            #endregion

        }

    }

}
