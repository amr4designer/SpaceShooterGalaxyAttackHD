using UnityEngine;

namespace ShmupBaby
{ 
    /// <summary>
    /// defines the behavior of this package normal weapons.
    /// </summary>
	public interface INormalWeapon 
	{
        /// <summary>
        /// triggered when the weapon is set on auto Fire mode.
        /// </summary>
		event ShmupDelegate OnShotFire;
        /// <summary>
        /// triggered when the weapon is set off auto Fire mode.
        /// </summary>
        event ShmupDelegate OnStopFire;
        /// <summary>
        /// triggered every time the weapon fires a shot.
        /// </summary>       
		event ShmupDelegate OnStartFire;
        /// <summary>
        /// triggered every time a weapon shot lands.
        /// </summary>
        event ShmupDelegate OnShotLand;
        /// <summary>
        /// triggered when the weapon destroy event starts.
        /// </summary>
        event ShmupDelegate OnStartDestroy;

        /// <summary>
        /// the weapon settings for the current stage.
        /// </summary>
		WeaponStageData CurrentStage
        {
            get ;
        }

        /// <summary>
		/// sets the weapon's auto fire mode on and off .
        /// </summary>
		bool IsFiring
        {
            get;
            set;
        }
        /// <summary>
        /// number of shots fired per second.
        /// </summary>
        float Rate
        {
            get;
            set;
        }

        /// <summary>
        /// called in awake to Initialize the weapon settings. 
        /// </summary>
		void Initialize();
        /// <summary>
        /// sets the weapon to a given stage.
        /// </summary>
        /// <param name="Data">the stage to set the weapon to.</param>
		void SetToStage(WeaponStageData data);
        /// <summary>
	    /// destroys the weapon in a safe way (raises the right event before the agent gets destroyed),
        /// it's called by the agent that holds this weapon.
	    /// </summary>
		void StartDestroy();
        /// <summary>
        /// fires one shot from the weapon.
        /// </summary>
		void Fire();
	}

    /// <summary>
    /// define what data to be passed when a shot has been fired.
    /// </summary>
    [AddComponentMenu("")]
    public class ShotFireArgs : ShmupEventArgs
    {
        /// <summary>
        /// the angle of the shot when it gets fired in degrees starting from the right.
        /// </summary>
		public float FireAngle ;
        /// <summary>
        /// the shot position in world space when it gets fired.
        /// </summary>
		public Vector3 FirePosition ;

        /// <summary>
        /// ShotFireArgs constructor.
        /// </summary>
        /// <param name="fireAngle">the angle of the shot when it gets fired in degrees starting from the right.
        /// </summary></param>
        /// <param name="firePosition">the shot position in world space when it gets fired.</param>
		public ShotFireArgs ( float fireAngle , Vector3 firePosition )
        {
			FireAngle = fireAngle ;
			FirePosition = firePosition;
		}

	}

    /// <summary>
    /// defines what data to be passed when a shot land.
    /// </summary>
	public class ShotLandArgs : ShmupEventArgs
    {
        /// <summary>
        /// the angle created by the normal when the shot collide in degrees starting from the right. 
        /// </summary>
		public float LandAngle;
        /// <summary>
        /// the shot landing position in world space.
        /// </summary>
		public Vector3 LandPosition;
        /// <summary>
        /// the object that gets hit by the shot.
        /// </summary>
		public GameObject HitObject;

        /// <summary>
        /// ShotLandArgs constructor.
        /// </summary>
        /// <param name="fireAngle">the angle created by the normal when the shot collides in degree starting from the right. </param>
        /// <param name="firePosition">the shot landing position in world space.</param>
        /// <param name="hitObject">the object that gets hit by the shot.</param>
		public ShotLandArgs ( float fireAngle , Vector3 firePosition ,  GameObject hitObject )
        {
			LandAngle = fireAngle ;
			LandPosition = firePosition;
			HitObject = hitObject;
		}

	}

    /// <summary>
    /// the base class of weapon stage data structure,
    /// a weapon stage will contain the weapon settings for a specific stage.
    /// </summary>
    public class WeaponStageData
    {
        /// <summary>
        /// the Amount of damage per shot.
        /// </summary>
		[Space]
		[Tooltip("The Amount of damage per shot")]
		public float Damage ;
        /// <summary>
        /// How many time this weapon will fire per Seconds.
        /// </summary>
		[Space]
		[Tooltip("how many time this weapon will fire per Seconds")]
		public float Rate ;
        /// <summary>
        /// speed of the shot Unit per Seconds.
        /// </summary>
		[Tooltip("speed of the shot Unit per Sec")]
		public float Speed ;
        /// <summary>
        /// the size of the shot.
        /// </summary>
	    [Tooltip("the size of the shot")]
	    public float Size;
    }
	
    /// <summary>
    /// the base class for most of the weapons in this package.
    /// </summary>
	[AddComponentMenu("")]
	public class NormalWeapon : MonoBehaviour , INormalWeapon
	{
        /// <summary>
        /// indicates if the weapon is initialized in the editor.
        /// </summary>
		[Space]
        [Tooltip("Indicates if the weapon is initialized, if it's unchecked the weapon will be" +
            "initialized on awake.")]
		public bool Initialized ;
        /// <summary>
        /// triggered when the weapon is set on auto Fire mode.
        /// </summary>
		public event ShmupDelegate OnStartFire ;
        /// <summary>
        /// triggered when the weapon is set off auto Fire mode.
        /// </summary>
		public event ShmupDelegate OnStopFire ;
        /// <summary>
        /// triggered every time the weapon fires a shot.
        /// </summary>
		public event ShmupDelegate OnShotFire ;
        /// <summary>
        /// triggered every time a weapon shot land.
        /// </summary>
		public event ShmupDelegate OnShotLand ;
        /// <summary>
        /// triggered when the weapon destroy event starts.
        /// </summary>
		public event ShmupDelegate OnStartDestroy;

        /// <summary>
        /// the weapon settings for the current stage.
        /// </summary>
		public virtual WeaponStageData CurrentStage
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// the side at which this weapons belong.
        /// </summary>
        public virtual AgentSide FiringSide
        {
            get;
            set;
        }

        /// <summary>
		/// sets the weapon's auto fire mode on and off .
        /// </summary>
        public bool IsFiring
        {
			get
            {
				return _isFiring;
			}
			set
            {
				if (_isFiring != value)
                {
					if (value)
                    {
                        RiseOnStartFire();
                    }
                    else
                    {
                        RiseOnStopFire();
                    }						
				}
                _isFiring = value;
			}
		}

        /// <summary>
        /// number of shots fired per second.
        /// </summary>
	    public float Rate
	    {
	        get
	        {
	            if (timeBetweenShoots == 0)
                {
                    return 0;
                }
                else
                {
                    return 1 / timeBetweenShoots;
                }	                
	        }
	        set
	        {
	            if (value <= 0)
	            {
	                timeBetweenShoots = 0;
                }
	            else
	            {
	                timeBetweenShoots = 1 / value;
	            }
	        }
	    }
        	    
        /// <summary>
        /// the time for firing the next shot.
        /// </summary>
        protected float nextShot;
        /// <summary>
        /// time in Seconds between shots fired.
        /// </summary>
        protected float timeBetweenShoots;
        /// <summary>
        /// back-end field for Firing shots.
        /// </summary>
        private bool _isFiring;


	    protected virtual void Awake ()
        {
			Rate = CurrentStage.Rate;
			// Refresh the next shot time on start.
			nextShot = Time.time;

	        // This initialize function adjusts the weapon if it's not adjusted manually.
            if (!Initialized)
	        {
	            Initialize();
            }
	       
	        SetToStage(CurrentStage);
	    }


		protected virtual void Update ()
        {           
            if (!(Time.time >= nextShot) || !IsFiring || timeBetweenShoots <= 0)
            {
                return;
            }
		        
		    Fire ();

		    //updates the time for the next shot
		    nextShot = Time.time + timeBetweenShoots ;
		}

        /// <summary>
        /// fires one shot from the weapon.
        /// </summary>
		public virtual void Fire ()
        {
			
		}

        /// <summary>
        /// sets the weapon to a given stage.
        /// </summary>
        /// <param name="Data">the stage to set the weapon to.</param>
		public virtual void SetToStage (WeaponStageData Data)
        {

		}

        /// <summary>
        /// called in awake to Initialize the weapon settings. 
        /// </summary>
		public virtual void Initialize ()
        {

		}

        /// <summary>
	    /// destroys the weapon in a safe way (raises the right event before the agent gets destroyed),
        /// called by the agent that holds this weapon.
	    /// </summary>
		public virtual void StartDestroy ()
        {
			RiseOnStartDestroy();
		}

        protected bool CheckForFriendlyFire (Agent target)
        {
            if (target == null)
            {
                return true;
            }
                
            if (target.Side == FiringSide)
            {
                return true;
            }
                
            return false;
        }

        /// <summary>
        /// handles the rise of OnShotFire event.
        /// </summary>
        /// <param name="Args">the shot data that trigger this event.</param>
        protected void RiseOnShotFire ( ShotFireArgs Args )
        {
			if (OnShotFire != null)
            {
                OnShotFire(Args);
            }				
		}
        /// <summary>
        /// handle the rise of OnShotLand event.
        /// </summary>
        /// <param name="Args">the shot data that triggers this event.</param>
		protected void RiseOnShotLand (ShotLandArgs Args)
        {
			if (OnShotLand != null)
				OnShotLand (Args);
		}
        /// <summary>
        /// handles the rise of OnStartFire event.
        /// </summary>
		protected void RiseOnStartFire ()
        {
			if (OnStartFire != null)
            {
                OnStartFire(null);
            }				
		}
        /// <summary>
        /// handle the rise of OnStopFire event.
        /// </summary>
		protected void RiseOnStopFire  ()
        {
			if (OnStopFire  != null)
				OnStopFire  (null);

		}
        /// <summary>
        /// handles the rise of the OnStartDestroy event.
        /// </summary>
		protected void RiseOnStartDestroy ()
        {
			if (OnStartDestroy != null)
            {
                OnStartDestroy(null);
            }				
		}

	}

}