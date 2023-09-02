using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// one of the player weapon types which controls a particle system to 
    /// emit its particles as bullets, that will have a curve to control
    /// its velocity.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Weapons/Player/Player Path Weapon")]
    public class PlayerPathWeapon : PathWeapon , IPlayerWeapon
    {
        /// <summary>
        /// the player settings for every stage.
        /// </summary>
        [Space]
        [Tooltip("list of weapon settings for every stage.")]
        public PathWeaponStageData[] Stages;

        /// <summary>
        /// trigger when a bullet from this weapon hit an enemy.
        /// </summary>
		public event ShmupDelegate OnDamageEnemy;
        /// <summary>
        /// trigger when the player current stage change.
        /// </summary>
		public event ShmupDelegate OnStageChanged;

        /// <summary>
        /// the layer which the bullet will collide with.
        /// </summary>
		protected override string TargetLayer
        {
            get
            {
                return this.GetPlayerWeaponTargetLayer();
            }
        }

        /// <summary>
        /// the index for the current stage.
        /// </summary>
        // change StageIndex will set the weapon to its index and trigger a stage change event.
        public int StageIndex
        {
			get
            {
				return _stageIndex;
			}
			set
            {				
				if (value >= Stages.Length || value < 0)
                {
                    return;
                }					
				
				if (_stageIndex != value)
                {
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
            get
            {
                return Stages[StageIndex];
            }
        }

        /// <summary>
        /// back-end field for the StageIndex.
        /// </summary>
        private int _stageIndex;

        /// <summary>
	    /// the Start method is one of Unity's messages that gets called when a new object is instantiated.
	    /// </summary>
        protected virtual void Start ()
        {
			SetToStage (CurrentStage);
		}

        /// <summary>
        /// one of unity messages which is called when the particle collide with another object
        /// this needs sendCollisionMessages to be enabled inside the Collision module.
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
		public void Upgrade ()
        {
			StageIndex++;
		}
        /// <summary>
        /// set the weapon to the previous stage.
        /// </summary>
		public void Downgrade ()
        {
			StageIndex--;
		}

        /// <summary>
        /// handle the rise of OnDamageEnemy.
        /// </summary>
        protected void RiseOnDamageEnemy()
        {
            if (OnDamageEnemy != null)
            {
                OnDamageEnemy(null);
            }                
        }
        /// <summary>
        /// handle the rise of OnStageChanged.
        /// </summary>
		protected void RiseOnStageChanged()
        {
            if (OnStageChanged != null)
            {
                OnStageChanged(null);
            }               
        }

    }

}