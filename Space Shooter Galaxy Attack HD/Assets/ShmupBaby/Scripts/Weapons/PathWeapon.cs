using UnityEngine;
using System.Collections.Generic;

namespace ShmupBaby
{
    /// <summary>
    /// data structure contains the path weapon settings for a specific stage.
    /// </summary>
    [System.Serializable]
	public class PathWeaponStageData : WeaponStageData
    {
        /// <summary>
        /// The radius of the bullet collider.
        /// </summary>
		[Space]
		[Tooltip("The size of the collider")]
		public float ColliderSize;
        /// <summary>
        /// The curve that will affect the bullet velocity by its lifetime.
        /// </summary>
	    [Space]
		[Tooltip("Path for bullets ( affected by Lifetime and CurveRange )")]
		public AnimationCurve Curve = new AnimationCurve (new Keyframe (0, 0));
        /// <summary>
        /// multiplier for the curve velocity on the X-axis. 
        /// </summary>
		[Tooltip("Scales the width of the curve")]
		public float CurveRange = 1;
        /// <summary>
        /// lifetime of the bullets in seconds.
        /// </summary>
	    [Tooltip("life time of the shot in Sec")]
	    public float Lifetime;
        /// <summary>
        /// The bullet material.
        /// </summary>
        [Space]
		[Tooltip("Reference to the bullet material")]
		public Object BulletMaterial;
        /// <summary>
        /// The bullet trail material.
        /// </summary>
		[Space]
		[Tooltip("Reference to the trail material (leave it empty to disable the trail)")]
		public Object TrailMaterial ;
        /// <summary>
        /// lifetime for the trail.
        /// </summary>
		[Tooltip("lifetime for the trail (needs to be adjusted when the speed changes)")]
		public float TrailTime = 0.1f;
        /// <summary>
        /// Width of the bullet trail.
        /// </summary>
		[Tooltip("Width of the trail (affected with the size of the bullets)")]
		public float TrailWidth = 1;

        /// <summary>
        /// bullet rotation speed in degrees per seconds.
        /// </summary>
		[Space]
		[Tooltip("Bullets rotation speed in degree per seconds.")]
		public float BulletRotationSpeed ;
        /// <summary>
        /// Offsets the bullet start Rotation
        /// </summary>
        [Tooltip("Offset the bullet Rotation")]
        public float BulletRotationOffset;
    }

    /// <summary>
    /// One of the normal weapons, uses the ParticleSystem to emit particles
    /// with change in velocity by a curve. 
    /// </summary>
	[RequireComponent(typeof(ParticleSystem))]
	[AddComponentMenu("")]
	public class PathWeapon : NormalWeapon
	{
        /// <summary>
        /// the weapons particle system.
        /// </summary>
        [Space]
        [Tooltip("Optional reference to the ParticleSystem, it should be assigned for faster a instantiation of the Weapons")]
        public ParticleSystem MyPS;
        /// <summary>
        /// the weapon particle system renderer.
        /// </summary>
        [Tooltip("Optional reference to the ParticleSystemRenderer, it should be assigned for a faster instantiation of the Weapon")]
        public ParticleSystemRenderer MyPSR;

        /// <summary>
        /// the layer which the bullet will collide with.
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
        protected List<ParticleCollisionEvent> shotCollisionEvent ;


        protected override void Awake ()
		{
			shotCollisionEvent = new List<ParticleCollisionEvent>();
			base.Awake ();
		}

        /// <summary>
        /// fires one shot from the weapon, called by the update when the 
        /// weapon is set to auto fire mode.
        /// </summary>
        public override void Fire ()
        {
			RiseOnShotFire (new ShotFireArgs (transform.eulerAngles.z, transform.position));
			MyPS.Emit (1);
		}

        /// <summary>
        /// One of Unity's messages that gets called when the particle collides with another object
        /// this needs sendCollisionMessages to be enabled inside the Collision module.
        /// </summary>
        /// <param name="hitTarget">the object hit by the bullet.</param>
        protected virtual void OnParticleCollision (GameObject HitTarget)
        {
			MyPS.GetCollisionEvents (HitTarget, shotCollisionEvent);
            
			ParticleCollisionEvent collision = shotCollisionEvent [shotCollisionEvent.Count-1];

			RiseOnShotLand 
                (new ShotLandArgs (Math2D.VectorToDegree (collision.normal), 
                collision.intersection , HitTarget));
		}

        /// <summary>
        /// called in awake to initialize the weapons settings.
        /// </summary>
        //every thing done inside this function can be done from the inspector of the ParticleSystem.
        [ContextMenu("Initialize")]
        public override void Initialize ()
        {
			if ( MyPS == null)
            {
                MyPS = gameObject.GetComponent<ParticleSystem>();
            }
				
			if ( MyPSR == null )
            {
                MyPSR = gameObject.GetComponent<ParticleSystemRenderer>();
            }
				

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
			//set the simulationSpace to world ( if we don't set it the bullet will be effected when player moves)
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
			//specific the collision layer to player layer 
			Collision.collidesWith = LayerMask.GetMask (TargetLayer);

			//adjusting the Shape module
			//enable the module first
			Shape.enabled = true;
			//set shapeType to edge and set that edge length to 0.01f so the particles will start at the same point and have same direction
			Shape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
			Shape.radius = 0.01f;

			//adjusting the Emission module
			//enable the module first
			Emission.enabled = false;

			//adjusting the Velocity Over Lifetime module
			//enable the module first
			Velocity.enabled = true;

			Initialized = true;
		}

        /// <summary>
        /// sets the weapon to a given stage.
        /// </summary>
        /// <param name="Data">the stage to set the weapon to.</param>
		public override void SetToStage (WeaponStageData Data)
		{
			PathWeaponStageData data = Data as PathWeaponStageData;

            //Update the rate
		    Rate = data.Rate;

			//to edit ParticleSystem first we get all the modules from our ParticleSystem
			//Main module
			ParticleSystem.MainModule main = MyPS.main;
			//Velocity Over Lifetime module
			ParticleSystem.VelocityOverLifetimeModule velocity = MyPS.velocityOverLifetime;
			//Rotation By Speed module
			ParticleSystem.RotationBySpeedModule rotation = MyPS.rotationBySpeed;
			//Trail module
			ParticleSystem.TrailModule trail = MyPS.trails;
			//Collision module
			ParticleSystem.CollisionModule Collision = MyPS.collision;

			Collision.radiusScale = data.ColliderSize;

			//adjusting the main module
			//sets the size of each particle.
			main.startSizeMultiplier = data.Size;
			//sets the lifetime for particles.
			main.startLifetimeMultiplier = data.Lifetime;
			//sets the speed for the particles ( units / seconds )
			main.startSpeedMultiplier = data.Speed;
            //offsets the bullet rotation
			main.startRotationZMultiplier = data.BulletRotationOffset*Mathf.Deg2Rad;

            //adjusting the Velocity Over Lifetime module
            //set the curve for the particle to follow in the X-axis only.
            velocity.x = new ParticleSystem.MinMaxCurve (data.CurveRange, data.Curve);
			//defines a Default curve to set it to the Y & Z axis
			ParticleSystem.MinMaxCurve defaultCurve = new ParticleSystem.MinMaxCurve (0, new AnimationCurve (new Keyframe[] { new Keyframe ( 0 , 0 ) , new Keyframe ( 1 , 0) }));
			//we set the Default curve to prevent any errors
			velocity.y = defaultCurve;
			velocity.z = defaultCurve;

			//check for bullets rotation if it's needed
			if (data.BulletRotationSpeed > 0)
            {
				//adjusting the Rotation By Speed module
				//enable the module first
				rotation.enabled = true;
				//we only need rotate in z - axis
				rotation.separateAxes = true;
				//define the rotation speed on the z - axis 360 deg per sec
				rotation.zMultiplier = Mathf.Deg2Rad*data.BulletRotationSpeed;
			}
            else
            {
				//disable the module if its not needed
				rotation.enabled = false;
			}

			//check for bullet Trail if its needed
			if (data.TrailMaterial != null)
            {
				//adjusting the Trail module
				//enable the module first
				trail.enabled = true;
				//sets the Trail material
				MyPSR.trailMaterial = (Material)data.TrailMaterial;
				//sets the life time for the trail
				trail.lifetime = data.TrailTime;
				//sets the width of the curve and set the curve to get thin over time
				trail.widthOverTrail = new ParticleSystem.MinMaxCurve 
                    (data.TrailWidth, new AnimationCurve 
                    (new Keyframe[] 
                    {new Keyframe (0, 1),
					new Keyframe (1, 0)}));
			}
            else
            {
				// Disables the module if it's not needed.
				trail.enabled = false;
			}

            // Sest the bullet material.
            if ( data.BulletMaterial != null ) MyPSR.material = (Material)data.BulletMaterial;
		}

	}

}