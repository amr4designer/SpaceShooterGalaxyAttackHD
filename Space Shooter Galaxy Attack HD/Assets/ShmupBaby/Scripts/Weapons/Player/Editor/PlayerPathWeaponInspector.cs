using UnityEditor;
using UnityEngine;
using ShmupBaby;

namespace ShmupEditor
{
    /// <summary>
    /// draw the inspector for PlayerPathWeapon and simulate its behavior 
    /// in the scene view.
    /// </summary>
    [CustomEditor(typeof(PlayerPathWeapon))]
    public class PlayerPathWeaponInspector : Editor
    {

        #region Fields
        /// <summary>
        /// the selected PlayerPathWeapon instance.
        /// </summary>
        private PlayerPathWeapon myScript
        {
            get
            {
                return ((PlayerPathWeapon)target);
            }
        }
        /// <summary>
        /// the selected PlayerPathWeapon instance gameObject.
        /// </summary>
        private GameObject gameObject
        {
            get
            {
                return myScript.gameObject;
            }
        }

        //SerializedProperty for PlayerPathWeapon fields
        private SerializedProperty Initialized;
        private SerializedProperty MyPS;
        private SerializedProperty MyPSR;
        private SerializedProperty Stages;


        [SerializeField]
        private bool RemoveTrail = false;

        [SerializeField]
        private bool CurveTab = false;
        [SerializeField]
        private bool TrailTab = false;

        /// <summary>
        /// disable the weapon simulation in the scene view.
        /// </summary>
        [SerializeField]
        private bool DisableShooting;
        /// <summary>
        /// the time for the next shot.
        /// </summary>
        [SerializeField]
        private float nextShot;

        /// <summary>
        /// instance for the selected stage.
        /// </summary>
        [SerializeField]
        private PathWeaponStageData ActiveStage;
        /// <summary>
        /// weapon stage settings SerializedProperty manager.
        /// </summary>
        [SerializeField]
        private SerializedPathWeaponData SerializedActiveStage;
        /// <summary>
        /// the time since the editor was started.
        /// </summary>
        float time
        {
            get
            {
                return ((float)EditorApplication.timeSinceStartup);
            }
        }

        /// <summary>
        /// the ParticleSystem for the selected weapon.
        /// </summary>
        ParticleSystem MyParticleSystem
        {
            get
            {
                return myScript.gameObject.GetComponent<ParticleSystem>();
            }
        }

        /// <summary>
        /// container for the selected material of the bullet.
        /// </summary>
        [SerializeField]
        ObjectContainer BulletMaterialCont = new ObjectContainer(null);
        /// <summary>
        /// container for the selected trail of the bullet.
        /// </summary>
        [SerializeField]
        ObjectContainer TrailMaterialCont = new ObjectContainer(null);
        
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
        private void OnChangeTab(int newTabIndex)
        {
            UpdateSerializedActiveStage(newTabIndex);

            SetShooting(newTabIndex);

            ResetStageGUI();
            //clear the particle from the scene.
            MyParticleSystem.Clear();
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
                SerializedActiveStage = new SerializedPathWeaponData(Stages.GetArrayElementAtIndex(Index));
            }
        }

        /// <summary>
        /// disable the weapon simulation if there is no stage selected.
        /// </summary>
        /// <param name="value">the new selected stage</param>
        void SetShooting(int value)
        {
            if (value == -1)
            {
                DisableShooting = true;
            }
            else
            {
                DisableShooting = false;
            }               

            if (ActiveStage != null && ActiveStage.Rate != 0)
            {
                nextShot = time + 1f / ActiveStage.Rate;
            }                
        }

        /// <summary>
        /// reset the stage GUI to default.
        /// </summary>
        void ResetStageGUI()
        {
            PathWeaponInspector.RestGUI();
        }

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        void OnEnable()
        {
            Initialized = serializedObject.FindProperty("Initialized");

            MyPS = serializedObject.FindProperty("MyPS");
            MyPSR = serializedObject.FindProperty("MyPSR");

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


        void Update()
        {
            if (EditorApplication.isPlaying || DisableShooting || ActiveStage == null)
                return;

            //simulate the weapon behavior when it set to auto fire mode.
            if (time >= nextShot)
            {
                myScript.Fire();
                nextShot = time + 1f / ActiveStage.Rate;
            }
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
        }

        /// <summary>
        /// adjust the particle system to the active stage.
        /// </summary>
        private void UpdateWeaponToActiveStage()
        {
            if (PrefabUtility.GetPrefabType(gameObject) != PrefabType.Prefab && !EditorApplication.isPlaying)
            {
                myScript.SetToStage(ActiveStage);
            }
        }

        /// <summary>
        /// draw the stages in the inspector.
        /// </summary>
        private void DrawInspector()
        {
            #region Add & Remove Stage Buttons

            GUILayout.BeginHorizontal();

            //draw the add stage and remove stage buttons.
            if (GUILayout.Button("Add Stage", GUILayout.Height(25)))
                Stages.InsertArrayElementAtIndex(Stages.arraySize);

            if (GUILayout.Button("Remove Stage", GUILayout.Height(25)) && Stages.arraySize > 0)
                Stages.DeleteArrayElementAtIndex(Stages.arraySize - 1);

            GUILayout.EndHorizontal();

            #endregion

            GUILayout.Space(10);

            #region Draw Stages

            //draw button for all stages
            for (int i = 0; i < Stages.arraySize; i++)
            {
                //the selected stage need to be drawn open and ready to edit.
                if (i == ActiveTabIndex)
                {

                    if (!GUILayout.Toggle(true, "Stage 0" + (i + 1).ToString(), GUI.skin.button, GUILayout.Height(40)))
                        ActiveTabIndex = -1;

                    if (SerializedActiveStage != null)
                    {
                        //get a copy of the Settings member
                        ActiveStage = SerializedActiveStage.GetWeaponStageData();

                        PathWeaponInspector.DrawStage(ActiveStage, this);

                        //copy the stage members to the SerializedProperty.
                        SerializedActiveStage.UpdateSerializedProperty(ActiveStage);
                    }


                }
                else if (GUILayout.Toggle(false, "Stage 0" + (i + 1).ToString(), GUI.skin.button, GUILayout.Height(40)))

                    ActiveTabIndex = i;

                GUILayout.Space(5);
            }

            #endregion
        }

        /// <summary>
        /// Enables the Editor to handle an event in the scene view.
        /// </summary>
        void OnSceneGUI()
        {
            if (MyPS.objectReferenceValue != null)
            {
                PathWeaponInspector.DrawHandle(ActiveStage, MyParticleSystem);
            }             
        }

    }

}
