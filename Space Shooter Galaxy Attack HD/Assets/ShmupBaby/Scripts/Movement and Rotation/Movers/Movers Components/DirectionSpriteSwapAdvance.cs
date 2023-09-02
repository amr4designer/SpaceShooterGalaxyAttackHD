using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Swaps the sprite in a sprite renderer depending on a mover direction.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Component/Direction Sprite Swap Advance ")]
    public class DirectionSpriteSwapAdvance : MonoBehaviour
	{

        /// <summary>
        /// Any possible direction to swap the sprite.
        /// </summary>
        public enum SpriteDirection
        {
            None,
            Idle,
            DownRight,
            UpLeft,
            UpRight,
            Down,
            DownLeft,
            Up,
            Left,
            Right
        }

        /// <summary>
        /// Defines the settings to swap a sprite on a given direction.
        /// </summary>
        [System.Serializable]
        public class SpriteSwapSetting
        {
            /// <summary>
            /// The main sprite that's going to be assigned for the sprite renderer.
            /// </summary>
            [Tooltip("The sprite that will be assigned to the sprite renderer" +
                "when the mover move on it's direction.")]
            public Sprite sprite;
            /// <summary>
            /// Options for other sprites to be assigned in the sprite renderer.
            /// </summary>
            [Tooltip("Use other assigned sprite when the mover moves in this direction")]
            public SpriteDirection UseOther = SpriteDirection.None;
            /// <summary>
            /// Indicates if the sprite renderer should flip the sprite vertically.
            /// </summary>
            [Tooltip("Flips the sprite vertically before assigning it")]
            public bool FlipY = false;
            /// <summary>
            /// Indicates if the sprite renderer should flip the sprite horizontally.
            /// </summary>
			[Tooltip("Flips the sprite horizontally before assigning it")]
            public bool FlipX = false;

        }


        /// <summary>
        /// The mover that will give directions.
        /// </summary>
		[Space]
        [Tooltip("The mover that defines the direction.")]
        public Mover mover ;
        /// <summary>
        /// The renderer which will represent the sprites.
        /// </summary>
        [Tooltip("Sprite renderer which will render the sprite.")]
        public SpriteRenderer TargetRenderer ;
        /// <summary>
        /// The sprite swap option for the idle state.
        /// </summary>
		[Space]
        [Tooltip("The sprite swap option for the idle state (when the mover is stopped)")]
		public SpriteSwapSetting Idle;
        /// <summary>
        /// The sprite swap option when the mover is moving up.
        /// </summary>
        [Tooltip("The sprite swap option when the mover is moving up")]
        public SpriteSwapSetting Up;
        /// <summary>
        /// The sprite swap option when the mover is moving Down.
        /// </summary>
        [Tooltip("The sprite swap option when the mover is moving Down")]
        public SpriteSwapSetting Down;
        /// <summary>
        /// The sprite swap option when the mover is moving Left.
        /// </summary>
        [Tooltip("The sprite swap option when the mover is moving Left")]
        public SpriteSwapSetting Left;
        /// <summary>
        /// The sprite swap option when the mover is moving Right.
        /// </summary>
        [Tooltip("The sprite swap option when the mover is moving Right")]
        public SpriteSwapSetting Right;
        /// <summary>
        /// The sprite swap option when the mover is moving Up and to the Left.
        /// </summary>
        [Tooltip("The sprite swap option when the mover is moving Up and to the Left")]
        public SpriteSwapSetting UpLeft;
        /// <summary>
        /// The sprite swap option when the mover moving Up and to the Right.
        /// </summary>
        [Tooltip("The sprite swap option when the mover moving Up and to the Right")]
        public SpriteSwapSetting UpRight;
        /// <summary>
        /// The sprite swap option when the mover is moving Down and to the Left.
        /// </summary>
        [Tooltip("The sprite swap option when the mover is moving Down and to the Left")]
        public SpriteSwapSetting DownLeft;
        /// <summary>
        /// The sprite swap option when the mover is moving Down and to the Right.
        /// </summary>
        [Tooltip("The sprite swap option when the mover is moving Down and to the Right")]
        public SpriteSwapSetting DownRight;


        void Update ()
		{

			if (!mover || !TargetRenderer)
				return;

			switch (mover.AdvanceDirection) {

			case EightDirection.Up: 
				{
					SpriteSwap (Up);
					break;
				}

			case EightDirection.Down: 
				{
					SpriteSwap (Down);
					break;
				}

			case EightDirection.Left: 
				{
					SpriteSwap (Left);
					break;
				}

			case EightDirection.Right: 
				{
					SpriteSwap (Right);
					break;
				}

			case EightDirection.UpLeft: 
				{
					SpriteSwap (UpLeft);
					break;
				}

			case EightDirection.DownLeft: 
				{
					SpriteSwap (DownLeft);
					break;
				}
			case EightDirection.UpRight: 
				{
					SpriteSwap (UpRight);
					break;
				}

			case EightDirection.DownRight: 
				{
					SpriteSwap (DownRight);
					break;
				}

			case EightDirection.None: 
				{
					SpriteSwap (Idle);
					break;
				}

			}

		}

        /// <summary>
        /// Swaps the sprite in the sprite renderer.
        /// </summary>
        /// <param name="Settings">Options to swap the sprite.</param>
		void SpriteSwap ( SpriteSwapSetting Settings ) {

            //flip the sprite for the renderer
			TargetRenderer.flipY = Settings.FlipY;
			TargetRenderer.flipX = Settings.FlipX;

			if (Settings.UseOther == SpriteDirection.None) {
				if (Settings.sprite != null) 
					TargetRenderer.sprite = Settings.sprite;
			} else 
                //Assigns the other sprite.
				TargetRenderer.sprite = GetSpriteByDirection (Settings.UseOther);
		}

        /// <summary>
        /// Returns the main sprite for a given direction.
        /// </summary>
        /// <param name="spriteDirection">The direction for the sprite.</param>
        /// <returns>The direction sprite.</returns>
		Sprite GetSpriteByDirection ( SpriteDirection spriteDirection ) 
		{
			switch (spriteDirection) {
				case SpriteDirection.Idle:
					return Up.sprite;

				case SpriteDirection.Up:
					return Up.sprite;

				case SpriteDirection.Down:
					return Down.sprite;

				case SpriteDirection.Left:
					return Left.sprite;

				case SpriteDirection.Right:
					return Right.sprite;

				case SpriteDirection.UpRight:
					return UpRight.sprite;

				case SpriteDirection.UpLeft:
					return UpLeft.sprite;

				case SpriteDirection.DownLeft:
					return DownLeft.sprite;

				case SpriteDirection.DownRight:
					return DownRight.sprite;
			}

			return null;
		}
	}

}