using System.Collections.Generic;
using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// The base class for all Player enemies.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Enemy/Enemy")]
    public class Enemy : Agent , IEnemy
	{
	    /// <summary>
	    /// This field represents the Enemy health in the inspector,
	    /// also this value will be used to set the enemy max health when the enemy starts.
	    /// </summary>
        [Space]
        [Tooltip("This value represent the enemy max health, the enemy health will be at max when" +
                 "the enemy starts, this field represents the enemy health at run time.")]
	    public float Health = 5;
	    /// <summary>
		/// This field represents the Enemy shield in the inspector, also
	    /// this value will be used to set the enemy max shield when the enemy starts.
	    /// </summary>
	    [Space]
	    [Tooltip("This value represent the enemy max shield, the enemy shield will be at max when" +
	             "the enemy starts, this field represents the enemy health at run time.")]
        [Space]
	    public float Shield = 5;
        /// <summary>
        /// The damage that the enemy will do to the agent that it collides with.
        /// </summary>
		[Space]
        [Tooltip("The damage that the enemy will do to the agent that it collides with.")]
		public float collisionDamage;
        /// <summary>
        /// The rewarding points the player will get when destroying this enemy.
        /// </summary>
		[Space]
		[Tooltip("The rewarding point the player will get when destroying this enemy")]
        public int PointGiven;
	    /// <summary>
	    /// The radius that the drops will be dropped randomly inside.
	    /// </summary>
		[Space]
        [Tooltip("The radius that the drops will be dropped randomly inside.")]
		public float dropRadius;
        /// <summary>
        /// The drop item that will be dropped when Drop is called.
        /// </summary>
	    [Space]
		[Tooltip("The drop item that will be dropped on enemy death")]
        public Drop[] drops;

        /// <summary>
        /// Trigger when there is no enemy in the scene.
        /// </summary>
		public static event ShmupDelegate OnAllEnemyDestroyed;

	    /// <summary>
	    /// Agent event is a trigger when the agent collides with another agent.
	    /// </summary>
        public event ShmupDelegate OnCollide;
	    /// <summary>
	    /// Agent event is a trigger on every drop by the agent.
	    /// </summary>
		public event ShmupDelegate OnDrop ;
	    /// <summary>
	    /// Agent event is a trigger when the agent activates the Immunity.
	    /// </summary>
	    public event ShmupDelegate OnImmunityActivate;
	    /// <summary>
	    /// Agent event is a trigger when the agent disables the Immunity.
	    /// </summary>
	    public event ShmupDelegate OnImmunityDisabled;
	    /// <summary>
	    /// Agent event is trigger when this enemy is added to PoolManager.
	    /// </summary>
        public event ShmupDelegate OnAddToPool;
	    /// <summary>
	    /// Agent event is a trigger when this enemy is removed to PoolManager.
	    /// </summary>
		public event ShmupDelegate OnRemoveFromPool;


        //Overrides properties because there is no property support in the inspector
	    public override float CurrentHealth
	    {
	        get { return Health; }
	        set { Health = value; }
	    }
	    public override float CurrentShield
	    {
	        get { return Shield; }
	        set { Shield = value; }
	    }

	    public override AgentSide Side
	    {
            get
            {
                return AgentSide.Enemy;
            } 
	    }

        public float CollisionDamage
	    {
	        get
	        {
                return collisionDamage;
	        }
	    }

	    public float DropRadius
	    {
	        get { return dropRadius; }
	        set { dropRadius = value; }
	    }

	    public Drop[] Drops
	    {
	        get { return drops; }
	    }


        /// <summary>
        /// Controls Enemy Immunity.
        /// </summary>
        public bool Immunity
	    {
	        get
	        {
	            return _immunity;
	        }
	        protected set
	        {
	            //Checks to see if the value that we set has changed from the last time
	            //it got set.
                if (_immunity != value)
	            {
                    //To rise the event once at the moment the value changed.
                    if ( value )
                        RiseOnImmunityActivate();
                    else                  
                        RiseOnImmunityDisabled();
                }

	            _immunity = value;
	        }
	    }

        /// <summary>
        /// The field of the game.
        /// </summary>
	    protected Rect GameField { get { return LevelController.Instance.GameField; } }

        /// <summary>
        /// The view field of the camera.
        /// </summary>
	    protected Rect CameraField { get { return OrthographicCamera.Instance.CameraRect; } }

        /// <summary>
        /// Enemy weapon handle component,used for enemy to handle a weapon it can be set to null
        /// if the enemy doesn't need weapons.  
        /// </summary>
        protected IHandleWeapons WeaponsHandle;

        //The back-end field for Immunity.
        private bool _immunity;


	    private void OnEnable ()
        {            
            // Add the enemy to the pool.
            this.AddToPool<Enemy>();
		}

		private void OnDisable ()
        {
            //Remove the enemy from the pool
            if (!this.RemoveFromPool<Enemy>())
            {
                return;
            }               

            // Triggers the OnAllEnemyDestroyed if no enemy is enabled in the scene.
            if (OnAllEnemyDestroyed != null)
            {
                OnAllEnemyDestroyed(null);
            }                        
        }

        protected virtual void Awake () 
		{
		    MaxHealth = CurrentHealth;
			MaxShield = CurrentShield;
			
		    WeaponsHandle = GetComponent<IHandleWeapons>();          
		}

	    /// <summary>
	    /// Damages the agent with the CollisionDamage.
	    /// </summary>
	    /// <param name="target">An agent that you collided with.</param>
	    public void DoCollisionDamage(Agent target)
	    {
	        if (target == null)
	            return;

            if (target.Side == Side)
                return;

	        RiseOnCollide();

	        target.TakeDamage(CollisionDamage, DamageSource.Collision);	        
	    }

        protected virtual void Update ()
        {
            //Activate enemy Immunity if the enemy is outside the CameraField. 
			ImmunityByRegion (CameraField, true);
		}

	    /// <summary>
	    /// Handles any Subscription to the agent event by using AllAgentEvents enumerator,
	    /// returns true for a successful subscription and false for a failed one.
	    /// </summary>
	    /// <param name="method">A method to get called when the events occur.</param>
	    /// <param name="eventType">The type of event</param>
	    public override bool Subscribe(ShmupDelegate method, AllAgentEvents EventType)
	    {
	        switch (EventType)
	        {
	            case AllAgentEvents.ImmunityActivate:
	            {
	                OnImmunityActivate += method;
	                return true;
                }

	            case AllAgentEvents.ImmunityDisabled:
	            {
	                OnImmunityDisabled += method;
	                return true;
                }

	            case AllAgentEvents.Drop:
	            {
	                OnDrop += method;
	                return true;
                }

	            case AllAgentEvents.DealCollision:
	            {
	                OnCollide += method;
	                return true;
	            }

                case AllAgentEvents.WeaponsStartFire:
	            {
	                WeaponsHandle.OnStartFire += method;
	                return true;
                }
	            case AllAgentEvents.WeaponsStopFire:
	            {
	                WeaponsHandle.OnStopFire += method;
	                return true;
                }
            }

	        return base.Subscribe(method, EventType);
        }

	    /// <summary>
	    /// Damages the agent by considering the type of defenses that the agent has.
	    /// </summary>
	    /// <param name="damage">The amount of damage that agent takes.</param>
	    /// <param name="source">The damage source that deals this damage.</param>
	    public override void TakeDamage (float Damage, DamageSource Source)
		{
		    //If immunity is activated then the player can't take any damage.
            if (Immunity)
				return;

			base.TakeDamage (Damage, Source);
		}

	    /// <summary>
	    /// Destroys the agent in a safe way (rises the right event before the agent gets destroyed).
	    /// </summary>
		public override void StartDestroy () 
		{
            //The enemy needs to drop and add the score to the player before it gets destroyed.
			Drop();

			LevelController.Instance.AddScore(PointGiven);

			base.StartDestroy ();
		}

	    /// <summary>
	    /// Control agent Immunity depending on agent position to a given region
	    /// this method should be used in Update.
	    /// </summary>
	    /// <param name="region">Region of Immunity</param>
		public void ImmunityByRegion (Rect region, bool flipRegion)
        {
            //Offsets the value from the field.
			const float offset = 0.2f;

		    bool inRegion = false;

            //Checks if the enemy is in the region or not.
			if (transform.position.y < region.yMax + offset && 
				transform.position.y > region.yMin - offset &&
				transform.position.x < region.xMax + offset && 
				transform.position.x > region.xMin - offset)
			    inRegion = true;
			else
			    inRegion = false;

			if (flipRegion) 
				Immunity = !inRegion;
			else
			    Immunity = inRegion;           
		}

	    /// <summary>
	    /// Instantiates and drops all the drops in the DropRadius.
	    /// </summary>
        public void Drop ()
        {
			for (int i = 0; i < drops.Length; i++)
            {
				InstantiateDrop (drops [i]);
			}
		}

        /// <summary>
        /// Creates the drop depending on its chance, and makes it ready to go.
        /// </summary>
        /// <param name="Drop"></param>
		private void InstantiateDrop (Drop Drop)
        {
            // Checks for drop chance.
			if (Drop.DropChance < Random.Range (0, 100))
            {
                return;
            }
				
            // Instantiates the drop and places it in the place of the enemy.
            GameObject DropObject = Instantiate(Drop.DropObject, LevelController.Instance.PickUpParent.transform) as GameObject;
            DropObject.transform.position = transform.position;

			PickUp pickUpScript = DropObject.GetComponent<PickUp> ();
            
            if (pickUpScript == null)
            {
				Debug.Log ("You have added an item to enemy drop which does not have a PickUp Script attached.");
				return;
			}
            
            // Sets the PickUp to its SpawnPosition.
		    Vector3 dropPosition = Random.insideUnitCircle * dropRadius;

            if (pickUpScript.mover == null)
            {
                pickUpScript.mover = DropObject.GetComponent<PickUpMover>();
            }

            pickUpScript.mover.SpawnPosition = transform.position + Math2D.Vector2ToVector3(dropPosition);

            DropObject.SetActive(true);

            // Raises OnDrop event.
            RiseOnDrop(new DropArgs(transform.position, dropPosition));
		}

        /// <summary>
        /// Destroys all enemy weapons.
        /// </summary>
        public void DestroyWeapon()
        {
            if (WeaponsHandle != null)
                WeaponsHandle.DestroyWeapon();
        }

        /// <summary>
        /// Handles the rise of OnCollide event.
        /// </summary>
        protected void RiseOnCollide()
	    {
	        if (OnCollide != null)
	            OnCollide(null);
	    }
        /// <summary>
        /// Handles the rise of OnImmunityActivate event.
        /// </summary>
        protected void RiseOnImmunityActivate()
	    {
	        if (OnImmunityActivate != null)
	            OnImmunityActivate(null);
	    }
        /// <summary>
        /// Handles the rise of OnImmunityDisabled event.
        /// </summary>
        protected void RiseOnImmunityDisabled()
	    {
	        if (OnImmunityDisabled != null)
	            OnImmunityDisabled(null);
	    }
        /// <summary>
        /// Handles the rise of OnDrop event.
        /// </summary>
        protected void RiseOnDrop(DropArgs args)
	    {
	        if (OnDrop != null)
	            OnDrop(args);
	    }

#if UNITY_EDITOR

        void OnDrawGizmos()
	    {
            //Draws the drop radius.
            Gizmos.color = Color.black;
            GizmosExtension.DrawCircle(transform.position,dropRadius);
	    }

#endif

    }

}