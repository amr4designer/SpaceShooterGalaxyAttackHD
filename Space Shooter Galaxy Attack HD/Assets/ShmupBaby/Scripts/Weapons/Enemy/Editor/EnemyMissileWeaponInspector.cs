using UnityEditor;
using UnityEngine;
using ShmupBaby;

namespace ShmupEditor
{

    /// <summary>
    /// draw the inspector for EnemyMissileWeapon and simulate its behavior 
    /// in the scene view.
    /// </summary>
    [CustomEditor(typeof(EnemyMissileWeapon))]
    public class EnemyMissileWeaponInspector : Editor
    {

        #region Fields

        /// <summary>
        /// the selected EnemyMissileWeapon instance.
        /// </summary>
        private EnemyMissileWeapon myScript
        {
            get { return ((EnemyMissileWeapon)target); }
        }
        /// <summary>
        /// the selected EnemyMissileWeapon instance gameObject.
        /// </summary>
        private GameObject gameObject
        {
            get { return myScript.gameObject; }
        }

        //SerializedProperty for EnemyMissileWeapon fields
        private SerializedProperty Initialized;
        private SerializedMissileWeaponData SerializedSettings;
        private SerializedProperty Settings;

        private MissileWeaponStageData Stage;

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

            Settings = serializedObject.FindProperty("Settings");

            MissileWeaponInspector.RestGUI();

            //initialize the SerializedSettings
            SerializedSettings = new SerializedMissileWeaponData(Settings);
            //get a copy of the Settings member
            Stage = SerializedSettings.GetMissileWeaponStageData();

            if (!EditorApplication.isPlaying)
            {
                myScript.Initialize();

                myScript.SetToStage(Stage);
            }

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
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            if (SerializedSettings != null)
            {
                //get a copy of the Settings member
                Stage = SerializedSettings.GetMissileWeaponStageData();

                MissileWeaponInspector.DrawStage(Stage, this);

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
        /// called on every frame in Unity editor.
        /// </summary>
        void Update()
        {

            if (EditorApplication.isPlaying)
                return;

        }

    }

}