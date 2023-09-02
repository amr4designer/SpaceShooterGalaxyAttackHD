using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShmupBaby
{

    /// <summary>
    /// Shield Upgrade PickUps are item that the player can pick to extend it's maximum shield.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Pick Up/Pick Up Shield Upgrade")]
    public class PickUpShieldUpgrade : PickUp
    {
        /// <summary>
        /// A number that will be added to the player maximum shield value OnPick.
        /// </summary>
        [Tooltip("How many shield point the player maximum shield will be extended by when this item is picked")]
        [Space]
        public float ShieldUpgradeAmount;

        /// <summary>
        /// Percentage of the player maximum shield that will be added to the player shield OnPick.
        /// </summary>
        [Tooltip("Percentage of the player shield that will be added to the player maximum shield" +
                 " when this pickup item is picked")]
        [Space]
        public float ShieldUpgradePercentage;

        /// <summary>
        /// Refilling the player shield bar after upgrading the shield.
        /// </summary>
        [Tooltip(" Refilling the player shield bar after upgrading the shield.")]
        [Space]
        public bool RefillShield;
        /// <summary>
        /// Rescale the player shield bar after upgrading the shield.
        /// </summary>
        [Space]
        [Tooltip(" Rescale the player shield bar after upgrading the shield.")]
        public bool RescaleShield;

        /// <summary>
        /// Max shield upgrading limit 
        /// </summary>
        [Tooltip("Limit the max shield from exceeding this value, setting it to zero means there is no limit.")]
        public float MaxShieldLimit = 0;

        protected override void PickUpEffect()
        {
            //Calculates the shield upgrade amount.
            float shieldUpgrade = ShieldUpgradeAmount + (ShieldUpgradePercentage / 100f) * target.MaxShield;

            if (MaxShieldLimit != 0)
            {
                if (target.MaxHealth + shieldUpgrade >= MaxShieldLimit)
                    shieldUpgrade = 0;
            }

            //Caches the player shield percentage before the upgrade.
            float shieldPercentage = target.CurrentShield/ target.MaxShield;

            target.MaxShield += ShieldUpgradeAmount;

            //Rescales and refills the shield if needed
            if (RescaleShield)
                target.CurrentShield = shieldPercentage * target.MaxShield;

            if (RefillShield)
                target.CurrentShield = target.MaxShield;

            //Raises the OnPick event for the player.
            target.RiseOnPickUp(PickUpType.ShieldUpgrade);

        }

    }

}
