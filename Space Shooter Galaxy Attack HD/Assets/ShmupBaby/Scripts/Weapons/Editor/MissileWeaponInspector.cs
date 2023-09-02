using UnityEngine;
using UnityEditor;
using ShmupBaby;

namespace ShmupEditor
{

    /// <summary>
    /// wrapper for the SerializedProperty of type MissileWeaponData class which helps to deal
	/// with the class directly.
    /// </summary>
    public class SerializedMissileWeaponData
    {
        //relative property for SerializedProperty of type MissileWeaponData
        public SerializedProperty Damage;
        public SerializedProperty Rate;
        public SerializedProperty Speed;
        public SerializedProperty Size;
        public SerializedProperty MissileObject;
        public SerializedProperty MissileScript;
        public SerializedProperty LifeTime;
        public SerializedProperty TurnSpeed;
        public SerializedProperty RandomStartRotation;
        public SerializedProperty DestroyOnTargetLost;
        public SerializedProperty Health;

        /// <summary>
        /// SerializedMissileWeaponData constructor.
        /// </summary>
        /// <param name="SerializedData">SerializedProperty of type MissileWeaponData</param>
        public SerializedMissileWeaponData(SerializedProperty SerializedData)
        {

            Damage = SerializedData.FindPropertyRelative("Damage");
            Rate = SerializedData.FindPropertyRelative("Rate");
            Speed = SerializedData.FindPropertyRelative("Speed");
            Size = SerializedData.FindPropertyRelative("Size");
            MissileObject = SerializedData.FindPropertyRelative("MissileObject");
            MissileScript = SerializedData.FindPropertyRelative("MissileScript");
            LifeTime = SerializedData.FindPropertyRelative("LifeTime");
            TurnSpeed = SerializedData.FindPropertyRelative("TurnSpeed");
            RandomStartRotation = SerializedData.FindPropertyRelative("RandomStartRotation");
            DestroyOnTargetLost = SerializedData.FindPropertyRelative("DestroyOnTargetLost");
            Health = SerializedData.FindPropertyRelative("Health");

        }

        /// <summary>
        /// edits the relative Property for the SerializedProperty of type MissileWeaponData
        /// </summary>
        /// <param name="Data">MissileWeaponData to update the relative Property for the SerializedProperty</param>
        public void UpdateSerializedProperty(MissileWeaponStageData Data)
        {

            Damage.floatValue = Data.Damage;
            Rate.floatValue = Data.Rate;
            Speed.floatValue = Data.Speed;
            Size.floatValue = Data.Size;
            MissileObject.objectReferenceValue = Data.MissileObject;
            MissileScript.objectReferenceValue = Data.MissileScript;
            LifeTime.floatValue = Data.LifeTime;
            TurnSpeed.floatValue = Data.TurnSpeed;
            RandomStartRotation.boolValue = Data.RandomStartRotation;
            DestroyOnTargetLost.boolValue = Data.DestroyOnTargetLost;
            Health.floatValue = Data.Health;

        }

        /// <summary>
        /// return instance of the MissileWeaponStageData that reflects 
        /// the value for its SerializedProperty
        /// </summary>
        /// <returns></returns>
        public MissileWeaponStageData GetMissileWeaponStageData()
        {

            MissileWeaponStageData Data = new MissileWeaponStageData();

            Data.Damage = Damage.floatValue;
            Data.Rate = Rate.floatValue;
            Data.Speed = Speed.floatValue;
            Data.Size = Size.floatValue;
            Data.MissileObject = MissileObject.objectReferenceValue;
            Data.MissileScript = (Missile)MissileScript.objectReferenceValue;
            Data.LifeTime = LifeTime.floatValue;
            Data.TurnSpeed = TurnSpeed.floatValue;
            Data.RandomStartRotation = RandomStartRotation.boolValue;
            Data.DestroyOnTargetLost = DestroyOnTargetLost.boolValue;
            Data.Health = Health.floatValue;

            return Data;

        }

    }

    /// <summary>
    /// draws the stage for the weapon of type Missile.
    /// </summary>
    public static class MissileWeaponInspector
    {

        /// <summary>
        /// container for the selected Visual for the missile.
        /// </summary>
        [SerializeField]
        static ObjectContainer MissileVisualCont;

        /// <summary>
        /// MissileWeaponInspector constructor
        /// </summary>
        static MissileWeaponInspector()
        {

            RestGUI();

        }

        /// <summary>
        /// resets the MissileWeaponInspector to its default state.
        /// </summary>
        public static void RestGUI()
        {

            MissileVisualCont = new ObjectContainer(null);

        }

        /// <summary>
        /// draw MissileWeaponStageData for an inspector.
        /// </summary>
        /// <param name="Data">the stage that needs to be drawn</param>
        /// <param name="thisInspector">the editor that will have the stage drawn to</param>
        public static void DrawStage(MissileWeaponStageData Data, Editor thisInspector)
        {

            GUILayout.Space(10);

            //draw a menu to pick the missile visual from.
            EditorGUILayout.BeginHorizontal();

            if (MissileVisualCont.O != null)
            {
                GameObject missile = Data.MissileObject as GameObject;

                if (missile != null)
                    GameObject.Instantiate(MissileVisualCont.O, missile.transform);

                MissileVisualCont.O = null;
            }

            if (GUILayout.Button("Add Visual", GUILayout.Height(20)))
            {

                PickByIcon Missile_PickWindow = EditorWindow.GetWindow<PickByIcon>(true, "Missiles");

                Missile_PickWindow.WindowsInitialize(MissileVisualCont, "Missiles", "prefab", thisInspector);

            }

            //draw the field for the stage.
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            Data.Damage = EditorGUILayout.FloatField("Damage :", Data.Damage);
            GUILayout.Space(5);
            Data.Speed = EditorGUILayout.FloatField("Speed :", Data.Speed);
            GUILayout.Space(5);
            Data.TurnSpeed = EditorGUILayout.FloatField("TurnSpeed  :", Data.TurnSpeed);
            GUILayout.Space(5);
            Data.Rate = EditorGUILayout.FloatField("Rate :", Data.Rate);
            GUILayout.Space(5);
            Data.LifeTime = EditorGUILayout.FloatField("Missile LifeTime :", Data.LifeTime);


            GUILayout.Space(10);

            //overrides the missile scale when the size is bigger than 0.
            if (Data.Size <= 0)
            {
                if (EditorGUILayout.Toggle("Override Missile Scale", false))
                    Data.Size = 1;
            }
            else
            {
                if (!EditorGUILayout.Toggle("Override Missile Scale", true))
                    Data.Size = 0;

                Data.Size = EditorGUILayout.FloatField("Size :", Data.Size);
            }

            GUILayout.Space(10);

            //the missile will have Immunity to bullets if the health is equal or
            //below zero.
            if (Data.Health <= 0)
            {
                if (EditorGUILayout.Toggle("Missile Take Damage", false))
                    Data.Health = 1;
            }
            else
            {
                if (!EditorGUILayout.Toggle("Missile Take Damage", true))
                    Data.Health = 0;

                Data.Health = EditorGUILayout.FloatField("Missile Health :", Data.Health);
            }

            GUILayout.Space(10);

            Data.RandomStartRotation = EditorGUILayout.Toggle("Random Rotation on Launch", Data.RandomStartRotation);

            GUILayout.Space(10);

            Data.DestroyOnTargetLost = EditorGUILayout.Toggle("Destroy Missile On Target Lost", Data.DestroyOnTargetLost);
        }

    }

}