using UnityEngine;
using System.Collections;

namespace ShmupBaby {
    
    /// <summary>
    /// A base class for all Shmup agents ( Player , Enemy , Other like Missile ).
    /// Manages the ability to handle the damage taken and Agent Event subscription.
    /// </summary>
	public abstract class Agent : MonoBehaviour , ITakeShieldDamage , ITakeCollision
	{
        /// <summary>
        /// This event rises on the Start method for this agent.
        /// </summary>
	    public event ShmupDelegate OnStart;
        /// <summary>
	    /// This event rises when the agent gets destroyed for running out of health.
	    /// </summary>
        public event ShmupDelegate OnDestroyStart ;
        /// <summary>
	    /// This event rises when the agent take damages directly on health.
	    /// </summary>
		public event ShmupDelegate OnTakeHealthDamage ;
        /// <summary>
	    /// This event rises when the agent takes damage directly on shield.
	    /// </summary>
		public event ShmupDelegate OnTakeShieldDamage ;
	    /// <summary>
	    /// Agent event is trigger when agent takes collision damage.
	    /// </summary>
	    public event ShmupDelegate OnTakeCollision;

        /// <summary>
        /// The current value for the health points.
        /// </summary>
        public virtual float CurrentHealth { get; set; }
	    /// <summary>
	    /// The value for the maximum health points.
	    /// </summary>
		public virtual float MaxHealth { get; set; }

	    /// <summary>
	    /// The current value for the shield points.
	    /// </summary>
		public virtual float CurrentShield { get; set; }
	    /// <summary>
	    /// The value for the maximum shield points.
	    /// </summary>
		public virtual float MaxShield  { get; set; }

        /// <summary>
        /// Returns the side of the agent, is it enemy or player or Allied.
        /// </summary>
        public virtual AgentSide Side { get; set; }

        /// <summary>
        /// Is this agent allowed to take collision damage.
        /// </summary>
	    public virtual bool AllowCollisionDamage { get; protected set; }

        //Collision cool-down time.
	    protected float _timeBetweenCollision;
	    //is this agent in cool down from taking collision.
        protected bool _collisionCoolDown;
        /// <summary>
        /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
        protected virtual void Start()
	    {
            //Rises the OnStart event for this agent.
	        RiseOnStart();
        }

	    /// <summary>
	    /// Damages the agent by considering the type of defenses that the agent has
	    /// </summary>
	    /// <param name="damage">The amount of damage that agent takes.</param>
	    /// <param name="source">The damage source that deals this damage.</param>
		public virtual void TakeDamage (float damage, DamageSource source)
		{
            if (damage <= 0)
                return;

		    //If the damage source is Collision it needs TakeCollisionDamage to handle it.
		    if (source == DamageSource.Collision)
		    {
                //The following conditions is what prevents the damage from collision
                //- The agent doesn't allow collision damage.
                //- Or the agent is in collision cool-down time.
                if (!AllowCollisionDamage || _collisionCoolDown )
                    return;

		        //Rise OnTakeCollision event.
		        RiseOnTakeCollision(null);
                
		        //But take collision on cool-down time.
		        StartCoroutine(TakeCollisionCoolDown(_timeBetweenCollision));

            }

            //Call TakeDamageOnShield first because it protects the health from direct hit.
            //TakeDamageOnShield returns the remaining damage that got saved in damageLeft.
            float damageLeft = TakeDamageOnShield (damage, source);

            //Call TakeDamageOnHealth with the remaining damage.
            TakeDamageOnHealth(damageLeft, source);
		}

	    /// <summary>
	    /// Damages the agent directly on health then check if
	    /// Health is below zero to start agent destruction.
	    /// </summary>
	    /// <param name="damage">The amount of damage that the agent takes.</param>
	    /// <param name="source">The damage source that deals this damage.</param>
		public virtual void TakeDamageOnHealth (float damage, DamageSource source)
		{

		    //Check if the damage is greater than zero, this is because TakeDamageOnHealth rises OnTakeHealthDamage event.
            if (damage <= 0)
				return ;

            //Subtracts the damage from Current Health.
            CurrentHealth -= damage;

            //Rise OnTakeHealthDamage event and pass it TakeDamageArgs object
            RiseOnTakeHealthDamage(new TakeDamageArgs (damage, MaxHealth, CurrentHealth, source));

            //Checks if the agent ran out of health, if it did then StartDestroy get called.
            if (CurrentHealth <= 0 )
				StartDestroy ();
			
		}

	    /// <summary>
		/// Damages the agent shield directly then returns the remaining damage points.
	    /// </summary>
	    /// <param name="damage">The amount of damage that the agent takes.</param>
	    /// <param name="source">The damage source that deals this damage.</param>
		public virtual float TakeDamageOnShield (float damage, DamageSource source)
		{
			//Checks if the damage is greater than zero, this is because TakeDamageOnShield raises the OnTakeShieldDamage event.
			if (damage <= 0)
				return CurrentShield;

			float shieldLeft = CurrentShield - damage;

			//Checks if what's left from the shield is greater than zero, if it is then zero
			//gets returned for whats's left from the damage and TakeShieldDamage event gets activated.
			if (shieldLeft >= 0) {
				CurrentShield = shieldLeft;
				RiseOnTakeShieldDamage (new TakeDamageArgs (damage, MaxShield, CurrentShield, source));
				return 0;
			}
			//If it isn't; then the CurrentShield is set to zero and the inverse of the shieldLeft gets returned.
            else
            {
                CurrentShield = 0;
		        return Mathf.Abs(shieldLeft);
		    }

		}
        
	    /// <summary>
	    /// Sends when an incoming collider makes contact with this object's collider (2D physics only).
	    /// </summary>
	    protected virtual void OnCollisionEnter2D(Collision2D hitObject)
	    {

	        //Check if the hit object has a component that deals collision damage.
	        //This is because the agent has a rigid body to detect the collision.

	        IDoCollision damageSource = hitObject.gameObject.GetComponent<IDoCollision>();

	        if (damageSource != null)
	            damageSource.DoCollisionDamage(this);            
	    }

        /// <summary>
        /// Destroys the agent in a safe way (rises the right event before the agent gets destroyed).
        /// </summary>
        public virtual void StartDestroy () 
		{
            if (this == null)
                return;

			StopAllCoroutines ();

			RiseOnDestroyStart ();

			Destroy (gameObject);
		}

        /// <summary>
        /// Handles any Subscription to the agent Event by using AllAgentEvents enumerator.
        /// Returns true for successful Subscription and false for failed.
        /// </summary>
        /// <param name="method">A method to get called when the events occur.</param>
        /// <param name="eventType">The type of event.</param>
        /// <returns>True if the Subscription is successful and false if its failed</returns>
		public virtual bool Subscribe ( ShmupDelegate method , AllAgentEvents eventType )
        {
			switch (eventType)
            {
			case AllAgentEvents.Awake:
			{
			    OnStart += method;
			    return true;
			}

            case AllAgentEvents.Destroy:
			{
				OnDestroyStart += method;
				return true;
			}

			case AllAgentEvents.TakeHealthDamage:
				{
					OnTakeHealthDamage += method;
					return true;
				}

			    case AllAgentEvents.TakeCollision:
			    {
			        OnTakeCollision += method;
			        return true;
			    }

                case AllAgentEvents.TakeShieldDamage:
				{
					OnTakeShieldDamage += method;
					return true;;
				}
            default:
                    return false;
            }            
		}

	    /// <summary>
	    /// Prevents any collision damage on the agent.
	    /// </summary>
	    /// <param name="time">Duration</param>
	    private IEnumerator TakeCollisionCoolDown(float time)
	    {
	        _collisionCoolDown = true;

	        yield return new WaitForSeconds(time);

	        _collisionCoolDown = false;

	        yield return null;
	    }

        /// <summary>
        ///  handle the rise of OnTakeHealthDamage event.
        /// </summary>
        /// <param name="args">the data about Take Damage events</param>
		protected void RiseOnTakeHealthDamage ( TakeDamageArgs args )
        {
			if (OnTakeHealthDamage != null)
				OnTakeHealthDamage (args);
		}

        /// <summary>
        /// Handles the rise of the OnTakeShieldDamage event.
        /// </summary>
        /// <param name="args">The data about the Take Damage events.</param>
        protected void RiseOnTakeShieldDamage ( TakeDamageArgs args )
        {
			if (OnTakeShieldDamage != null)
				OnTakeShieldDamage (args);
		}

        /// <summary>
        /// Handles the rise of OnTakeCollision event.
        /// </summary>
        /// <param name="args"Tthe data about Take collision events</param>
        protected void RiseOnTakeCollision(TakeCollisionArgs args)
	    {
	        if (OnTakeCollision != null)
	            OnTakeCollision(args);
        }

        /// <summary>
        /// Handles the rise of OnDestroyStart event.
        /// </summary>
        protected void RiseOnDestroyStart ()
        {
			if (OnDestroyStart != null)
				OnDestroyStart (null);
		}

        /// <summary>
        /// Handles the rise of OnStart event.
        /// </summary>
        protected void RiseOnStart()
	    {
	        if (OnStart != null)
	            OnStart(null);
	    }

    }

}