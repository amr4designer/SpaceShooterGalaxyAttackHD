using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// One of the enemy weapon types which controls a particle system to 
    /// emit its particles as bullets, this will have a curve to control
    /// its velocity.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Weapons/Enemy/Enemy Path Weapon")]
    public class EnemyPathWeapon : PathWeapon , IEnemyWeapon {

        /// <summary>
        /// the enemy weapon settings.
        /// </summary>
        [Space]
        [Tooltip("the bullet fire settings for this weapon.")]
        public PathWeaponStageData Settings ;

        /// <summary>
        /// the weapon settings for the current stage.
        /// </summary>
        public override WeaponStageData CurrentStage
        {
            get
            {
                return Settings;
            }
        }

        /// <summary>
        /// the layer which the bullet will collide with.
        /// </summary>
        protected override string TargetLayer
		{
		    get
            {
                return this.GetEnemyWeaponTargetLayer();
            }
		}
        /// <summary>
		/// the side at which this weapon belong (player or enemy).
        /// </summary>
	    public override AgentSide FiringSide
	    {
	        get
            {
                return AgentSide.Enemy;
            }
	    }

        /// <summary>
        /// one of Unity's messages which is called when the particles collide with another object
        /// this needs sendCollisionMessages to be enabled inside the Collision module.
        /// </summary>
        /// <param name="hitTarget">the object hit by the bullet.</param>
        protected override void OnParticleCollision (GameObject hitTarget)
		{
			base.OnParticleCollision (hitTarget);

		    Agent hitAgent = hitTarget.GetComponent<Agent>();

            if (!CheckForFriendlyFire(hitAgent))
                hitAgent.TakeDamage(CurrentStage.Damage, DamageSource.Bullet);

        }

	}

}