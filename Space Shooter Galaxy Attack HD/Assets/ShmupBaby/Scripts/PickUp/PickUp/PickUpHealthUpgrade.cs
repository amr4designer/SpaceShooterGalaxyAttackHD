using UnityEngine;

namespace ShmupBaby
{

    /// <summary>
    /// PickUp item that player can pick to extend his max health.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Pick Up/Pick Up Health Upgrade")]
    public class PickUpHealthUpgrade : PickUp
    {
        /// <summary>
        /// a number that will be added to the player max health OnPick.
        /// </summary>
        [Tooltip("how many health point the player max health will be extend by when he pick this pickup")]
        [Space]
        public float HealthUpgradeAmount;

        /// <summary>
        /// percentage of the player max health that will be added to the player max health OnPick.
        /// </summary>
        [Tooltip("percentage of the player max health that will be added to the player max health" +
                 " when he pick this pickup")]
        [Space]
        public float HealthUpgradePercentage;

        /// <summary>
        /// refilling the player health bar after upgrading the health.
        /// </summary>
        [Tooltip(" refilling the player health bar after upgrading the health.")]
        [Space]
        public bool RefillHealth;

        /// <summary>
        /// rescale the player health bar after upgrading the health.
        /// </summary>
        [Space]
        [Tooltip(" rescale the player health bar after upgrading the health.")]
        public bool RescaleHealth;

        /// <summary>
        /// max health upgrading limit 
        /// </summary>
        [Tooltip("limit the max health from exceeding this value, set it to zero mean there is no limit.")]
        public float MaxHealthLimit = 0;

        protected override void PickUpEffect()
        {

            //calculate the health upgrade amount.
            float healthUpgrade = HealthUpgradeAmount + (HealthUpgradePercentage / 100f) * target.MaxHealth;

            if (MaxHealthLimit != 0)
            {
                if (target.MaxHealth + healthUpgrade >= MaxHealthLimit)
                    healthUpgrade = 0;
            }

            //cash the player health percentage before the upgrade.
            float healthPercentage = target.CurrentHealth / target.MaxHealth;

            target.maxHealth += healthUpgrade;

            //rescale and refill the health if needed
            if (RescaleHealth)
                target.CurrentHealth = healthPercentage * target.MaxHealth;

            if (RefillHealth)
                target.CurrentHealth = target.MaxHealth;

            //rise the OnPick event for the player.
            target.RiseOnPickUp(PickUpType.HealthUpgrade);

        }

    }

}
