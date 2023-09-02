using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// One of the player weapons type, which controls a particle system to emit
    /// its particles as bullets,that gets fired radially around
    /// the weapon.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Weapons/Enemy/Enemy Radial Weapon")]
    public class EnemyRadialWeapon : RadialWeapon, IEnemyWeapon
    {
        /// <summary>
        /// the enemy weapon settings.
        /// </summary>
        [Space]
        [Tooltip("the bullet fire settings for this weapon.")]
        public RadialWeaponStageData Settings;

        /// <summary>
        /// the weapon settings for the current stage.
        /// </summary>
        public override WeaponStageData CurrentStage
        {
            get { return Settings; }
        }

        /// <summary>
        /// the layer which the bullet will collide with.
        /// </summary>
        protected override string TargetLayer
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
        /// one of Unity's messages which gets called when the particle collide with another object
        /// this needs sendCollisionMessages to be enabled inside the Collision module.
        /// </summary>
        /// <param name="hitTarget">the object hit by the bullet.</param>
        protected override void OnParticleCollision(GameObject hitTarget)
        {
            base.OnParticleCollision(hitTarget);

            Agent hitAgent = hitTarget.GetComponent<Agent>();

            if (!CheckForFriendlyFire(hitAgent))
                hitAgent.TakeDamage(CurrentStage.Damage, DamageSource.Bullet);

        }

    }

}