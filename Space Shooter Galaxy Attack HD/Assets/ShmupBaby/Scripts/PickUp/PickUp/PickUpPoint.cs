using UnityEngine;

namespace ShmupBaby
{

    /// <summary>
    /// PickUp item that player can pick to add bonus score.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Pick Up/Pick Up Point ")]
    public class PickUpPoint : PickUp
    {
        /// <summary>
        /// The amount of points that will be added to the score
        /// when the player picks the items.
        /// </summary>
        [Tooltip("The amount of points that will be added to the score" +
                 "when the player picks it.")]
        [Space]
        public int PointAmount;
        
        protected override void PickUpEffect()
        {
            LevelController.Instance.AddScore(PointAmount);

            //Raises the OnPick event for the player.
            target.RiseOnPickUp(PickUpType.Point);

        }

    }

}
