using UnityEditor;
using UnityEngine;
using ShmupBaby;

namespace ShmupEditor
{
    /// <summary>
    /// wrapper for SerializedProperty of type PathWeaponData class which helps to deal
	/// with the class directly.
    /// </summary>
    public class SerializedPathWeaponData
    {
        //relative property for SerializedProperty of type PathWeaponData
        public SerializedProperty Damage;
        public SerializedProperty Rate;
        public SerializedProperty Speed;
        public SerializedProperty Size;
        public SerializedProperty ColliderSize;

        public SerializedProperty Lifetime;
        public SerializedProperty Curve;
        public SerializedProperty CurveRange;

        public SerializedProperty BulletMaterial;

        public SerializedProperty TrailMaterial;
        public SerializedProperty TrailTime;
        public SerializedProperty TrailWidth;

        public SerializedProperty BulletRotationSpeed;
        public SerializedProperty BulletRotationOffset;

        /// <summary>
        /// SerializedPathWeaponData constructor.
        /// </summary>
        /// <param name="SerializedData">SerializedProperty of type PathWeaponData</param>
        public SerializedPathWeaponData(SerializedProperty SerializedData)
        {
            Damage = SerializedData.FindPropertyRelative("Damage");
            Rate = SerializedData.FindPropertyRelative("Rate");
            Speed = SerializedData.FindPropertyRelative("Speed");
            Size = SerializedData.FindPropertyRelative("Size");
            ColliderSize = SerializedData.FindPropertyRelative("ColliderSize");
            Lifetime = SerializedData.FindPropertyRelative("Lifetime");
            Curve = SerializedData.FindPropertyRelative("Curve");
            CurveRange = SerializedData.FindPropertyRelative("CurveRange");
            BulletMaterial = SerializedData.FindPropertyRelative("BulletMaterial");
            TrailMaterial = SerializedData.FindPropertyRelative("TrailMaterial");
            TrailTime = SerializedData.FindPropertyRelative("TrailTime");
            TrailWidth = SerializedData.FindPropertyRelative("TrailWidth");
            BulletRotationSpeed = SerializedData.FindPropertyRelative("BulletRotationSpeed");
            BulletRotationOffset = SerializedData.FindPropertyRelative("BulletRotationOffset");
        }

        /// <summary>
        /// edit the relative Property for the SerializedProperty of type PathWeaponData
        /// </summary>
        /// <param name="Data">PathWeaponData to update the relative Property for the SerializedProperty</param>
        public void UpdateSerializedProperty(PathWeaponStageData Data)
        {
            Damage.floatValue = Data.Damage;
            Rate.floatValue = Data.Rate;
            Speed.floatValue = Data.Speed;
            Size.floatValue = Data.Size;
            ColliderSize.floatValue = Data.ColliderSize;
            Lifetime.floatValue = Data.Lifetime;
            Curve.animationCurveValue = Data.Curve;
            CurveRange.floatValue = Data.CurveRange;
            BulletMaterial.objectReferenceValue = Data.BulletMaterial;
            TrailMaterial.objectReferenceValue = Data.TrailMaterial;
            TrailTime.floatValue = Data.TrailTime;
            TrailWidth.floatValue = Data.TrailWidth;
            BulletRotationSpeed.floatValue = Data.BulletRotationSpeed;
            BulletRotationOffset.floatValue = Data.BulletRotationOffset;
        }

        /// <summary>
        /// return instance of the PathWeaponStageData that reflects 
        /// the value for its SerializedProperty
        /// </summary>
        /// <returns></returns>
        public PathWeaponStageData GetWeaponStageData()
        {
            PathWeaponStageData Data = new PathWeaponStageData();

            Data.Damage = Damage.floatValue;
            Data.Rate = Rate.floatValue;
            Data.Speed = Speed.floatValue;
            Data.Size = Size.floatValue;
            Data.ColliderSize = ColliderSize.floatValue;
            Data.Lifetime = Lifetime.floatValue;
            Data.Curve = Curve.animationCurveValue;
            Data.CurveRange = CurveRange.floatValue;
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
    /// draws the stage for the weapon of type path.
    /// </summary>
    public static class PathWeaponInspector
    {
        /// <summary>
        /// control the Curve Tab if it's open or closed.
        /// </summary>
        [SerializeField]
        static bool CurveTab;
        /// <summary>
        /// control the Trail Tab if it's open or closed.
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
        /// PathWeaponInspector constructor
        /// </summary>
        static PathWeaponInspector()
        {
            RestGUI();
        }

        /// <summary>
        /// resets the PathWeaponInspector to its default state.
        /// </summary>
        public static void RestGUI()
        {
            CurveTab = false;
            TrailTab = false;

            BulletMaterialCont = new ObjectContainer(null);
            TrailMaterialCont = new ObjectContainer(null);
        }

        /// <summary>
        /// draws PathWeaponStageData for an inspector.
        /// </summary>
        /// <param name="Data">the stage that need to be drawn</param>
        /// <param name="thisInspector">the editor that will have the stage drawn to</param>
        public static void DrawStage(PathWeaponStageData Data, Editor thisInspector)
        {

            GUILayout.Space(10);

            //draws a a menu to pick the bullet material from.
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

            //draws the field for the stage.
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
            Data.Lifetime = EditorGUILayout.FloatField("Bullet Lifetime :", Data.Lifetime);
            GUILayout.Space(5);
            Data.BulletRotationSpeed = EditorGUILayout.FloatField("Rotation Speed :", Data.BulletRotationSpeed);
            GUILayout.Space(5);
            Data.BulletRotationOffset = EditorGUILayout.FloatField("Rotation Offset :", Data.BulletRotationOffset);

            //draws the field for the curve in the arc tab.
            GUILayout.Space(20);

            CurveTab = GUILayout.Toggle(CurveTab, "Curve", GUI.skin.button, GUILayout.Height(25));

            if (CurveTab)
            {

                GUILayout.Space(10);

                Data.Curve = EditorGUILayout.CurveField(Data.Curve, Color.cyan, new Rect(0, -1, 1, 2), GUILayout.Height(30));
                GUILayout.Space(5);
                Data.CurveRange = EditorGUILayout.FloatField("Curve Range :", Data.CurveRange);

                GUILayout.Space(10);

            }

            //draws the field for the trail in the trail tab.
            GUILayout.Space(10);

            TrailTab = GUILayout.Toggle(TrailTab, "Trail", GUI.skin.button, GUILayout.Height(25));

            if (TrailTab)
            {
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();

                if (TrailMaterialCont.O != null)
                    Data.TrailMaterial = TrailMaterialCont.O;

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
        /// draws a gizmo for a given stage.
        /// </summary>
        /// <param name="Data">the stage that needs to be drawn</param>
        /// <param name="transform">the transform of the stage editor</param>
        public static void DrawHandle(PathWeaponStageData Data, ParticleSystem ps)
        {
            if (Data == null)
            {
                return;
            }              

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[100];
            ps.GetParticles(particles);

            for (int i = 0; i < particles.Length; i++)
            {
                Handles.DrawWireArc(particles[i].position, Vector3.forward, Vector3.right, 360f, Data.ColliderSize * 0.5f * Data.Size);
            }
        }

    }
}
	