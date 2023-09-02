using System.Collections;
using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// The base class for any temporary ability for the player.
	/// A buff is a temporal effect increasing the player's abilities.
    /// </summary>
    public class PlayerBuff 
    {
        /// <summary>
        /// The player that has this buff activated on.
        /// </summary>
        protected Player MyPlayer;

        /// <summary>
        /// Indicates when the buff duration has ended.
        /// </summary>
        public bool BuffEnded = false;

        /// <summary>
		/// Initializes the buff.
        /// </summary>
        /// <param name="myPlayer">The Player that has this buff.</param>
        /// <param name="duration">The duration of this buff in seconds.</param>
        protected void InitializeBuff(Player myPlayer, float duration)
        {
            MyPlayer = myPlayer;
            myPlayer.StartCoroutine(ActiveBuff(duration));
        }

        /// <summary>
        /// Activates the buff.
        /// </summary>
        /// <param name="duration">The duration of activation.</param>
        IEnumerator ActiveBuff (float duration)
        {

            float endTime = Time.time+duration;

            Start();

            while (Time.time <= endTime)
            {
                Update();
                yield return null;
            }

            End();

            BuffEnded = true;

        }

        /// <summary>
        /// This method will be called on every frame while the buff is activated.
        /// </summary>
        protected virtual void Update()
        {

        }
        /// <summary>
        /// This method will be called when the buff is activated.
        /// </summary>
        protected virtual void Start()
        {

        }
        /// <summary>
        /// This method will be called when the buff duration has ended.
        /// </summary>
        protected virtual void End()
        {

        }
        
    }

   
	/// <summary>
	/// PickUps are items that the player can pick to provide different effects.
	/// </summary>
	[RequireComponent(typeof(PickUpMover))]
    [AddComponentMenu("")]
	public class PickUp : MonoBehaviour
    {       
        /// <summary>
        /// The player that will be provided with the effect when the item is picked.
        /// </summary>
	    protected Player target { get { return LevelController.Instance.PlayerComponent; } }

        /// <summary>
        /// The mover for this pickup.
        /// </summary>
	    public PickUpMover mover { get; set; }

        /// <summary>
        /// Indicates if the effect is applied to the target player.
        /// </summary>
	    private bool takeEffect = false;

	    /// <summary>
	    /// One of Unity's messages that act the same way as start but gets called before start.
	    /// </summary>
        void Awake()
        {
            mover = GetComponent<PickUpMover>();
            mover.OnReachTarget += OnPick;          
		}
        
        /// <summary>
        /// Called when this PickUp is picked.
        /// </summary>
        protected void OnPick(ShmupEventArgs args)
        {
            //This is to make sure the pickup item is only picked once.
            //In some cases it might take many frames before destroying the pickup.
		    if (!takeEffect)
		    {
		        PickUpEffect();
		        takeEffect = true;
		    }

		    Destroy (gameObject);
        }

        /// <summary>
        /// Will be called once when the pickup is picked.
        /// </summary>
		protected virtual void PickUpEffect ()
        {
			Debug.Log ("Picking");
		}

	}

}
	