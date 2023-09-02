using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
	/// The speed buff (timed improvement) provides the player with temporary speed increase.
    /// </summary>
    public class SpeedBuff : PlayerBuff
    {
        /// <summary>
        /// The speed added to the player for this buff duration.
        /// </summary>
        private float _speed;

        /// <summary>
        /// SpeedBuff constructor.
        /// </summary>
        /// <param name="myPlayer">The player that will have the buff</param>
        /// <param name="duration">The duration of the buff in sec</param>
        /// <param name="speedAmount">The speed amount that will be added to the player
        /// when the buff is active</param>
        public SpeedBuff(Player myPlayer, float duration , float speedAmount ) 
        {
            _speed = speedAmount;

            InitializeBuff(myPlayer,duration);

        }

        protected override void Start()
        {
            //Adds speed to the player when the buff effect starts.
            MyPlayer.mover.Speed += _speed;
        }

        protected override void End()
        {
            //Removes the speed from the player when the buff effect ends.
            MyPlayer.mover.Speed -= _speed;
        }
        
    }

    /// <summary>
    /// PickUp item that player can pick to provide temporary speed.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Pick Up/Pick Up Speed")]
    public class PickUpSpeed : PickUp
    {
        /// <summary>
        /// The speed added to the player for this buff duration.
        /// </summary>
        [Space]
        [Tooltip("the speed added to the player for this buff duration")]
        public float SpeedAmount;
        /// <summary>
        /// A percentage of the player speed that will be added to the player for this buff duration
        /// </summary>
        [Space]
        [Tooltip("A percentage of the player speed that will be added to the player for this buff duration")]
        public float SpeedPercentage;
        /// <summary>
		/// A limit for the pickup speed which must not be exceeded,
        /// this is a safety net if multiple speed pickups affect the player.
        /// </summary>
        [Space]
		[Tooltip("A limit for the pickup speed which must not be exceeded.")]
        public float MaxSpeed;
        /// <summary>
        /// Duration of the temporary speed
        /// </summary>
        [Space]
        [Tooltip("Duration of the temporary speed increase")]
        public float Duration;
        
        protected override void PickUpEffect()
        {
            //The amount of speed for the player.
            float bounsSpeed = SpeedAmount + (SpeedPercentage / 100) * target.mover.Speed;

            //Checks that the player speed doesn't exceed the maximum speed when the player
            //picks the pick up.
            if (MaxSpeed != 0 && ((Mover) target.mover).speed + bounsSpeed > MaxSpeed)
                bounsSpeed = 0;
            
            //adds the speed buff for the player.
            if (target.ActiveBuff == null || target.ActiveBuff.BuffEnded)
                target.ActiveBuff = new SpeedBuff(target,Duration, bounsSpeed);

            //Raises the OnPick event for the player.
            target.RiseOnPickUp(PickUpType.Speed);

        }

    }

}