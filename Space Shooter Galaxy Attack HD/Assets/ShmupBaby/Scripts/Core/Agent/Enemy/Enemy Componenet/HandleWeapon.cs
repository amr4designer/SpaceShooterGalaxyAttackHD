using System.Collections;
using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Adds a component to an Enemy to give a functionality to handle the weapon.  
    /// </summary>
	[RequireComponent(typeof(Enemy))]
	[AddComponentMenu("Shmup Baby/Agent/Enemy/Component/Handle Weapon")]
	public class HandleWeapon : MonoBehaviour , IHandleWeapons {

        /// <summary>
        /// The Enemy that will handle the weapons.
        /// </summary>
        [Tooltip("Connect here the agent item. (This is added so you " +
                 "can add the HandleWeapon as a component of a " +
                 "child object and not necessarily the agent root game object)")]
        public Agent agent;
        
        /// <summary>
        /// All weapons should be a child of an empty GameObject parent underneath the agent, and you 
		/// could have as many weapons as needed parented underneath the WeaponParent.
		/// You can see any of our prefab examples for a better idea
        /// </summary>
		[Space]
		[HelpBox("All weapons should be a child of an empty GameObject parented underneath the agent, and you " +
		         "could have as many weapons as needed parented underneath the WeaponParent")]
        public GameObject WeaponParent;
        /// <summary>
        /// All the weapons that the Enemy controls.
        /// </summary>
		public NormalWeapon[] Weapons;
        /// <summary>
        /// How much time the weapons will remain after the destruction of the agent.
        /// </summary>
		[Tooltip("Because our weapons are particle systems; destroying the weapon will result in destroying the bullets." +
                 "This value acts as a delay for the particle system destruction after the agent destruction")]
        public float BulletRemainTime = 10.0f;        
        /// <summary>
        /// How much time the weapons will take before firing any bullets.
        /// </summary>
		[Tooltip("This time delays the firing of the weaopon, just in case you want a quick way to delay the enemy weapon firing.")]
        public float WeaponStartDelay = 0.0f;


        /// <summary>
        /// Event is a trigger when Weapons start firing.
        /// </summary>
        public event ShmupDelegate OnStartFire ;
        /// <summary>
        /// Event is a trigger when Weapons start firing.
        /// </summary>
		public event ShmupDelegate OnStopFire ;

        /// <summary>
        /// Controls Weapons Firing.
        /// </summary>
		public bool IsFiring
        {
			get
            {
				return _isFiring;
			}
			set
            {
			    //Checks to see if the value that we set has changed from the last time
			    //it got set.
                if (_isFiring != value)
                {
					if (value)
						StartFire ();
					else
						StopFire ();
				}
				_isFiring = value;
			}
		}

        //The back-end field for IsFiring
        private bool _isFiring;


        private void Start ()
        {
            //For an enemy they start firing on awake.
            //IsFiring = true;
            StartCoroutine(DelayFire());

			if (agent == null)
				agent = GetComponent<Agent>();

            //Delays weapon destruction when the agent is destroyed
			agent.OnDestroyStart += DelayWeaponsDestruction;

		}

        IEnumerator DelayFire()
        {
            yield return new WaitForSeconds(WeaponStartDelay);
            IsFiring = true;
            yield return null;
        }

        /// <summary>
        /// Called by the agent when the agent is destroyed.
        /// </summary>
		private void DelayWeaponsDestruction (ShmupEventArgs args)
        {
			DestroyWeapon ();
			agent.OnDestroyStart -= DelayWeaponsDestruction;
		}

        /// <summary>
        /// Fires all Weapons once.
        /// </summary>
		public void Fire ()
        {           
            //Calls fire on all Weapons
            for (int i = 0; i < Weapons.Length; i++)
            {
				Weapons [i].Fire ();
			}           
		}

        /// <summary>
		/// Because we have a particle system acting as weapons; destroying the weapon
        /// will result in destroying the bullets, that's why weapons destruction has to be delayed.
        /// </summary>
		public void DestroyWeapon ()
        {
            //Unparent all Weapons from the Enemy.
            //They are all under the weapon parent so we just unparent the weapon parent.
            WeaponParent.transform.parent = null;

            //Stops all player weapons from firing.
            IsFiring = false;

            //Calls start destroy on every Weapon.
            for (int i = 0; i < Weapons.Length; i++)
			{
				Weapons[i].StartDestroy();
			}

			//Destroys all the weapons after a time delay since all the weapons are under the weapons parent; 
			//We just destroys the weapons parent
            Destroy(WeaponParent, BulletRemainTime);
		}

        /// <summary>
        /// Make all player weapons start firing until StopFire is called.
        /// </summary>
		private void StartFire ()
        {
            //Raise OnStartFire event.
            RiseOnStartFire();

            //Stats firing all Weapons
            for (int i = 0; i < Weapons.Length; i++)
            {
				Weapons [i].IsFiring = true;
			}
		}

        /// <summary>
        /// Makes all player weapons stop firing until StopFire is called.
        /// </summary>
		private void StopFire ()
        {
            //Stops firing on all Weapons
            for (int i = 0; i < Weapons.Length; i++) {
				Weapons [i].IsFiring = false;
			}

            //Raise the OnStopFire event.
            RiseOnStopFire();
		}

        /// <summary>
        /// Handles the rise of OnStartFire event.
        /// </summary>
        protected void RiseOnStartFire ()
        {
			if (OnStartFire != null)
				OnStartFire (null);
		}

        /// <summary>
        /// Handles the rise of OnStopFire event.
        /// </summary>
        protected void RiseOnStopFire()
        {
			if (OnStopFire != null)
				OnStopFire (null);
		}

	}

}