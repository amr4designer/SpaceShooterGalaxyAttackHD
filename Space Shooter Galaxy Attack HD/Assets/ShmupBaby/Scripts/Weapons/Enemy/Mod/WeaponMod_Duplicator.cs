using UnityEngine;

namespace ShmupBaby { 

    /// <summary>
    /// The rotation options for the duplicated weapons.
    /// </summary>
	public enum WeaponModRoation
    {
        Radial ,
        None
    }

	/// <summary>
	/// WeaponMod stand for Weapon modifiers  , these modifiers 
	/// add other functionality to the weapon , 
	/// Duplicator Modifier creates other instances of the weapon around the player
	/// </summary>
	[RequireComponent(typeof(NormalWeapon))]
    [AddComponentMenu("Shmup Baby/Weapons/Component/Duplicator Modifier")]
    public class WeaponMod_Duplicator : MonoBehaviour , IWeaponModifier
	{
        /// <summary>
        /// the rotation of the duplicated weapon.
        /// </summary>
		[Space]
		[Tooltip("duplicated weapon Rotation")]
		public WeaponModRoation Rotation ;
        /// <summary>
        /// the pivot of duplication.
        /// </summary>
		[Space]
		[Tooltip("Duplication Pivot")]
		public WeaponModPivot Pivot ;
        /// <summary>
        /// the number of duplicated weapons.
        /// </summary>
		[Space]
		[Tooltip("number of weapon duplicates")]
		public int DuplicateNum;

        /// <summary>
        /// the weapon modified by this modifier.
        /// </summary>
        public NormalWeapon MyWeapon { get; private set; }
        /// <summary>
        /// the parent for this weapon.
        /// </summary>
        public Transform MyWeaponParent
        {
            get { return _weaponParent; }
        }

        /// <summary>
        /// the duplicate weapon handles.
        /// </summary>
        private NormalWeapon[] _duplicates ;
        /// <summary>
        /// the parent for this weapon.
        /// </summary>
        Transform _weaponParent ;

        /// <summary>
        /// the Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
		void Start ()
		{
            MyWeapon = gameObject.GetComponent<NormalWeapon> ();

			//Make sure there is a weapon to duplicate
			if (MyWeapon == null) {
				Debug.Log ("no weapon found to duplicate");
				return;
			}

			_duplicates = new NormalWeapon[DuplicateNum];

            //the weapon needs to have a parent
			_weaponParent = transform.parent;
			if (_weaponParent == null)
				return;

            //for the duplicates to stop Fire when the weapon welder is destroyed
            MyWeapon.OnStartDestroy += StopDublicateFire;

            //get the position info from the parent
            Vector2 displacementFromParent = transform.position - _weaponParent.transform.position;
            float distanceFromParent = displacementFromParent.magnitude;
            float angleFromParent = 0;

			if ( Pivot == WeaponModPivot.WeaponParent )
				angleFromParent = Math2D.VectorToDegree (displacementFromParent.normalized );

			float stepAngle = 360f / (DuplicateNum+1);
			float RotationOnZ = transform.eulerAngles.z;

			//duplicate main weapon
			for (int i = 0; i < DuplicateNum; i++) {
				
				//instantiate position and rotate 
				GameObject weapon = GameObject.Instantiate (gameObject , _weaponParent);

				if ( Pivot == WeaponModPivot.WeaponParent )
					weapon.transform.position = _weaponParent.position + Math2D.DegreeToVector3 ( stepAngle*(i+1) + angleFromParent )* distanceFromParent;

				if ( Pivot == WeaponModPivot.Weapon )
					weapon.transform.position = transform.position;

				if ( Rotation == WeaponModRoation.Radial )
					weapon.transform.rotation = Quaternion.Euler ( transform.eulerAngles.x , transform.eulerAngles.y , RotationOnZ + stepAngle*(i+1) );

				//reference the enemy weapon for each duplicate to our array of duplicated enemyWeapon
				_duplicates[i] = weapon.GetComponent<NormalWeapon> ();
                _duplicates[i].IsFiring = true;

                //Destroy the WeaponMod_Duplicator from the new instances.
                Destroy ( weapon.GetComponent <WeaponMod_Duplicator> () );

			}
		}

        /// <summary>
        /// this method will be called when the weapon welder is destroyed
        /// </summary>
        void StopDublicateFire ( ShmupEventArgs args ) {
			
			//iterate through all duplicates and stop their fire
			for (int i = 0; i < _duplicates.Length; i++) {
				_duplicates [i].IsFiring = false;
			}

		}

	}

}

