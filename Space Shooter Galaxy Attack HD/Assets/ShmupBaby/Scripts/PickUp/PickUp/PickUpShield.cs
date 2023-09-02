using UnityEngine;

namespace ShmupBaby
{

    /// <summary>
    /// PickUp item that the player can pick to provide refilling of the shield.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Pick Up/Pick Up Shield")]
    public class PickUpShield : PickUp
    {
        /// <summary>
        /// A number that will be added to the player shield OnPick.
        /// </summary>
        [Tooltip("How many shield points the player will get when this item is picked")]
        [Space]
        public float ShieldAmount;

        /// <summary>
        /// Percentage of the the player maximum shield that will be added to the player shield OnPick.
        /// </summary>
        [Tooltip("Percentage of the player maximum shield that will be added to the player shield" +
                 " when this item is picked")]
        [Space]
        [Range(0, 100f)]
        public float ShieldPercentage;

        protected override void PickUpEffect()
        {
            //The amount of shield points for the player.
            float shield = ShieldAmount + (ShieldPercentage / 100f) * target.MaxShield;

            //Make sure the the amount doesn't exceed the maximum shield.
            if (target.CurrentShield + shield >= target.MaxShield)
                target.CurrentShield = target.MaxShield;
            else
                target.CurrentShield += shield;

            //Raise OnPick event for the player.
            target.RiseOnPickUp(PickUpType.Shield);

        }

    }

}
