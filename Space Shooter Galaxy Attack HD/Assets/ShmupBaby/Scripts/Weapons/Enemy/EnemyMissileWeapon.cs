using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// One of the enemy weapon types that work as a missile luncher.
    /// the weapon.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Weapons/Enemy/Enemy Missile Weapon")]
    public class EnemyMissileWeapon : MissileWeapon , IEnemyWeapon
    {
        /// <summary>
        /// The enemy weapons settings.
        /// </summary>
        [Space]
        [Tooltip("The bullet fire settings for this weapon.")]
        public MissileWeaponStageData Settings;

        /// <summary>
        /// the weapon settings for the current stage.
        /// </summary>
        public override WeaponStageData CurrentStage
        {
            get { return Settings; }
        }
        /// <summary>
        /// the layer which the missiles will collide with.
        /// </summary>
        protected string TargetLayer
        {
            get { return this.GetEnemyWeaponTargetLayer(); }
        }
        /// <summary>
		/// the side at which this weapon belong (player or enemy).
        /// </summary>
        public override AgentSide FiringSide
        {
            get { return AgentSide.Enemy; }
        }

        /// <summary>
        /// target agent for the missiles.
        /// </summary>
        public Agent Target
        {
            get { return LevelController.Instance.PlayerComponent; }
        }

        /// <summary>
        /// called in awake to Initialize the weapon settings.
        /// </summary>
        [ContextMenu("Initialize")]
        public override void Initialize()
        {
            MissileWeaponStageData stageData = CurrentStage as MissileWeaponStageData;

            InitializeMissile(1,stageData);

            Initialized = true;
        }

        /// <summary>
        /// create a missile instance that matches the given stagesData
        /// and place it under the missile weapon.
        /// </summary>
        /// <param name="num">the number of the stage to name the missile</param>
        /// <param name="stagesData">the data for the missile stage.</param>
        protected override void InitializeMissile(int num, MissileWeaponStageData stageData)
        {
            base.InitializeMissile(num, stageData);

            //set the missile to be enemy missile.
            GameObject missile = (GameObject)stageData.MissileObject;
            missile.layer = LayerMask.NameToLayer("Enemy");
            stageData.MissileScript.MyMover.targetOption = TargetOption.Player;

        }

    }
}