using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Swaps the sprite in a sprite renderer depending on a mover direction.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Component/Direction Sprite Swap")]
    public sealed class DirectionSpriteSwap : MonoBehaviour
	{
        /// <summary>
        /// Sprite flip axis.
        /// </summary>
	    public enum FlipAxis
	    {
	        X ,
	        Y
	    }

        /// <summary>
        /// The mover that will give direction.
        /// </summary>
		[Space]
        [Tooltip("The mover that defines the direction.")]
		public Mover mover;
        /// <summary>
        /// The renderer which will represent the sprites.
        /// </summary>
        [Tooltip("Sprite renderer which will render the sprite.")]
		public SpriteRenderer TargetRenderer ;
        /// <summary>
        /// The sprite for idle (when the mover is not moving).
        /// </summary>
	    [Space]
        [Tooltip("The sprite which will appear when the mover is not moving")]
		public Sprite Idle;
	    /// <summary>
	    /// The sprite for the up direction.
	    /// </summary>
		[Space]
		[Tooltip("The sprite which will appear when the mover is moving up")]
        public Sprite Up;
	    /// <summary>
	    /// The sprite for the down direction.
	    /// </summary>
	    [Tooltip("The sprite which will appear when the mover is moving down")]
        public Sprite Down;
        /// <summary>
        /// Indicates if the sprite should be flipped if the opposite sprite is used.
        /// </summary>
	    [Tooltip("By default if the up sprite is empty the down sprite will take its place and vice versa," +
	             "checking this will flip the down sprite before it takes the place of the upward direction.")]
        public bool FlipY ;
	    /// <summary>
	    /// The sprite for the right direction.
	    /// </summary>
		[Space]
		[Tooltip("The sprite which will appear when the mover is moving right")]
        public Sprite Right;
	    /// <summary>
	    /// The sprite for the left direction.
	    /// </summary>
	    [Tooltip("The sprite which will appear when the mover is moving left")]
        public Sprite Left;
	    /// <summary>
	    /// Indicate if the sprite should be flipped if the opposite sprite is used.
	    /// </summary>
	    [Tooltip("By default if the right sprite is empty the left sprite will take its place and vice versa," +
                 "Checking this will flip the left sprite before it take the place of the right.")]
        public bool FlipX ;

	    /// <summary>
	    /// One of Unity's messages that gets called on every frame.
	    /// </summary>
		private void Update ()
		{
			if (!mover || !TargetRenderer)
				return;

            //Will switch the sprite based on the mover direction.
			switch (mover.BasicDirection) {

			    case FourDirection.Up: 
				    {
					    SpriteSwap (Up, Down, FlipY, FlipAxis.Y);
					    break;
				    }

			    case FourDirection.Down: 
				    {
					    SpriteSwap (Down, Up, FlipY, FlipAxis.Y);
					    break;
				    }

			    case FourDirection.Right: 
				    {
					    SpriteSwap (Right, Left, FlipX, FlipAxis.X);
					    break;
				    }

			    case FourDirection.Left: 
				    {
					    SpriteSwap (Left, Right, FlipX, FlipAxis.X);
					    break;
				    }

			    case FourDirection.None: 
				    {
					    if (Idle != null) {
						    TargetRenderer.flipY = false;
						    TargetRenderer.flipX = false;
						    TargetRenderer.sprite = Idle;
					    }
					    break;
				    }

			}

		}

        /// <summary>
        /// Swap the sprite for the spite renderer.
        /// </summary>
        /// <param name="mainSprite">The sprite that will be assigned.</param>
        /// <param name="oppositeSprite">Backup sprite if the main sprite is null.</param>
        /// <param name="flipOppsite">Should the sprite be flipped.</param>
        /// <param name="axis">The flip axis.</param>
		private void SpriteSwap ( Sprite mainSprite , Sprite oppositeSprite , bool flipOppsite , FlipAxis axis ) {
            
            //Resets the flip value for the sprite renderer.
			TargetRenderer.flipX = false;
            TargetRenderer.flipY = false;

			if (mainSprite == null) {
				
				if (oppositeSprite != null )
					
				if (axis == FlipAxis.X && flipOppsite) {
					
					TargetRenderer.flipX = true;
				}else
					TargetRenderer.flipY = true;
				
				TargetRenderer.sprite = oppositeSprite;

			} else {
				
				TargetRenderer.sprite = mainSprite;
			}

		}

	}
}