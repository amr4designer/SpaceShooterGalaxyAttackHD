using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// one of the player weapon types which control a particle system to 
    /// emit its particles as bullets, that get fired radially 
    /// around the weapon.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Weapons/Player/Player Radial Weapon")]
    public class PlayerRadialWeapon : RadialWeapon , IPlayerWeapon {

        /// <summary>
        /// the player settings for every stage.
        /// </summary>
		[Space]
        [Tooltip("list of weapon settings for every stage.")]
		public RadialWeaponStageData[] Stages ;

        /// <summary>
        /// triggered when a bullet from this weapon hit an enemy.
        /// </summary>
		public event ShmupDelegate OnDamageEnemy;
        /// <summary>
        /// triggered when the player current stage change.
        /// </summary>
		public event ShmupDelegate OnStageChanged ;

        /// <summary>
        /// the layer which the bullet will collide with.
        /// </summary>
		protected override string TargetLayer
        {
            get { return this.GetPlayerWeaponTargetLayer(); }
        }
        /// <summary>
		/// the side at which this weapon belong (player or enemy).
        /// </summary>
        public override AgentSide FiringSide
        {
            get { return AgentSide.Player; }
        }

        /// <summary>
        /// the index for the current stage.
        /// </summary>
        // change StageIndex will set the weapon to its index and trigger an on stage change event.
        public int StageIndex {
			get {
				return _stageIndex;
			}
			set {

				if (value >= Stages.Length || value < 0)
					return;

				if (_stageIndex != value) {

					_stageIndex = value;

					SetToStage (CurrentStage);

					RiseOnStageChanged ();

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
	    /// the Start method is one of Unity's messages that gets called when a new object is instantiated.
	    /// </summary>
        protected virtual void Start () {
			SetToStage (CurrentStage);
		}

        /// <summary>
        /// one of Unity's messages that gets called when the particle collide with another object
        /// this need sendCollisionMessages to be enabled inside the Collision module.
        /// </summary>
        /// <param name="hitTarget">the object hit by the bullet.</param>
        protected override void OnParticleCollision (GameObject hitTarget)
		{
			base.OnParticleCollision (hitTarget);

		    Agent hitAgent = hitTarget.GetComponent<Agent>();

            if (!CheckForFriendlyFire(hitAgent))
            {
                hitAgent.TakeDamage(CurrentStage.Damage, DamageSource.Bullet);
                RiseOnDamageEnemy();
            }

        }

        /// <summary>
        /// set the weapon to the next stage.
        /// </summary>
		public void Upgrade () {
			StageIndex++;
		}
        /// <summary>
        /// set the weapon to the previous stage.
        /// </summary>
		public void Downgrade () {
			StageIndex--;
		}

        /// <summary>
        /// handle the rise of OnDamageEnemy.
        /// </summary>
		protected void RiseOnDamageEnemy () {
			if (OnDamageEnemy != null)
				OnDamageEnemy (null);
		}
        /// <summary>
        /// handle the rise of OnStageChanged.
        /// </summary>
		protected void RiseOnStageChanged () {
			if (OnStageChanged != null)
				OnStageChanged (null);
		}

	}

}