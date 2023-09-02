using UnityEditor;
using UnityEngine;
using ShmupBaby;

namespace ShmupEditor
{
    /// <summary>
    /// draw the inspector for EnemyPathWeapon and simulate its behavior 
    /// in the scene view.
    /// </summary>
    [CustomEditor(typeof(EnemyPathWeapon))]
    public class EnemyPathWeaponInspector : Editor
    {

        #region Fields

        /// <summary>
        /// the selected EnemyPathWeapon instance.
        /// </summary>
        private EnemyPathWeapon myScript
        {
            get
            {
                return ((EnemyPathWeapon)target);
            }
        }
        /// <summary>
        /// the selected EnemyPathWeapon instance gameObject.
        /// </summary>
        private GameObject gameObject
        {
            get
            {
                return myScript.gameObject;
            }
        }

        //SerializedProperty for EnemyPathWeapon fields
        private SerializedProperty Initialized;
        private SerializedProperty MyPS;
        private SerializedProperty MyPSR;
        private SerializedProperty Settings;

        /// <summary>
        /// weapon stage settings SerializedProperty manager.
        /// </summary>
        private SerializedPathWeaponData SerializedSettings;

        /// <summary>
        /// instance for the EnemyPathWeapon Settings.
        /// </summary>
        private PathWeaponStageData Stage;

        /// <summary>
        /// the time for the next shot.
        /// </summary>
        [SerializeField]
        private float nextShot;
        /// <summary>
        /// the time since the editor was started.
        /// </summary>
        private float time
        {
            get { return ((float)EditorApplication.timeSinceStartup); }
        }

        #endregion

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        void OnEnable()
        {
            Initialized = serializedObject.FindProperty("Initialized");
            MyPS = serializedObject.FindProperty("MyPS");
            MyPSR = serializedObject.FindProperty("MyPSR");
            Settings = serializedObject.FindProperty("Settings");

            PathWeaponInspector.RestGUI();

            //initialize the SerializedSettings
            SerializedSettings = new SerializedPathWeaponData(Settings);
            //get a copy of the Settings member
            Stage = SerializedSettings.GetWeaponStageData();
                        
            if (!EditorApplication.isPlaying)
            {
                myScript.Initialize();

                myScript.SetToStage(Stage);
            }

            //hook the Update to the editor update.
            EditorApplication.update += Update;
        }


        void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            if (SerializedSettings != null)
            {
                //get a copy of the Settings member
                Stage = SerializedSettings.GetWeaponStageData();

                PathWeaponInspector.DrawStage(Stage, this);

                //copy the stage members to the SerializedProperty.
                SerializedSettings.UpdateSerializedProperty(Stage);
            }

            if (!EditorApplication.isPlaying)
            {

                if (!Initialized.boolValue)
                    myScript.Initialize();

                myScript.SetToStage(Stage);

            }


            serializedObject.ApplyModifiedProperties();

        }

        /// <summary>
        /// Enables the Editor to handle an event in the scene view.
        /// </summary>
        void OnSceneGUI()
        {
            //draw the bullets collider.
            if (MyPS.objectReferenceValue != null)
            {
                PathWeaponInspector.DrawHandle(Stage, (ParticleSystem)MyPS.objectReferenceValue);
            }              
        }


        void Update()
        {

            if (EditorApplication.isPlaying)
            {
                return;
            }
                
            //simulate the weapon behavior when it set to auto fire mode.
            if (time >= nextShot)
            {
                myScript.Fire();
                //update the time for the next shot
                nextShot = time + 1f / Stage.Rate;
            }

        }

    }

}