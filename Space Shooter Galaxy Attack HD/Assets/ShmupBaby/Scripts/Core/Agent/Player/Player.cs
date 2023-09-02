using System.Collections;
using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Player class is the main class for the GameObject that is controlled by the player.
    /// </summary>
	[SelectionBase]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(PlayerMover))]
	[AddComponentMenu("Shmup Baby/Agent/Player/Player")]
	public sealed class Player : Agent , IPlayer
	{
        /// <summary>
        /// The value for the maximum health points
        /// </summary>
		[HelpBox("The maximum health the player can reach without taking health upgrade pickups.")]
		public float maxHealth = 10;
        /// <summary>
		/// The current Health points value.
        /// </summary>
		public float Health = 10;

        /// <summary>
	    /// The value for the maximum shield points
	    /// </summary>
		[HelpBox("The maximum shield the player can reach without taking shield upgrade pickups")]
		public float maxShield = 10;

        /// <summary>
        /// The current value for the Shield points.
        /// </summary>
        public float Shield = 10;

        /// <summary>
        /// Indicates that the player should take collision damage from the enemy.
        /// </summary>
		[HelpBox("Allows the player to take collision damage from the enemy")]
		public bool allowCollisionDamage;
        /// <summary>
        /// Cool-down time to prevent collision damage from taking place on every frame.
        /// </summary>
        [Tooltip("This is the cool-down time to prevent collision damage from taking place on every frame")]
		public float TimeBetweenCollision;

        /// <summary>
        /// The time from the player spawning until it starts taking damage.
        /// </summary>
		[HelpBox("The time from the player spawning until it starts taking damage.")]
		public float InvincibilityTime;

        /// <summary>
        /// The gameObject that contains all the player weapons nested underneath it.
        /// </summary>
	    [HelpBox("All weapons should be a child of an empty GameObject parent underneath the agent, and you " +
	             "could have as much weapons as needed parented underneath the WeaponParent")]
        public GameObject WeaponParent;
        /// <summary>
        /// All weapons used by the player.
        /// </summary>
		public NormalWeapon[] Weapons;
        /// <summary>
		/// Because we have a particle system acting as weapons; destroying a weapon will result in destroying the bullets
		/// it has fired as well. This value acts as a delay for the particle system destruction after agent destruction.
        /// </summary>
		[Tooltip("Because we have a particle system acting as weapons; destroying a weapon will result in destroying the bullets" +
				 "it has fired as well. This value acts as a delay for the particle system destruction after agent destruction.")]
        public float BulletRemainTime = 10;
        
        /// <summary>
	    /// Agent event is a trigger when agent activates the immunity effect.
	    /// </summary>
		public event ShmupDelegate OnImmunityActivate ;
	    /// <summary>
	    /// Agent event is a trigger when agent disables the immunity effect.
	    /// </summary>
		public event ShmupDelegate OnImmunityDisabled ;
	    /// <summary>
	    /// Agent event is a trigger when agent starts firing weapons.
	    /// </summary>
		public event ShmupDelegate OnStartFire ;
	    /// <summary>
	    /// Agent event is a trigger when agent stops firing weapons.
	    /// </summary>
		public event ShmupDelegate OnStopFire ;
	    /// <summary>
	    /// Agent event is a trigger when agent weapons gets upgraded.
	    /// </summary>
		public event ShmupDelegate OnWeaponUpgrade ;
	    /// <summary>
	    /// Agent event is a trigger when agent weapons get downgraded.
	    /// </summary>
		public event ShmupDelegate OnWeaponDowngrade ;
	    /// <summary>
	    /// Agent event is a trigger when agent picks a pickup.
	    /// </summary>
		public event ShmupDelegate OnPick ;

        /// <summary>
        /// A player Mover is attached to this player.
        /// </summary>
	    public PlayerMover mover { get; set; }
        /// <summary>
		/// The current buff the player is effected with. (A timed ability increase).
        /// </summary>
	    public PlayerBuff ActiveBuff { get; set; }
	    /// <summary>
	    /// Return true if immunity is activated and false if its not.
	    /// </summary>
        public bool Immunity { get ; private set; }
        /// <summary>
        /// The current stage of the player weapons.
        /// </summary>
		public int CurrentStage { get ; private set; }

        //Overrides properties because there is no property support in the inspector.
	    public override float MaxHealth
	    {
	        get { return maxHealth; }
	        set { maxHealth = value; }
	    }
        public override float CurrentHealth
        {
            get { return Health; }
            set { Health = value; }
        }
        public override float MaxShield
        {
            get { return maxShield; }
            set { maxShield = value; }
        }
        public override float CurrentShield
        {
            get { return Shield; }
            set { Shield = value; }
        }

	    public override AgentSide Side
	    {
	        get { return AgentSide.Player; }
	    }

	    public override bool AllowCollisionDamage
	    {
	        get { return allowCollisionDamage; }
	        protected set { allowCollisionDamage = value; }
	    }

	    /// <summary>
        /// Sets all player weapons to fire.
        /// </summary>
        public bool IsFiring { 
			get {
				return _isFiring;
			}
			set {
				if (_isFiring != value) {
					if (value == true)
						StartFire ();
				    else
				        StopFire();
				    
				}	
				_isFiring = value;
			}
		}

        //the back-end field for IsFiring.
	    private bool _isFiring ;

        /// <summary>
        /// One of Unity's messages that act the same way as start but gets called before start.
        /// </summary>
		private void  Awake()
		{
            MaxHealth = maxHealth;
		    CurrentHealth = Health;

			MaxShield = maxShield;
			CurrentShield = Shield;

            _timeBetweenCollision = TimeBetweenCollision;
            
			//Checks if the Player have a reference to the mover, if it does not; we try to get it.
            //It could be assigned by the LevelController.
			if ( mover == null )
				mover = GetComponent<PlayerMover> ();
            
            //Actives the immunity effect on the player. We don't want the player to get destroyed when spawns.
            StartCoroutine (ActiveImmunity (InvincibilityTime));
		}

        /// <summary>
        /// One of Unity's messages that gets called on every frame.
        /// </summary>
	    private void Update()
	    {
            IsFiring = InputManager.Instance.GetInput(PlayerID.Player1).Fire;
	    }

	    /// <summary>
	    /// Handles any Subscription to the agent event by using AllAgentEvents enumerator,
	    /// returns true for a successful Subscription and false for a failed one.
	    /// </summary>
	    /// <param name="method">A method that gets called when events occur</param>
	    /// <param name="eventType">The type of event.</param>
	    public override bool Subscribe ( ShmupDelegate method , AllAgentEvents EventType ) {
            
            switch (EventType) {

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
                
			case AllAgentEvents.WeaponsStartFire:
				{
				    OnStartFire += method;
				    return true;
                }

			case AllAgentEvents.WeaponsStopFire:
				{
				    OnStopFire += method;
				    return true;
                }

			case AllAgentEvents.WeaponUpgrade:
				{
				    OnWeaponUpgrade += method;
				    return true;
                }

			case AllAgentEvents.WeaponDowngrade:
				{
				    OnWeaponDowngrade += method;
				    return true;
                }

			case AllAgentEvents.PickUp:
				{
				    OnPick += method;
				    return true;
                }

			}

			return base.Subscribe (method, EventType);
		}

        /// <summary>
        /// Upgrades all player weapon to the next stage.
        /// </summary>
		public void WeaponsUpgrade ()
		{
            //Raises OnWeaponUpgrade event.
            RiseOnWeaponUpgrade();

            //Calls Upgrade for all player weapons.
            for (int i = 0; i < Weapons.Length; i++) {

			    if (Weapons[i] == null)
			        continue;

			    IPlayerWeapon weapon = Weapons [i] as IPlayerWeapon;
			    weapon.Upgrade ();

			}

            //update the current stage for the player weapon.
			CurrentStage++;

		}

	    /// <summary>
	    /// Returns all player weapons to the previous stage.
	    /// </summary>
		public void WeaponsDowngrade ()
		{

            //Raises OnWeaponDowngrade event.
            RiseOnWeaponDowngrade();

            //Calls Downgrade for all player weapons.
            for (int i = 0; i < Weapons.Length; i++) {

			    if (Weapons[i] == null)
			        continue;

			    IPlayerWeapon weapon = Weapons [i] as IPlayerWeapon;
			    weapon.Downgrade ();

			}

		    //Updates the current stage for the player weapon.
            if (CurrentStage > 0)
				CurrentStage--;

		}

        /// <summary>
        /// Sets all Player weapons to a specific index. 
        /// </summary>
        /// <param name="value">Index of the stage.</param>
		public void SetWeaponsStage ( int value ) 
		{
            //Checks if the stage index is valid. 
			if (value < 0)
				return;

            //Sets stage for all player weapons.
			for (int i = 0; i < Weapons.Length; i++) {

			    if (Weapons[i] == null)
			        continue;

			    IPlayerWeapon weapon = Weapons [i] as IPlayerWeapon;
			    weapon.StageIndex = value;

			}

            //update the CurrentStage for the player
            CurrentStage = value;

		}

        /// <summary>
        /// Destroys all Player weapons.
        /// </summary>
		public void DestroyWeapon() {
			DelayWeaponsDestruction();
        }

        /// <summary>
		/// Because we have a particle system acting as weapons; destroying the weapon will
        /// result in destroying the bullets, that's why weapon destruction has to be delayed.
        /// </summary>
		private void DelayWeaponsDestruction ( )
		{
			//Stops all player weapons from firing.
			IsFiring = false;

			//Calls start destroy on every player weapon.
			for (int i = 0; i < Weapons.Length; i++) {

				Weapons [i].StartDestroy ();

			}

			//UnParent all player weapons from the player.
			//Since they are all under one parent; we just UnParent the weapon parent.
			if (WeaponParent != null)
				WeaponParent.transform.parent = null;

			//Destroys all the weapons after a delay since all the weapons are under one parent;
			//we just destroy the weapon parent
			Destroy (WeaponParent, BulletRemainTime);

		}

        /// <summary>
        /// Fires all player weapons once.
        /// </summary>
		public void Fire () {

            //calls fire on all player weapons.
			for (int i = 0; i < Weapons.Length; i++) {
				Weapons [i].Fire ();
			}
            
		}

        /// <summary>
        /// Makes all player weapon start firing until StopFire is called.
        /// </summary>
		private void StartFire () {

            //Raises the OnStartFire event.
            RiseOnStartFire();

            //Stats firing all player weapons
			for (int i = 0; i < Weapons.Length; i++) {
				Weapons [i].IsFiring = true;
			}

		}

	    /// <summary>
	    /// Makes all player weapons stops fire until StopFire is called.
	    /// </summary>
		private void StopFire () {

	        //Stops firing all player weapons
            for (int i = 0; i < Weapons.Length; i++) {
				Weapons [i].IsFiring = false;
			}

	        //Raises the OnStopFire event.
            RiseOnStopFire();

		}
        
	    /// <summary>
	    /// Damages the agent by considering what type of defenses that agent has
	    /// </summary>
	    /// <param name="damage">The amount of damage that the agent takes.</param>
	    /// <param name="source">The damage source that deals this damage.</param>
		public override void TakeDamage ( float damage , DamageSource source )
		{
            //If immunity is activated then the player can't take any damage.
			if (Immunity)
				return;
            
			base.TakeDamage (damage, source);
		}
        
	    /// <summary>
	    /// Destroys the agent in a safe way (raises the right event before the agent gets destroyed).
	    /// </summary>
		public override void StartDestroy () 
		{
			DelayWeaponsDestruction ();

			base.StartDestroy ();

		}

	    /// <summary>
	    /// Prevents any damage to take place on the agent.
	    /// </summary>
	    /// <param name="time">Duration of Immunity</param>
		public IEnumerator ActiveImmunity ( float time )
		{
		    yield return null;

            //Sets immunity to true then raises the OnImmunityActivate event. 
            Immunity = true;

			RiseOnImmunityActivate (new ImmunityArgs ( time ));

            //Wait for the Immunity duration.
            yield return new WaitForSeconds (time);

            //Sets the Immunity to false then raise OnImmunityDisabled event. 
            RiseOnImmunityDisabled(new ImmunityArgs ( time ));

			Immunity = false;

		}
        
        /// <summary>
        /// Handles the rise of the OnPickUp event.
        /// </summary>
        public void RiseOnPickUp (PickUpType pickup) {

			if (OnPick != null)
				OnPick (new PickArgs (pickup));

		}
        /// <summary>
        /// Handles the rise of OnImmunityActivate event.
        /// </summary>
        private void RiseOnImmunityActivate ( ShmupEventArgs args ) {

		    if (OnImmunityActivate != null)
		        OnImmunityActivate(args);
		    

		}
        /// <summary>
        /// Handles the rise of OnImmunityDisabled event.
        /// </summary>
        private void RiseOnImmunityDisabled ( ShmupEventArgs args ) {

			if (OnImmunityDisabled != null)
				OnImmunityDisabled (args);

		}
        /// <summary>
        /// Handles the rise of OnStartFire event.
        /// </summary>
        private void RiseOnStartFire () {

			if (OnStartFire != null)
				OnStartFire (null);

		}
        /// <summary>
        /// Handles the rise of OnStopFire event.
        /// </summary>
        private void RiseOnStopFire () {

			if (OnStopFire != null)
				OnStopFire (null);

		}
        /// <summary>
        /// Handles the rise of OnWeaponUpgrade event.
        /// </summary>
        private void RiseOnWeaponUpgrade () {

			if (OnWeaponUpgrade != null)
				OnWeaponUpgrade (null);

		}
        /// <summary>
        /// Handles the rise of OnWeaponDowngrade event.
        /// </summary>
        private void RiseOnWeaponDowngrade () {

			if (OnWeaponDowngrade != null)
				OnWeaponDowngrade (null);

		}

	}

}