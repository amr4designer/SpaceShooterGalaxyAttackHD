using System.Collections;
using UnityEngine;

namespace ShmupBaby
{	    	
	/// <summary>
	/// WeaponMod stand for Weapon modifiers, these modifiers
	/// add other functionality to the weapon , 
	/// Burst Control Modifier control when a weapon should Fire
	/// </summary>
	[RequireComponent(typeof(NormalWeapon))]
    [AddComponentMenu("Shmup Baby/Weapons/Component/Rate Control Modifier")]
    public class WeaponMod_RateControl : MonoBehaviour , IWeaponModifier
	{
        /// <summary>
        /// this type is only used inside WeaponMod_FireController 
        /// </summary>
        [System.Serializable]
        public class FireSteps
        {
            [Tooltip("Time of the step in seconds.")]
            public float Time;
            [Tooltip("the weapon rate for this step.")]
            public float Rate;
        }


        /// <summary>
        /// the stages to control the weapon fire rate over time.
        /// </summary>
        [Space]
		[Tooltip("Define weapon fire rate over time")]
		public FireSteps[] Steps ;
        /// <summary>
        /// indicate that the stages will loop when they are finished.
        /// </summary>
		[Space]
		[Tooltip("loop all the steps above")]
		public bool Loop ;

        /// <summary>
        /// the weapon modified by this modifier.
        /// </summary>
		public NormalWeapon MyWeapon { get; set; }
        /// <summary>
        /// the parent for this weapon.
        /// </summary>
		public Transform MyWeaponParent
        {
            get
            {
                return MyWeapon.transform.parent;
            }
        }


		private void Start()
        {
	        MyWeapon = gameObject.GetComponent<NormalWeapon> ();

		    MyWeapon.OnStartDestroy += OnDestroyStart;

            StartCoroutine (FireWithControl());
		}

        /// <summary>
        /// Coroutine that control the weapon fire rate over time,
        /// according to the defined Steps.
        /// </summary>
		private IEnumerator FireWithControl ()
        {		
			do
            {			
			    for (int i = 0; i < Steps.Length; i++)
			    {
                
			        MyWeapon.Rate = Steps[i].Rate;

				    yield return new WaitForSeconds (Steps[i].Time);

			    }
				//if loop is checked we keep looping through the steps
			}
            while (Loop);

			yield return null;
		}

        /// <summary>
        /// called when the weapon welder gets destroyed.
        /// </summary>
		void OnDestroyStart ( ShmupEventArgs args )
		{
            //stop the weapon from fire
            MyWeapon.Rate = 0;
		    MyWeapon.IsFiring = false;
            StopAllCoroutines ();
		}

	}

}

