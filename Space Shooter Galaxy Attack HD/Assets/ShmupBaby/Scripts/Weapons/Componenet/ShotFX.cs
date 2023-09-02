using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// event options for the shots.
    /// </summary>
	public enum ShotEvent
    {
        Fire ,
        Land
    }

    /// <summary>
    /// component for the normal weapon, creates an FX
    /// according to a ShotEvent.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Weapons/Component/Shot FX")]
    public class ShotFX : MonoBehaviour {

        /// <summary>
        /// the shot event that will trigger the FX instantiation. 
        /// </summary>
        [Space]
        [Tooltip("The type of shot event that will instantiate the shot FX")]
        public ShotEvent shotEvent;
        /// <summary>
        /// FX prefab.
        /// </summary>
		[Space]
        [Tooltip("reference to the FX prefab")]
        public Object FX ;
        /// <summary>
        /// the lifetime for the FX instances in seconds.
        /// </summary>
		[Space]
        [Tooltip("the life time for the FX instances in sec")]
		public float FXLifetime;
        /// <summary>
        /// indicates if the FX instance should be position constrained to their target.
        /// </summary>
		[Space]
        [Tooltip("should the instance be constrained to its position with the target.")]
		public bool pointConstrain;
        /// <summary>
        /// offsets of the FX instance on the X-Axis from it's original position
        /// </summary>
		[Space]
        [Tooltip("offsets of the FX instance on the X-Axis from its original position.")]
		public float OffestOnX ;
        /// <summary>
        /// offset of the FX instance on the Y-Axis from it's original position
        /// </summary>
        [Tooltip("offset of the FX instance on the Y-Axis from its original position.")]
        public float OffestOnY ;
        /// <summary>
        /// offset of the FX instance rotation on the Z-Axis.
        /// </summary>
		[Space]
        [Tooltip("offset of the FX instance rotation on the Z-Axis")]
		public float AngleOffest ;

        /// <summary>
        /// the weapon this component is attached to.
        /// </summary>
        public INormalWeapon MyWeapon;


        /// <summary>
	    /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
	    /// </summary>
        void Start () {

			if (MyWeapon == null)
				MyWeapon = GetComponent<INormalWeapon> ();

			if ( shotEvent == ShotEvent.Fire )
				MyWeapon.OnShotFire += CreateShotFireFX;

			if ( shotEvent == ShotEvent.Land )
				MyWeapon.OnShotLand += CreateShotLandFX;

		}

        /// <summary>
        /// Creates an FX when a shot is fired from the weapon.
        /// </summary>
        private void CreateShotFireFX ( ShmupEventArgs args ) {

			ShotFireArgs Args = args as ShotFireArgs;

			GameObject fx;

            //constrains the FX instance position.
		    if (pointConstrain)
		    {
		        fx = Instantiate(FX) as GameObject;
		        fx.AddComponent<PointConstrain>().Target = transform;
		    }
		    else
				fx = Instantiate (FX) as GameObject;

            //rotates the FX instance to match the shooting normal
			fx.transform.rotation = Quaternion.Euler (0, 0, Args.FireAngle + AngleOffest );
			fx.transform.position = new Vector3 ( Args.FirePosition.x + OffestOnX , Args.FirePosition.y + OffestOnY , Args.FirePosition.z );

            //destroys the instance after its lifetime is over.
			if (FXLifetime > 0)
				Destroy (fx, FXLifetime);

		}

        /// <summary>
        /// creates an FX when a shot lands.
        /// </summary>
		void CreateShotLandFX ( ShmupEventArgs args ) {

			ShotLandArgs Args = args as ShotLandArgs;

			GameObject fx;

            //constrains the FX instance position.
            if (pointConstrain)
		    {
		        fx = Instantiate(FX) as GameObject;
		        fx.AddComponent<PointConstrain>().Target = Args.HitObject.transform;
		    }
		    else
				fx = Instantiate (FX) as GameObject;

            //rotates the FX instance to match the landing normal
            fx.transform.rotation = Quaternion.Euler (0, 0, Args.LandAngle + AngleOffest );
			fx.transform.position = new Vector3 ( Args.LandPosition.x + OffestOnX , Args.LandPosition.y + OffestOnY , Args.LandPosition.z );

            //destroys the instance after its life time is over.
            if (FXLifetime > 0)
				Destroy (fx, FXLifetime);

		}


	}

}