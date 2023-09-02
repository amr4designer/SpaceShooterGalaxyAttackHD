using UnityEngine;

namespace ShmupBaby {
        
    /// <summary>
    /// WeaponMod stand for Weapon modifiers ,these modifiers
    /// add other functionality to the weapon , 
    /// Rotation Modifier give the weapon a rotation speed around the parent pivot
    /// </summary>
    [RequireComponent(typeof(NormalWeapon))]
    [AddComponentMenu("Shmup Baby/Weapons/Component/Rotation Modifier")]
    public class WeaponMod_Rotation : MonoBehaviour , IWeaponModifier {

        /// <summary>
        /// the pivot of rotation.
        /// </summary>
        [Space]
		[Tooltip("Rotation Pivot")]
		public WeaponModPivot Pivot ;
        /// <summary>
        /// weapon rotation speed in degrees/second. 
        /// </summary>
		[Tooltip("weapon rotation speed in degrees/second")]
		public float RotationSpeed ;

        /// <summary>
        /// the weapon modified by this modifier.
        /// </summary>
		public NormalWeapon MyWeapon
        {
            get { return GetComponent <NormalWeapon> ();  }
        }
        /// <summary>
        /// the parent for this weapon.
        /// </summary>
		public Transform MyWeaponParent
        {
            get { return _weaponParent; }
        }

        /// <summary>
        /// angle that represent how much did the weapon rotate around its pivot.
        /// </summary>
        private float _moveingAngle ;
        /// <summary>
        /// the default angle that the weapon has before rotation
        /// </summary>
        private float _rotationOnZ ;
        /// <summary>
        /// this angle present the position of the weapon from parent by radius degree where 0 mean that the weapon is on the right of the parent.
        /// </summary>
        private float _angleFromParent ;
        /// <summary>
        /// distance from parent in world units
        /// </summary>
        private float _distanceFromParent ;
        /// <summary>
        /// back-end field for the MyWeaponParent.
        /// </summary>
		private Transform _weaponParent ;

        /// <summary>
        /// the Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
        void Start () {

			_rotationOnZ = transform.eulerAngles.z;

			//if the weapon doesn't connect to a parent we return
			if (transform.parent == null || Pivot == WeaponModPivot.Weapon) 
				return;
			
			_weaponParent = transform.parent;

            Vector3 DirectionFromParent = transform.position - _weaponParent.transform.position;

			_distanceFromParent = DirectionFromParent.magnitude;

            DirectionFromParent.Normalize();

            _angleFromParent = Math2D.VectorToDegree ( DirectionFromParent );

		}

        /// <summary>
	    /// one of unity's messages that get called every frame.
	    /// </summary>
		void Update () {

			//increase the MoveingAngle by rotation speed
			_moveingAngle += RotationSpeed * Time.deltaTime;

			if (Pivot == WeaponModPivot.Weapon) {
				transform.rotation = Quaternion.Euler (transform.eulerAngles.x, transform.eulerAngles.y, _rotationOnZ + _moveingAngle );
				return;
			}

			//if the weapon doesn't connect to a parent we return
			if (_weaponParent == null)
				return;
			
			//change the position of the weapon to move around the parent by the MoveingAngle
			transform.position = _weaponParent.position + Math2D.DegreeToVector3 ( _moveingAngle + _angleFromParent ).normalized * _distanceFromParent ;
			//rotate the weapon by MoveingAngle
			transform.rotation = Quaternion.Euler (transform.eulerAngles.x, transform.eulerAngles.y, _rotationOnZ + _moveingAngle );
		}

	}

}
