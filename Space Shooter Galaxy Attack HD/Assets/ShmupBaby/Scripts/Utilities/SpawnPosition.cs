using UnityEngine;

namespace ShmupBaby
{

    /// <summary>
    /// Position a target transform to a relative position to the gameField.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Utilities/Spawn Position")]
    public class SpawnPosition : MonoBehaviour
    {
        /// <summary>
        /// the target that need to be positioned.
        /// </summary>
        [Tooltip("the target that need to be position, leave it empty to use this gameObject.")]
        public Transform Target;
        /// <summary>
        /// the x position of the target relative to the gameField.
        /// </summary>
        [Header("Spawn Position")]
        [Space]
        [Range(0, 1f)]
        [Tooltip("position in the x direction relative to the gameField.")]
        public float PositionX;
        /// <summary>
        /// the y position of the target relative to the gameField.
        /// </summary>
        [Range(0, 1f)]
        [Tooltip("position in the y direction relative to the gameField.")]
        public float PositionY;
        /// <summary>
        /// the index of the target, represent the target position in the z axis.
        /// </summary>
        [Tooltip("the index of target, represent the target position in the z axis.")]
        [Range(0, 100)]
        public int LayerIndex = 0;

        /// <summary>
        /// game field for this level.
        /// </summary>
        private Rect GameField
        {
            get { return LevelController.Instance.GameField; }
        }
        /// <summary>
        /// space between this level layers.
        /// </summary>
        private float SpaceBetween
        {
            get { return LevelController.Instance.SpaceBetween; }
        }

        /// <summary>
        /// the Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
        void Start()
        {
            if (Target == null)
                Target = transform;

            //position the target relative to the gameField.
            Target.position = new Vector3(
                Mathf.Lerp(GameField.xMin, GameField.xMax, PositionX),
                Mathf.Lerp(GameField.yMin, GameField.yMax, PositionY),
                SpaceBetween * LayerIndex);

			//Activates the game object if it was disabled.
			Target.gameObject.SetActive (true);
        }

        #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            BackgroundController bgController = FindObjectOfType<BackgroundController>();

            if (bgController == null)
                return;

            Rect backgroundRect = bgController.BackgroundForGizmo;

            Vector2 spawnPosition = new Vector2(Mathf.Lerp(backgroundRect.xMin, backgroundRect.xMax, PositionX),
                Mathf.Lerp(backgroundRect.yMin, backgroundRect.yMax, PositionY));

            Gizmos.color = new Color(1,0.6f,0.6f,1);

            GizmosExtension.DrawCircle(spawnPosition, 2f);

        }

        #endif

    }

}