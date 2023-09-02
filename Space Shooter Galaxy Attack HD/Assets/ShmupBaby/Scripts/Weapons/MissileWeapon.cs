using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// data structure that contains the missile weapon settings for a specific stage.
    /// </summary>
    [System.Serializable]
    public class MissileWeaponStageData : WeaponStageData
    {
        /// <summary>
        /// the missile prefab.
        /// </summary>
        [Space]
        [Tooltip("Reference to the gameObject that represent the missile, this could be a prefab or" +
            "or a gameObject nested under the weapon.")]
        public Object MissileObject;
        /// <summary>
        /// the missile component attached to the missile prefab.
        /// </summary>
        [Tooltip("reference to the missile component attached to the missile prefab.")]
        public Missile MissileScript;
        /// <summary>
        /// the lifetime for the missile in seconds.
        /// </summary>
        [Space]
        [Tooltip("the lifetime for the missile in seconds")]
        public float LifeTime;
        /// <summary>
        /// missile turn speed in degrees per seconds towards its target.
        /// </summary>
        [Tooltip("how fast does the missile rotate towards its target.")]
        public float TurnSpeed;
        /// <summary>
        /// indicates that the missile should be fired at a random start rotation.
        /// </summary>
        [Space]
        [Tooltip("fire the missile at a random start rotation.")]
        public bool RandomStartRotation;
        /// <summary>
        /// indicates that the missile will be destroyed when its target is destroyed.
        /// </summary>
        [Tooltip("destroys the missile at the moment it lost its target.")]
        public bool DestroyOnTargetLost;
        /// <summary>
        /// amount of health the missile has.
        /// </summary>
        [Space]
        [Tooltip("Amount of health the missile has.")]
        public float Health;
    }

    /// <summary>
	/// The missile weapon inherits from the NormalWeapon class , it instantiates objects with 
	/// a missile component to be lunched toward its target.
    /// </summary>
    [AddComponentMenu("")]
    public class MissileWeapon : NormalWeapon
    {
        /// <summary>
        /// The missile prefab for the current stage.
        /// </summary>
        private Object MissileObject;
        /// <summary>
        /// the missile component attached to the missile prefab for the current stage.
        /// </summary>
        private Missile MissileScript;
        /// <summary>
        /// the override for the missile size for the current stage.
        /// </summary>
        private float Size;
        /// <summary>
        /// indicates the missile should be fired at a random start rotation for the current stage.
        /// </summary>
        private bool RandomStartRotation;
        /// <summary>
        /// the lifetime for the missile in seconds for the current stage.
        /// </summary>
        private float LifeTime;

        /// <summary>
        /// fires one shot from the weapon, called by the update when the 
        /// weapon is set to auto fire mode.
        /// </summary>
        public override void Fire()
        {
            FireMissile();
        }

        /// <summary>
        /// instantiates a new missile based on the current stage settings.
        /// </summary>
        /// <returns>the instantiated missile</returns>
        public GameObject FireMissile()
        {
            if (MissileObject == null)
            {
                return null;
            }              

            float angle = 0;

            //checks if random rotation is needed
            if (RandomStartRotation)
            {
                angle = Random.Range(0, 360);
                MissileScript.MyMover.FollowAngle = angle;
            }
            else
            {
                MissileScript.MyMover.FollowAngle = transform.eulerAngles.z;
            }

            //instantiates our main missile
            GameObject missile = (GameObject)Instantiate(MissileObject);
            missile.transform.position = transform.position;

            if (Size != 0)
            {
                missile.transform.localScale = Vector3.one * Size;
            }
                
            RiseOnShotFire(new ShotFireArgs(angle, missile.transform.position));

            //sets timer to destroy instance after time
            Destroy(missile, LifeTime);

            //enables the missile instance
            missile.SetActive(true);

            return missile;
        }

        /// <summary>
        /// this method will be called by the missile instance when it collides with the target.
        /// </summary>
        private void RiseOnMissilesLand(ShmupEventArgs args)
        {
            MissileCollideArgs missileArgs = args as MissileCollideArgs;
            
            if (missileArgs == null)
                return;

            RiseOnShotLand(new ShotLandArgs( missileArgs.Angle , missileArgs.Position , missileArgs.HitObject));
        }

        /// <summary>
        /// sets the weapon to a given stage.
        /// </summary>
        /// <param name="Data">the stage to set the weapon to.</param>
        public override void SetToStage(WeaponStageData Data)
        {
            MissileWeaponStageData data = (MissileWeaponStageData) Data;

            //assigns the private field to the current stage.
            Rate = data.Rate;
            MissileObject = data.MissileObject;
            MissileScript = data.MissileScript;
            RandomStartRotation = data.RandomStartRotation;
            LifeTime = data.LifeTime;
            Size = data.Size;

            //updates the missile component on the missile prefab to match the stage.
            MissileScript.Damage = data.Damage;
            MissileScript.DestroyOnTargetLost = data.DestroyOnTargetLost;

            //if the missile health is below zero this indicate that the missile has Immunity to bullets.
            if (data.Health <= 0)
                MissileScript.ImmunityFromBullet = true;
            else
            {
                MissileScript.Health = data.Health;
                MissileScript.ImmunityFromBullet = false;
            }

            //update the missile mover to match the current stage.
            MissileScript.MyMover.Speed = data.Speed;
            MissileScript.MyMover.TurnSpeed = data.TurnSpeed;
            MissileScript.Side = FiringSide;

            //hook the RiseOnMissilesLand to be called when the missile collides.
            MissileScript.OnCollide += RiseOnMissilesLand;          
        }

        /// <summary>
        /// creates a missile instance that matches the given stagesData
        /// and places it under the missile weapon.
        /// </summary>
        /// <param name="num">the number of the stage to name the missile after</param>
        /// <param name="stagesData">the data for the missile stage.</param>
        protected virtual void InitializeMissile(int num, MissileWeaponStageData stagesData)
        {
            //create the missile
            GameObject missile = stagesData.MissileObject as GameObject;
            Missile missileScript = stagesData.MissileScript;
            Collider2D missileCollider2D = null;

            //name the missile and parent it.
            if (missile == null)
            {
                missile = new GameObject("Missile " + num.ToString());
                missile.transform.parent = transform;
            }

            missileCollider2D = missile.GetComponent<Collider2D>();

            if (missileCollider2D == null)
                missileCollider2D = missile.AddComponent<CircleCollider2D>();

            //add the missile component and missile mover component
            if (missileScript == null)
            {
                missileScript = missile.GetComponent<Missile>();
                if (missileScript == null)
                    missileScript = missile.AddComponent<Missile>();
            }
            if (missileScript.MyMover == null)
            {
                missileScript.MyMover = missile.GetComponent<MissileMover>();
                if (missileScript.MyMover == null)
                    missileScript.MyMover = missile.AddComponent<MissileMover>();
            }

            //disable the instance.
            missile.SetActive(false);

            //update the stage reference to the missile
            stagesData.MissileObject = missile;
            stagesData.MissileScript = missileScript;
        }

    }

}
