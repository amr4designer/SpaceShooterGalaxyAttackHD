using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// one of the player weapon type that work as missile luncher.
    /// the weapon.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Weapons/Player/Player Missile Weapon")]
    public class PlayerMissileWeapon : MissileWeapon, IPlayerWeapon
    {

        /// <summary>
        /// the player settings for every stage.
        /// </summary>
		[Space]
        [Tooltip("list of weapon settings for every stage.")]
        public MissileWeaponStageData[] Stages;

        /// <summary>
        /// trigger when a missile from this weapon hit an enemy.
        /// </summary>
        public event ShmupDelegate OnDamageEnemy;
        /// <summary>
        /// trigger when the player current stage change.
        /// </summary>
        public event ShmupDelegate OnStageChanged;

        /// <summary>
        /// the layer which the bullet will collide with.
        /// </summary>
        protected string TargetLayer
        {
            get { return this.GetPlayerWeaponTargetLayer(); }
        }
        /// <summary>
        /// the side at which this weapon belong.
        /// </summary>
        public override AgentSide FiringSide
        {
            get { return AgentSide.Player; }
        }

        /// <summary>
        /// the index for the current stage.
        /// </summary>
        // change StageIndex will set the weapon to it's index and trigger on stage change event.
        public int StageIndex
        {
            get
            {
                return _stageIndex;
            }
            set
            {

                if (value >= Stages.Length || value < 0)
                    return;

                if (_stageIndex != value)
                {

                    _stageIndex = value;

                    SetToStage(CurrentStage);

                    RiseOnStageChanged();

                }
            }
        }

        /// <summary>
        /// the weapon settings for the current stage.
        /// </summary>
        public override WeaponStageData CurrentStage
        {
            get { return Stages[StageIndex]; }
        }

        /// <summary>
        /// back-end field for the StageIndex.
        /// </summary>
        private int _stageIndex;

        /// <summary>
	    /// the Start method one of Unity message that get called when a new object is instantiated.
	    /// </summary>
        protected virtual void Start()
        {
            SetToStage(CurrentStage);
        }

        /// <summary>
        /// called in awake Initialize the weapon settings.
        /// </summary>
        [ContextMenu("Initialize")]
        public override void Initialize()
        {
            if (Stages == null)
                return;

            //Initialize a missile for each stage
            for (int i = 0; i < Stages.Length; i++)
            {
                MissileWeaponStageData stageData = Stages[i];

                InitializeMissile(i + 1, stageData);
            }
            
            Initialized = true;
        }

        /// <summary>
        /// create a missile instance that match the given stagesData
        /// and place it under the missile weapon.
        /// </summary>
        /// <param name="num">the number of the stage to name the missile</param>
        /// <param name="stagesData">the data for the missile stage.</param>
        protected override void InitializeMissile(int num, MissileWeaponStageData stageData)
        {
            base.InitializeMissile(num, stageData);

            //set the missile to be player missile.
            GameObject missile = (GameObject)stageData.MissileObject;
            missile.layer = LayerMask.NameToLayer("Player");
            stageData.MissileScript.MyMover.targetOption = TargetOption.RandomEnemy;

        }

        /// <summary>
        /// set the weapon to the next stage.
        /// </summary>
		public void Upgrade()
        {
            StageIndex++;
        }
        /// <summary>
        /// set the weapon to the previous stage.
        /// </summary>
		public void Downgrade()
        {
            StageIndex--;
        }

        /// <summary>
        /// handle the rise of OnDamageEnemy.
        /// </summary>
		protected void RiseOnDamageEnemy()
        {
            if (OnDamageEnemy != null)
                OnDamageEnemy(null);
        }
        /// <summary>
        /// handle the rise of OnStageChanged.
        /// </summary>
		protected void RiseOnStageChanged()
        {
            if (OnStageChanged != null)
                OnStageChanged(null);
        }

    }

}
