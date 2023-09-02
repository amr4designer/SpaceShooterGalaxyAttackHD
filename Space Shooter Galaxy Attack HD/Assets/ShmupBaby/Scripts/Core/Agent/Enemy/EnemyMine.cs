using UnityEngine;
using System.Collections;

namespace ShmupBaby
{
    /// <summary>
    /// Defines the mine agent behavior.
    /// </summary>
    public interface IMine
    {
        TrackerDetector Tracker { get; }

        /// <summary>
        /// This event should rise when the mine detonation start. 
        /// </summary>
        event ShmupDelegate OnDetonationStart;
        /// <summary>
        /// This event should rise when the mine explosion hits the target.
        /// </summary>
        event ShmupDelegate OnExplosionHitTarget;
    }

    /// <summary>
    /// The base class for all enemy mines.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Enemy/Enemy Mine")]
    public class EnemyMine : Enemy , IMine
    {
        /// <summary>
        /// The radius around the mine,which if the target enters it the mine will start its detonation.
        /// </summary>
        [Header("Mine Settings")]
		[Tooltip("The radius around the mine,which if the target enters it the mine will start its detonation.")]
        [Space]
        public float DetonationRadius;
        /// <summary>
        /// Time in seconds from when the mine start the detonation until its destroyed.
        /// </summary>
		[Tooltip("Time in seconds from when the mine start the detonation until its destroyed.")]
        public float DetonationTime;

        /// <summary>
        /// The radius around the mine, which if the target is inside it when the
        /// mine gets destroyed the player will then take explosion damage.
        /// </summary>
        [Space]
		[Tooltip("The radius around the mine, which if the target is inside it when the" +
				" mine gets destroyed the player will then take explosion damage.")]
        public float ExplosionRadius;

        /// <summary>
        /// The damage the Mine will deal if its explosion hits the target.
        /// </summary>
        [Space]
		[Tooltip("The damage the Mine will deal if its explosion hits the target.")]
        public float Damage;
        /// <summary>
        /// This will make the target take the full damage value if the target was in the center
        /// of  the explosion when the mine explodes, and less damage if the target is far from the center.
        /// </summary>
		[Tooltip("This will make the target take the full damage value if the target was in the center" +
				"of  the explosion when the mine explodes, and less damage if the target is far from the center.")]
        public bool DamagePercentage;
        
        #if UNITY_EDITOR

        /// <summary>
        /// This field is only available in the editor.
        /// </summary>
        [Space]
        [Tooltip("Draw the radius of detection")]
        public bool DrawGizmos;

        #endif

        /// <summary>
        /// Agent event is a trigger when the mine detonation starts.
        /// </summary>
        public event ShmupDelegate OnDetonationStart;
        /// <summary>
        /// Agent event is a trigger when the mine explosion hits the target.
        /// </summary>
        public event ShmupDelegate OnExplosionHitTarget;
        

        /// <summary>
        /// The agent that represents the mine's target.
        /// </summary>
        protected Agent target
        {
            get
            {
               return LevelController.Instance.PlayerComponent;
            }
        }

        /// <summary>
        /// Setting it to true will start the Detonation.
        /// </summary>
        protected bool DetonationMode
        {
            get { return _detonationMode; }
            set
            {
                //Checks if the value is changed from the last time it has been set.
                if (value != _detonationMode)
                {
                    //Trigger OnDetonationStart event.
                    RiseOnDetonationStart();
                    //Explodes the mine after some time.
                    StartCoroutine(MineExplode(DetonationTime));
                }

                _detonationMode = value;
            }
        }

        /// <summary>
        /// The public component that is responsible for tracking the target.
        /// </summary>
        public TrackerDetector Tracker
        {
            get { return tracker; }
        }

        /// <summary>
        /// The protected component that is responsible for tracking the target.
        /// </summary>
        protected TrackerDetector tracker;

        //The back-end field for DetonationMode.
        private bool _detonationMode = false;

        /// <summary>
        /// One of Unity's messages that act the same way as start but gets called before start.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            SetTrackMover();

            if (tracker == null)
                tracker = gameObject.AddComponent<TrackerDetector>();

            tracker.Target = TargetOption.Player;
            tracker.DetectRadius = DetonationRadius;

            tracker.OnStateChange += CheckToDetonate;
        }

        private void SetTrackMover ()
        {
            //If the tracker didn't exist then we add one, and initialize its value.
            TrackMover mover = GetComponent<TrackMover>();

            if (mover == null)
                return;

            if (mover.Tracker != null)
                tracker = mover.Tracker;

            if (tracker == null)
            {
                tracker = gameObject.AddComponent<TrackerDetector>();
                mover.Tracker = tracker;
            }

            mover.targetOption = TargetOption.Player;
        }

        /// <summary>
        /// called by the tracker, check if the target is reached to detonate it.
        /// </summary>
	    private void CheckToDetonate(ShmupEventArgs args)
        {
            TrackStateArgs trackerArgs = (TrackStateArgs)args;

            switch (trackerArgs.State)
            {
                case TrackState.TargetDetected:
                    DetonationMode = true;
                    break;
            }

        }
                
        /// <summary>
        /// Explodes the mine after a time delay and deals damage to the target if it's in the explosion radius.
        /// </summary>
        /// <param name="time"></param>
        protected virtual IEnumerator MineExplode( float time )
        {
            yield return new WaitForSeconds(time);

            RiseOnDestroyStart();

            if (tracker.Distance <= ExplosionRadius)
            {
                DamageTarget();
            }

            Destroy(gameObject);

        }

        /// <summary>
        /// Applies damage to the target.
        /// </summary>
        private void DamageTarget()
        {
            float damage;

            if (DamagePercentage)
                //Sets the damage value relative to the target position to the center of the explosion. 
                damage = (1 - tracker.Distance / ExplosionRadius) * Damage;
            else
                damage = Damage;

            //Calls take damage for the target agent.
            target.TakeDamage(damage, DamageSource.Mine);

            RiseOnExplosionHitTarget();
        }

        /// <summary>
        /// Handles any Subscription to the agent event by using AllAgentEvents enumerator,
        /// and returns true for a successful Subscription and false for a failed one.
        /// </summary>
        /// <param name="method">A method that gets called when events occur</param>
        /// <param name="eventType">The type of event</param>
        public override bool Subscribe(ShmupDelegate method, AllAgentEvents eventType)
        {

            switch (eventType)
            {
                case AllAgentEvents.DetonationStart:
                {
                    OnDetonationStart += method;
                    return true;
                }
                
            }

            return base.Subscribe(method, eventType);

        }

        /// <summary>
        /// Handles the rise of OnExplosionHitTarget event.
        /// </summary>
        protected void RiseOnExplosionHitTarget()
        {
            if (OnExplosionHitTarget != null)
                OnExplosionHitTarget(null);
        }
        /// <summary>
        /// Handles the RiseOnDetonationStart event.
        /// </summary>
        protected void RiseOnDetonationStart()
        {
            if (OnDetonationStart != null)
                OnDetonationStart(null);
        }

        #if UNITY_EDITOR

        protected virtual void OnDrawGizmos()
        {
            if (!DrawGizmos)
                return;

            //Draw a gizmo circle that represents the DetonationRadius.
            Gizmos.color = Color.cyan;
            GizmosExtension.DrawCircle(transform.position, DetonationRadius);

            //Draws a gizmo circle that represent the ExplosionRadius.
            Gizmos.color = Color.red;
            GizmosExtension.DrawCircle(transform.position, ExplosionRadius);
        }

        #endif

    }
}
