using UnityEngine;

namespace ShmupBaby
{

    /// <summary>
    /// PickUp item that the player can pick to provide healing.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Pick Up/Pick Up Heal")]
    public class PickUpHeal : PickUp
    {
        /// <summary>
        /// A number that will be added to the player health OnPick.
        /// </summary>
        [Tooltip("How many health points the player will get when picking this item")]
        [Space]
        public float HealAmount;

        /// <summary>
        /// Percentage of the player maximum health that will be added to the player health OnPick.
        /// </summary>
        [Tooltip("Percentage of the player maximum health that will be added to the player health" +
                 " when he picks this item")]
        [Space]
        [Range(0,100f)]
        public float HealPercentage;

        protected override void PickUpEffect()
        {
            //The amount of healing for the player.
            float heal = HealAmount + (HealPercentage / 100f) * target.MaxHealth;

            //Make sure the the amount doesn't exceed maximum health.
            if (target.CurrentHealth + heal >= target.MaxHealth)
                target.CurrentHealth = target.MaxHealth;
            else
                target.CurrentHealth += heal;

            //Raise the OnPick event for the player.
            target.RiseOnPickUp(PickUpType.Heal);
            
        }

    }

}
