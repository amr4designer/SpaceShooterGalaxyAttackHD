using UnityEngine;
using System.Collections.Generic;

namespace ShmupBaby
{

    /// <summary>
    /// Data structure that contains the radial weapon settings for a specific stage.
    /// </summary>
    [System.Serializable]
    public class RadialWeaponStageData : WeaponStageData
    {
        /// <summary>
        /// the radius of the bullet collider.
        /// </summary>
		[Space]
		[Tooltip("the size of the collider")]
		public float ColliderSize;      
        /// <summary>
        /// number of bullets fired every time the weapon is fired. 
        /// </summary>
        [Space]
        [Tooltip("Number of the bullet lines")]
        public int BulletNum = 1;
        /// <summary>
        /// Length of the fire arc in degrees.
        /// </summary>
        [Tooltip("Length of the arc in degrees")]
        public float ArcLength = 0;
        /// <summary>
        /// Length of the fire arc in world units.
        /// </summary>
        [Tooltip("Use this value if you want to shift the fire point from  the center")]
        public float ArcRadius = 1;
        /// <summary>
        /// offsets the fire arc's start angle.
        /// </summary>
        [Tooltip("Arc start location by degree")]
        public float ArcShiftAngle = 90;
        /// <summary>
        /// Distance that bullets travel in world units before it gets destroyed.
        /// </summary>
        [Tooltip("Distance that bullets travel in world units")]
        public float Distance = 10;
        /// <summary>
        /// the bullet material.
        /// </summary>
        [Space]
        [Tooltip("reference to the bullet material")]
        public Object BulletMaterial;
        /// <summary>
        /// the bullet trail material.
        /// </summary>
        [Space]
        [Tooltip("reference to the trail material (leave it empty to disable the trail)")]
        public Object TrailMaterial;
        /// <summary>
        /// lifetime for the trail.
        /// </summary>
        [Tooltip("lifetime for the trail (need to be adjusted when the speed changes)")]
        public float TrailTime = 0.1f;
        /// <summary>
        /// width of the bullet trail.
        /// </summary>
        [Tooltip("width of the trail (affected with the size of bullets)")]
        public float TrailWidth = 1;

        /// <summary>
        /// bullets rotation speed in degrees per seconds.
        /// </summary>
        [Space]
        [Tooltip("bullets rotation speed degree per seconds")]
        public float BulletRotationSpeed;
        /// <summary>
        /// Offsets the bullet start Rotation
        /// </summary>
        [Tooltip("Offsets the bullet Rotation")]
        public float BulletRotationOffset;
    }

    /// <summary>
    /// one of the normal weapons, it uses the ParticleSystem to emit particles as radial bullets. 
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    [AddComponentMenu("")]
    public class RadialWeapon : NormalWeapon
    {
        /// <summary>
        /// the weapon particle system.
        /// </summary>
        [Space]
        [Tooltip("Optional reference to the ParticleSystem, it should be assigned for a faster instantiation of the Weapon")]
        public ParticleSystem MyPS;
        /// <summary>
        /// the weapon particle system renderer.
        /// </summary>
        [Tooltip("Optional reference to the ParticleSystemRenderer, it should be assign for a faster instantiation of the Weapon")]
        public ParticleSystemRenderer MyPSR;

        /// <summary>
        /// The layer which the bullet will collide with.
        /// </summary>
        protected virtual string TargetLayer
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// the collision event for the bullets in the scene. 
        /// </summary>
        private List<ParticleCollisionEvent> shotCollisionEvent;
        /// <summary>
        /// number of bullets fired every time the weapon is fired for the current stage.
        /// </summary>
        private int BulletNum;
        /// <summary>
        /// offset the fire arc start angle for the current stage.
        /// </summary>
        private float ArcShiftAngle;
        /// <summary>
        /// bullet speed for the current frame.
        /// </summary>
        private float Speed;
        /// <summary>
        /// bullet size for the current stage.
        /// </summary>
		private float Size;
        /// <summary>
        /// Length of the fire arc in world units for the current stage.
        /// </summary>
        private float ArcRadius;
        /// <summary>
        /// the angle between bullets for the current stage.
        /// </summary>
        private float StepAngle;
        /// <summary>
        /// Offsets the bullet start Rotation for the current stage.
        /// </summary>
        private float RotationOffset;
        /// <summary>
        /// uses add position and velocity to the particle before it emits it.
        /// </summary>
        private ParticleSystem.EmitParams BulletEmitParams;

        /// <summary>
        /// one of Unity's messages that gets called before Start.
        /// </summary>
        protected override void Awake()
        {
            shotCollisionEvent = new List<ParticleCollisionEvent>();           
            base.Awake();
        }

        /// <summary>
        /// fires one shot from the weapon, called by the update when the 
        /// weapon is set to auto fire mode.
        /// </summary>
        public override void Fire()
        {            
            //calculates the angle that is produced by enemy rotation and ArcShiftAngle
            float shiftingAngle = Mathf.Deg2Rad * (ArcShiftAngle + transform.eulerAngles.z);
            
            //iterates through the number of Bullets
            for (int i = 0; i < BulletNum; i++)
            {
                BulletEmitParams = new ParticleSystem.EmitParams();

                float angle = i * StepAngle + shiftingAngle;
                //calculates the bullet direction from the rotation of the StepAngle and ShiftingAngle
                Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

                //just adding the position location to the bullet direction ( because it's relative to the ( 0 , 0 , 0 ) location
                //then we amplify the direction by arcRadius so we give the bullet an offset from the center of the enemy.
                BulletEmitParams.position = transform.position + dir * ArcRadius;
                //calculate the velocity of the bullet by multiply the speed by direction 
                BulletEmitParams.velocity = dir * Speed;
                //edit the start rotation of the bullet to match the fire angle.
                BulletEmitParams.rotation =  RotationOffset - (StepAngle * i + shiftingAngle)  * Mathf.Rad2Deg;
				BulletEmitParams.startSize = Size;

                RiseOnShotFire(new ShotFireArgs(angle, BulletEmitParams.position));

				//after filling the particle parameters, it's ready to be emitted.
                MyPS.Emit(BulletEmitParams, 1);
            }

        }

        /// <summary>
        /// One of Unity's messages that gets called when the particle collides with another object
        /// this need sendCollisionMessages to be enabled inside the Collision module.
        /// </summary>
        /// <param name="hitTarget">the object hit by the bullet.</param>
        protected virtual void OnParticleCollision(GameObject hitTarget)
        {
            //rise OnShotLand event.
            MyPS.GetCollisionEvents(hitTarget, shotCollisionEvent);
            ParticleCollisionEvent collision = shotCollisionEvent[shotCollisionEvent.Count - 1];

            RiseOnShotLand(new ShotLandArgs(Math2D.VectorToDegree(collision.normal), collision.intersection, hitTarget));
        }

        /// <summary>
        /// called in awake to Initialize the weapon settings.
        /// </summary>
        // Everything done inside this function can be done from the inspector of the ParticleSystem.
        [ContextMenu("Initialize")]
        public override void Initialize()
        {

            //check for the ParticleSystem reference if it exists 
            if (MyPS == null)
                //if not we find it
                MyPS = gameObject.GetComponent<ParticleSystem>();

            //check for ParticleSystemRenderer reference if it exists 
            if (MyPSR == null)
                //if not we find it
                MyPSR = gameObject.GetComponent<ParticleSystemRenderer>();

            //to edit ParticleSystem first we get all the modules from our ParticleSystem
            //Main module
            ParticleSystem.MainModule Main = MyPS.main;
            //Shape module
            ParticleSystem.ShapeModule Shape = MyPS.shape;
            //Collision module
            ParticleSystem.CollisionModule Collision = MyPS.collision;
            //Velocity Over Lifetime module
            ParticleSystem.VelocityOverLifetimeModule Velocity = MyPS.velocityOverLifetime;
            //Emission module
            ParticleSystem.EmissionModule Emission = MyPS.emission;

            //adjusting the main module
            //set the simulationSpace to world ( if we don't set it the bullet we be effected with ship move )
            Main.simulationSpace = ParticleSystemSimulationSpace.World;


            //adjusting the Collision module
            //enable the module first
            Collision.enabled = true;
            //set the Collision to world so it will collide with any object in the world
            Collision.type = ParticleSystemCollisionType.World;
            //set the maxKillSpeed to zero , so the bullet is destroyed when it hit the collider
            Collision.maxKillSpeed = 0;
            //set the Collision mode to 2D , more accurate and lighter
            Collision.mode = ParticleSystemCollisionMode.Collision2D;
            //sendCollisionMessages is important for OnParticleCollision to work
            Collision.sendCollisionMessages = true;
            //enemy weapon needs to hit the player which has a dynamic RigidBody
            Collision.enableDynamicColliders = true;
			//specifies the collision layer to player layer 
            Collision.collidesWith = LayerMask.GetMask(TargetLayer);

            //adjusting the Shape module
            //disable the module first
            Shape.enabled = false;
            
            //adjusting the Emission module
            //enable the module first
            Emission.enabled = false;
            
            Initialized = true;
        }

        /// <summary>
        /// set the weapon to a given stage.
        /// </summary>
        /// <param name="Data">the stage to set the weapon to.</param>
        public override void SetToStage(WeaponStageData Data)
        {
            RadialWeaponStageData data = Data as RadialWeaponStageData;

            //Updates the rate
            Rate = data.Rate;
            //Updates Bullet Speed;
            Speed = data.Speed;
            //Updates Arc Radius;
            ArcRadius = data.ArcRadius;
            //updates the number of bullets
            BulletNum = data.BulletNum;
            //Updates Shift Arc Radius;
            ArcShiftAngle = data.ArcShiftAngle;
            //Updates the rotation Offset
            RotationOffset = data.BulletRotationOffset;
			//Updates bullet Size
			Size = data.Size;
            //Updates Step Angle
            if (BulletNum <= 1)
                StepAngle = 0;
            else
            {
                if ( data.ArcLength % 360f != 0 )
                    StepAngle = data.ArcLength / (BulletNum - 1);
                else
                    StepAngle = data.ArcLength / BulletNum;
            }

            StepAngle *= Mathf.Deg2Rad;

            //to edit the ParticleSystem first we get all the modules from our ParticleSystem.
            //Main module
            ParticleSystem.MainModule main = MyPS.main;
            //Rotation By Speed module
            ParticleSystem.RotationBySpeedModule rotation = MyPS.rotationBySpeed;
            //Trail module
            ParticleSystem.TrailModule trail = MyPS.trails;
			//Collision module
			ParticleSystem.CollisionModule Collision = MyPS.collision;

			Collision.radiusScale = data.ColliderSize;

            //adjusting the main module
            //set the lifetime for particles
            main.startLifetimeMultiplier = data.Distance / data.Speed;
            
            //check for bullet rotation if it's needed
            if (data.BulletRotationSpeed > 0)
            {
                //adjusting the Rotation By Speed module
                //enable the module first
                rotation.enabled = true;
                //we only need to rotate in the Z-axis
                rotation.separateAxes = true;
                //defines the rotation speed on the Z-axis 360 degrees per second.
                rotation.zMultiplier = Mathf.Deg2Rad * data.BulletRotationSpeed;
            }
            else
            {
                //disable the module if it's not needed
                rotation.enabled = false;
            }

            //checks for bullet Trail if it's needed
            if (data.TrailMaterial != null)
            {
                //adjusting the Trail module
                //enables the module first
                trail.enabled = true;
                //sets the Trail material
                MyPSR.trailMaterial = (Material)data.TrailMaterial;
                //sets the life time for the trail
                trail.lifetime = data.TrailTime;
                //sets the width of the curve and set the curve to get thin over time
                trail.widthOverTrail = new ParticleSystem.MinMaxCurve(data.TrailWidth, new AnimationCurve(new Keyframe[] {
                    new Keyframe (0, 1),
                    new Keyframe (1, 0)
                }));
            }
            else
            {
                //disables the module if its not needed
                trail.enabled = false;
            }

            if (data.BulletMaterial != null)
            {
                MyPSR.material = (Material)data.BulletMaterial;
            }
        }

    }

}
