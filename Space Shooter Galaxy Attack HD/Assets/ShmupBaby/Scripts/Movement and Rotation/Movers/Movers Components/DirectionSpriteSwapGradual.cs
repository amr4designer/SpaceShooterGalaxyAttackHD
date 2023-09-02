using UnityEngine;

namespace ShmupBaby
{

    /// <summary>
    /// Flips though a series of sprites based on the agent direction.
    /// This works only on one movement axis at a time, horizontal or vertical.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Component/Direction Sprite Swap Gradual")]
    public sealed class DirectionSpriteSwapGradual : MonoBehaviour
	{
        [Tooltip("The agent mover which defines direction. I.e player or enemy movers.")]
        public Mover AgentMover;

        [Tooltip("Sprite renderer which will be swapped by the images you input below.")]
        public SpriteRenderer TargetRenderer;

        [Tooltip("How much time it takes to flip from one sprite frame to the next.")]
        public float TimeBetweenFrames;


        [Space]
        [Header("Use With Vertical Levels")]
        public SpriteFrames Left;
        public SpriteFrames Right;

        [Space]
        [Header("Use With Horizontal Levels")]        
        public SpriteFrames Up;
        public SpriteFrames Down;

        private Sprite idleFrame;

        private int leftFramesCount;
        private int rightFramesCount;
        private int upFramesCount;
        private int downFramesCount;

        // The frameDisplayed integer is the basis of how this script works.
        // A positive or negative value determines if the agent is moving Up or Down,
        // Left or Right. And it will control the sprite swapping mechanism. 
        private int frameDisplayed;

        private float frameSwapTimer;

        private bool isVerticalLevel
        {
            get
            {
                return (ViewType == LevelViewType.Vertical);
            }
        }

        private bool isHorizontalLevel
        {
            get
            {
                return (ViewType == LevelViewType.Horizontal);
            }
        }

        private LevelViewType ViewType
        {
            get
            {
                return LevelController.Instance.View;
            }
        }

        private bool isMissingFrames;       


        [System.Serializable]
        public class SpriteFrames
        {
            public Sprite[] Frames;
        }


        private void Start()
        {
            CacheIdleSprite();
            CacheFramesCount();
            CheckMissingFrames();
        }

        private void Update()
		{
			if (AgentMover == null)
            {
                return;
            }

            if(TargetRenderer == null)
            {
                return;
            }

            if (isMissingFrames)
            {
                return;
            }

            if (Time.time < frameSwapTimer)
            {
                return;
            }

            UpdateFrameDisplayed();
            SwapFrame();
            ResetFrameSwapTimer();
        }


        #region Checking For Missing Sprites


        private void CheckMissingFrames()
        {
            if (isVerticalLevel)
            {
                isMissingFrames = IsMissingLeftOrRightFrames();
            }

            if (isHorizontalLevel)
            {
                isMissingFrames = IsMissingUpOrDownFrames();
            }                      
        }


        private bool IsMissingLeftOrRightFrames()
        {
            bool isMissingLeftFrames =
                IsMissingArraySprites(Left.Frames);

            bool isMissingRightFrames =
                IsMissingArraySprites(Right.Frames);

            return (isMissingLeftFrames || isMissingRightFrames);
        }


        private bool IsMissingUpOrDownFrames()
        {
            bool isMissingUpFrames =
                IsMissingArraySprites(Up.Frames);

            bool isMissingDownFrames =
                IsMissingArraySprites(Down.Frames);

            return (isMissingUpFrames || isMissingDownFrames);
        }


        private bool IsMissingArraySprites(Sprite[] directionFrames)
        {
            for (int i = 0; i < directionFrames.Length; i++)
            {
                if (directionFrames[i] == null)
                {
                    return true;
                }
            }
            return false;
        }


        #endregion


        private void CacheIdleSprite()
        {
            if (TargetRenderer != null)
            {
                idleFrame = TargetRenderer.sprite;
            }
            return;
        }


        private void CacheFramesCount()
        {
            rightFramesCount = Right.Frames.Length;
            leftFramesCount = Left.Frames.Length;
            upFramesCount = Up.Frames.Length;
            downFramesCount = Down.Frames.Length;
        }

        private void ResetFrameSwapTimer()
        {
            frameSwapTimer = Time.time + TimeBetweenFrames;
            return;
        }


        private void UpdateFrameDisplayed()
        {
            if (isVerticalLevel)
            {
                UpdateVerticalLevelFrameDisplayed();
            }

            if (isHorizontalLevel)
            {
                UpdateHorizontalLevelFrameDisplayed();
            }

            return;
        }

        private void SwapFrame()
        {
            if (frameDisplayed == 0)
            {
                TargetRenderer.sprite = idleFrame;
            }

            if (isVerticalLevel)
            {
                SwapVerticalLevelFrames();
            }

            if (isHorizontalLevel)
            {
                SwapHorizontalLevelFrames();
            }

            return;
        }


        #region Vertical Level Methods


        private void UpdateVerticalLevelFrameDisplayed()
        {
            if (AgentMover.SideToSideDirection == HorizontalDirection.Right)
            {
                if (frameDisplayed < rightFramesCount)
                {
                    frameDisplayed++;
                }
            }

            if (AgentMover.SideToSideDirection == HorizontalDirection.Left)
            {
                if (frameDisplayed > -leftFramesCount)
                {
                    frameDisplayed--;
                }
            }

            if (AgentMover.SideToSideDirection == HorizontalDirection.None)
            {
                if (frameDisplayed > 0)
                {
                    frameDisplayed--;
                }

                if (frameDisplayed < 0)
                {
                    frameDisplayed++;
                }
            }
            return;
        }


        void SwapVerticalLevelFrames()
        {
            if (frameDisplayed > 0)
            {
                TargetRenderer.sprite =
                    Right.Frames[frameDisplayed - 1];
            }

            if (frameDisplayed < 0)
            {
                TargetRenderer.sprite =
                    Left.Frames[-(frameDisplayed + 1)];
            }
            return;
        }


        #endregion


        #region Horizontal Level Sprites


        private void UpdateHorizontalLevelFrameDisplayed()
        {
            if (AgentMover.UpAndDownDirection == VerticalDirection.Up)
            {
                if (frameDisplayed < Up.Frames.Length)
                {
                    frameDisplayed++;
                }
            }

            if (AgentMover.UpAndDownDirection == VerticalDirection.Down)
            {
                if (frameDisplayed > -Down.Frames.Length)
                {
                    frameDisplayed--;
                }
            }

            if (AgentMover.UpAndDownDirection == VerticalDirection.None)
            {
                if (frameDisplayed > 0)
                {
                    frameDisplayed--;
                }

                if (frameDisplayed < 0)
                {
                    frameDisplayed++;
                }
            }
        }


        private void SwapHorizontalLevelFrames()
        {
            if (frameDisplayed > 0)
            {
                TargetRenderer.sprite =
                    Up.Frames[frameDisplayed - 1];
            }

            if (frameDisplayed < 0)
            {
                TargetRenderer.sprite =
                    Down.Frames[-(frameDisplayed + 1)];
            }

            return;
        }


        #endregion


    }


}