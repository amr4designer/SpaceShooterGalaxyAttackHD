using UnityEditor;
using UnityEngine;
using ShmupBaby;

namespace ShmupEditor
{
    /// <summary>
    /// draw the inspector for EnemyRadialWeapon and simulate its behavior 
    /// in the scene view.
    /// </summary>
    [CustomEditor(typeof(EnemyRadialWeapon))]
    public class EnemyRadialWeaponInspector : Editor
    {

        #region Fields

        /// <summary>
        /// the selected EnemyRadialWeapon instance.
        /// </summary>
        EnemyRadialWeapon myScript
        {
            get { return ((EnemyRadialWeapon)target); }
        }
        /// <summary>
        /// the selected EnemyRadialWeapon instance gameObject.
        /// </summary>
        GameObject gameObject
        {
            get { return myScript.gameObject; }
        }

        //SerializedProperty for EnemyRadialWeapon fields
        SerializedProperty Initialized;
        SerializedProperty MyPS;
        SerializedProperty MyPSR;
        SerializedRadialWeaponData SerializedSettings;
        SerializedProperty Settings;
        RadialWeaponStageData Stage;

        /// <summary>
        /// the time for the next shot.
        /// </summary>
        [SerializeField]
        float nextShot;
        /// <summary>
        /// the time since the editor was started.
        /// </summary>
        float time
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

            RadialWeaponInspector.RestGUI();

            //initialize the SerializedSettings
            SerializedSettings = new SerializedRadialWeaponData(Settings);
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
                Stage = SerializedSettings.GetWeaponStageData();

                RadialWeaponInspector.DrawStage(Stage, this);

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
            //draw bullets Radial and collider.
            if (MyPS.objectReferenceValue!= null)
                RadialWeaponInspector.DrawHandle(Stage, (ParticleSystem)MyPS.objectReferenceValue);

        }

        /// <summary>
        /// called every frame in Unity editor.
        /// </summary>
        void Update()
        {

            if (EditorApplication.isPlaying)
                return;

            //simulate the weapon behavior when it's set to auto fire mode.
            if (time >= nextShot)
            {
                myScript.Fire();
                //update the time for the next shot
                nextShot = time + 1f / Stage.Rate;
            }

        }

    }

}