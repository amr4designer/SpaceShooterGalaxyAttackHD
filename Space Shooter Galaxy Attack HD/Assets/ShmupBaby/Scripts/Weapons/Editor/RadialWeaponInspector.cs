using UnityEngine;
using UnityEditor;
using ShmupBaby;

namespace ShmupEditor
{
    /// <summary>
    /// wrapper for the SerializedProperty of type RadialWeaponData which class helps to deal with
    /// the class directly.
    /// </summary>
    public class SerializedRadialWeaponData
    {
        //relative property for SerializedProperty of type RadialWeaponData
        public SerializedProperty Damage;
        public SerializedProperty Rate;
        public SerializedProperty Speed;
        public SerializedProperty Size;
        public SerializedProperty ColliderSize;

        public SerializedProperty BulletNum;
        public SerializedProperty ArcLength;
        public SerializedProperty ArcRadius;
        public SerializedProperty ArcShiftAngle;
        public SerializedProperty Distance;

        public SerializedProperty BulletMaterial;

        public SerializedProperty TrailMaterial;
        public SerializedProperty TrailTime;
        public SerializedProperty TrailWidth;

        public SerializedProperty BulletRotationSpeed;
        public SerializedProperty BulletRotationOffset;

        /// <summary>
        /// SerializedRadialWeaponData constructor.
        /// </summary>
        /// <param name="SerializedData">SerializedProperty of type RadialWeaponData</param>
        public SerializedRadialWeaponData(SerializedProperty SerializedData)
        {

            Damage = SerializedData.FindPropertyRelative("Damage");
            Rate = SerializedData.FindPropertyRelative("Rate");
            Speed = SerializedData.FindPropertyRelative("Speed");
            Size = SerializedData.FindPropertyRelative("Size");
            ColliderSize = SerializedData.FindPropertyRelative("ColliderSize");
            BulletNum = SerializedData.FindPropertyRelative("BulletNum");
            ArcLength = SerializedData.FindPropertyRelative("ArcLength");
            ArcRadius = SerializedData.FindPropertyRelative("ArcRadius");
            ArcShiftAngle = SerializedData.FindPropertyRelative("ArcShiftAngle");
            Distance = SerializedData.FindPropertyRelative("Distance");
            BulletMaterial = SerializedData.FindPropertyRelative("BulletMaterial");
            TrailMaterial = SerializedData.FindPropertyRelative("TrailMaterial");
            TrailTime = SerializedData.FindPropertyRelative("TrailTime");
            TrailWidth = SerializedData.FindPropertyRelative("TrailWidth");
            BulletRotationSpeed = SerializedData.FindPropertyRelative("BulletRotationSpeed");
            BulletRotationOffset = SerializedData.FindPropertyRelative("BulletRotationOffset");

        }

        /// <summary>
        /// edits the relative Property for the SerializedProperty of type RadialWeaponData
        /// </summary>
        /// <param name="Data">RadialWeaponData to update the relative Property for the SerializedProperty</param>
        public void UpdateSerializedProperty(RadialWeaponStageData Data)
        {

            Damage.floatValue = Data.Damage;
            Rate.floatValue = Data.Rate;
            Speed.floatValue = Data.Speed;
            Size.floatValue = Data.Size;
            ColliderSize.floatValue = Data.ColliderSize;
            BulletNum.intValue = Data.BulletNum;
            ArcLength.floatValue = Data.ArcLength;
            ArcRadius.floatValue = Data.ArcRadius;
            ArcShiftAngle.floatValue = Data.ArcShiftAngle;
            Distance.floatValue = Data.Distance;
            BulletMaterial.objectReferenceValue = Data.BulletMaterial;
            TrailMaterial.objectReferenceValue = Data.TrailMaterial;
            TrailTime.floatValue = Data.TrailTime;
            TrailWidth.floatValue = Data.TrailWidth;
            BulletRotationSpeed.floatValue = Data.BulletRotationSpeed;
            BulletRotationOffset.floatValue = Data.BulletRotationOffset;

        }

        /// <summary>
        /// return instance of the RadialWeaponStageData that reflects 
        /// the value for its SerializedProperty
        /// </summary>
        /// <returns></returns>
        public RadialWeaponStageData GetWeaponStageData()
        {

            RadialWeaponStageData Data = new RadialWeaponStageData();

            Data.Damage = Damage.floatValue;
            Data.Rate = Rate.floatValue;
            Data.Speed = Speed.floatValue;
            Data.Size = Size.floatValue;
            Data.ColliderSize = ColliderSize.floatValue;
            Data.BulletNum = BulletNum.intValue;
            Data.ArcLength = ArcLength.floatValue;
            Data.ArcRadius = ArcRadius.floatValue;
            Data.ArcShiftAngle = ArcShiftAngle.floatValue;
            Data.Distance = Distance.floatValue;
            Data.BulletMaterial = BulletMaterial.objectReferenceValue;
            Data.TrailMaterial = TrailMaterial.objectReferenceValue;
            Data.TrailTime = TrailTime.floatValue;
            Data.TrailWidth = TrailWidth.floatValue;
            Data.BulletRotationSpeed = BulletRotationSpeed.floatValue;
            Data.BulletRotationOffset = BulletRotationOffset.floatValue;

            return Data;

        }

    }

    /// <summary>
    /// Draws the stage for the weapon of type radial.
    /// </summary>
    public static class RadialWeaponInspector
    {
        /// <summary>
        /// controls the Arc Tab if it's open or closed.
        /// </summary>
        [SerializeField]
        static bool ArcTab;
        /// <summary>
        /// controls the Trail Tab if it's open or closed.
        /// </summary>
        [SerializeField]
        static bool TrailTab;

        /// <summary>
        /// container for the selected material of the bullet.
        /// </summary>
        [SerializeField]
        static ObjectContainer BulletMaterialCont;
        /// <summary>
        /// container for the selected material of the trail.
        /// </summary>
        [SerializeField]
        static ObjectContainer TrailMaterialCont;

        /// <summary>
        /// RadialWeaponInspector constructor
        /// </summary>
        static RadialWeaponInspector()
        {

            RestGUI();

        }

        /// <summary>
        /// reset the RadialWeaponInspector to its default state.
        /// </summary>
        public static void RestGUI()
        {

            ArcTab = false;
            TrailTab = false;

            BulletMaterialCont = new ObjectContainer(null);
            TrailMaterialCont = new ObjectContainer(null);

        }

        /// <summary>
        /// draw RadialWeaponStageData for an inspector.
        /// </summary>
        /// <param name="Data">the stage that needs to be drawn</param>
        /// <param name="thisInspector">the editor that will have the stage drawn to</param>
        public static void DrawStage(RadialWeaponStageData Data, Editor thisInspector)
        {

            GUILayout.Space(10);

            //draw a a menu to pick the bullet material from.
            EditorGUILayout.BeginHorizontal();

            if (BulletMaterialCont.O != null)
                Data.BulletMaterial = BulletMaterialCont.O;

            if (GUILayout.Button("Pick Bullet", GUILayout.Height(20)))
            {

                PickByIcon Bullet_PickWindow = EditorWindow.GetWindow<PickByIcon>(true, "Picking Bullet");

                Bullet_PickWindow.WindowsInitialize(BulletMaterialCont, "bullets", "mat", thisInspector);

            }

            BulletMaterialCont.O = EditorGUILayout.ObjectField("", Data.BulletMaterial, typeof(Material), false);

            EditorGUILayout.EndHorizontal();

            //draw the field for the stage.
            GUILayout.Space(5);
            Data.Damage = EditorGUILayout.FloatField("Damage :", Data.Damage);
            GUILayout.Space(5);
            Data.Speed = EditorGUILayout.FloatField("Speed :", Data.Speed);
            GUILayout.Space(5);
            Data.Rate = EditorGUILayout.FloatField("Emission  Rate :", Data.Rate);
            GUILayout.Space(5);
            Data.Size = EditorGUILayout.FloatField("Size :", Data.Size);
            GUILayout.Space(5);
            Data.ColliderSize = EditorGUILayout.FloatField("Collider Size :", Data.ColliderSize);
            GUILayout.Space(5);
            Data.BulletRotationSpeed = EditorGUILayout.FloatField("Rotation Speed :", Data.BulletRotationSpeed);
            GUILayout.Space(5);
            Data.BulletRotationOffset = EditorGUILayout.FloatField("Rotation Offset :", Data.BulletRotationOffset);

            //draw the field for the arc in the arc tab.
            GUILayout.Space(20);

            ArcTab = GUILayout.Toggle(ArcTab, "Arc", GUI.skin.button, GUILayout.Height(25));

            if (ArcTab)
            {

                GUILayout.Space(10);

                Data.BulletNum = EditorGUILayout.IntField("Bullets Number :", Data.BulletNum);
                GUILayout.Space(5);
                Data.ArcLength = EditorGUILayout.FloatField("Arc Length :", Data.ArcLength);
                GUILayout.Space(5);
                Data.ArcRadius = EditorGUILayout.FloatField("Arc Radius :", Data.ArcRadius);
                GUILayout.Space(5);
                Data.ArcShiftAngle = EditorGUILayout.FloatField("Arc Shift Angle :", Data.ArcShiftAngle);
                GUILayout.Space(5);
                Data.Distance = EditorGUILayout.FloatField("Distance :", Data.Distance);

                GUILayout.Space(10);

            }

            //draw the field for the trail in the trail tab.
            GUILayout.Space(10);

            TrailTab = GUILayout.Toggle(TrailTab, "Trail", GUI.skin.button, GUILayout.Height(25));

            if (TrailTab)
            {

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();

                if (TrailMaterialCont.O != null)
                    Data.TrailMaterial = TrailMaterialCont.O;

                //draw a a menu to pick the trail material from.
                if (GUILayout.Button("Pick Trail", GUILayout.Height(20)))
                {
                    PickByIcon Trail_PickWindow = EditorWindow.GetWindow<PickByIcon>(true, "Picking Trail");
                    Trail_PickWindow.WindowsInitialize(TrailMaterialCont, "trails", "mat", thisInspector);
                }

                TrailMaterialCont.O = EditorGUILayout.ObjectField("", Data.TrailMaterial, typeof(Material), false);

                GUILayout.EndHorizontal();

                if (GUILayout.Button("Remove Trail", GUILayout.Height(20)))
                {
                    Data.TrailMaterial = null;
                    TrailMaterialCont.O = null;
                }

                GUILayout.Space(5);
                Data.TrailTime = EditorGUILayout.FloatField("Trail Time :", Data.TrailTime);
                GUILayout.Space(5);
                Data.TrailWidth = EditorGUILayout.FloatField("Trail Width :", Data.TrailWidth);

                GUILayout.Space(10);
            }

            GUILayout.Space(10);

        }

        /// <summary>
        /// draw a gizmo for a given stage.
        /// </summary>
        /// <param name="Data">the stage that needs to be drawn</param>
        /// <param name="transform">the transform of the stage editor</param>
        public static void DrawHandle(RadialWeaponStageData Data, ParticleSystem ps)
        {
            if (Data == null)
                return;

            Handles.color = Color.yellow;

            //draws the arc that the bullet will be emitting from.
            Vector3 arcStartingPoint = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (Data.ArcShiftAngle + ps.transform.eulerAngles.z)), Mathf.Sin(Mathf.Deg2Rad * (Data.ArcShiftAngle + ps.transform.eulerAngles.z)), 0);
            Handles.DrawWireArc(ps.transform.position, Vector3.forward, arcStartingPoint, Data.ArcLength, Data.ArcRadius);

            Handles.color = Color.yellow;

            //the angle between each bullet.
            float stepAngle = 0;
            
            if (Data.BulletNum <= 1)
                stepAngle = 0;
            else
            {
                if (Data.ArcLength % 360f != 0)
                    stepAngle = Data.ArcLength / (Data.BulletNum - 1);
                else
                    stepAngle = Data.ArcLength / Data.BulletNum;
            }

            //draw the bullets paths
            for (int i = 0; i < Data.BulletNum; i++)
            {

                float Angle = (i * stepAngle + Data.ArcShiftAngle + ps.transform.eulerAngles.z) * Mathf.Deg2Rad;
                Vector3 Pos = new Vector3(Mathf.Cos(Angle), Mathf.Sin(Angle), 0);
                Handles.DrawDottedLine(Pos * Data.ArcRadius + ps.transform.position, Pos * (Data.Distance + Data.ArcRadius) + ps.transform.position, 5);
            }

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[100];
            ps.GetParticles(particles);

            for (int i = 0; i < particles.Length; i++)
            {
                Handles.DrawWireArc(particles[i].position , Vector3.forward, Vector3.right, 360f, Data.ColliderSize*0.5f*Data.Size);
            }

        }

    }

}
