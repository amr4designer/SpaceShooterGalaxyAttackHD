using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Defines what data to be passed when a missile collides with a target.
    /// </summary>
    public sealed class MissileCollideArgs : ShmupEventArgs
    {
        /// <summary>
        /// Missile angle when it collides.
        /// </summary>
        public float Angle;
        /// <summary>
        /// Missile position when it collides.
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// The GameObject that gets hit.
        /// </summary>
        public GameObject HitObject;

        /// <summary>
        /// MissileCollideArgs Constructor.
        /// </summary>
        /// <param name="angle">Missile angle when it collides.</param>
        /// <param name="position">Missile position when it collides.</param>
        /// <param name="hitObject">The GameObject that gets hit.</param>
        public MissileCollideArgs(float angle, Vector3 position, GameObject hitObject)
        {
            Angle = angle;
            Position = position;
            HitObject = hitObject;
        }
    }

    /// <summary>
    /// A missile is a simple agent that can be used as a weapon by an enemy or a player.
    /// </summary>
    [RequireComponent(typeof(MissileMover))]
    [AddComponentMenu("")]
    public sealed class Missile : Agent  , IImmunity , IDoCollision
    {

        /// <summary>
        /// A missile Mover is attached to this missile.
        /// </summary>
        [Tooltip("A Missile mover is attached here")]
        public MissileMover MyMover;
        /// <summary>
		/// Missile health points.
        /// </summary>
        [Tooltip("Missile health points.")]
        public float Health;
        /// <summary>
        /// The damage the missile will do to the target
        /// </summary>
        [Tooltip("The damage the missile will do to the target")]
        public float Damage;
        /// <summary>
        /// Make the missile immune from bullets.
        /// </summary>
		[Tooltip("make the missile immune from bullets")]
        public bool ImmunityFromBullet;
        /// <summary>
        /// If the missile target gets destroyed, this will trigger the missile to be destroyed as will.
        /// </summary>
		[Tooltip("If the missile target gets destroyed, this will trigger the missile to be destroyed as will.")]
        public bool DestroyOnTargetLost;

        public AgentSide side;

        public override AgentSide Side
        {
            get { return side; }
            set { side = value; }
        }

        /// <summary>
        /// Agent event is a trigger when the agent collides with another agent.
        /// </summary>
        public event ShmupDelegate OnCollide;

        public float CollisionDamage
        {
            get { return Damage; }
        }
        
        /// <summary>
        /// Agent event is a trigger when agent Immunity is activated.
        /// </summary>
        public event ShmupDelegate OnImmunityActivate;
        /// <summary>
        /// agent event is trigger when agent Immunity is activated.
        /// </summary>
        public event ShmupDelegate OnImmunityDisabled;
        
        public override float CurrentHealth
        {
            get { return Health; }
            set { Health = value; }
        }

        public bool Immunity
        {
            get { return ImmunityFromBullet; }
            set { ImmunityFromBullet = value; }
        }
        
        //Acomponent that will keep track of a given target.
        private Tracker _tracker;

        /// <summary>
		/// Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            MaxHealth = CurrentHealth;
            
            if (ImmunityFromBullet) 
                RiseOnImmunityActivate();
            else
                RiseOnImmunityDisabled();
            
            //Setting up the tracker
            if (MyMover.tracker == null)
            {
                MyMover.tracker = gameObject.AddComponent<Tracker>();
            }
            _tracker = MyMover.tracker;
            
            if (DestroyOnTargetLost)
                _tracker.OnStateChange += DestroyMissile;
        }

        public void DoCollisionDamage(Agent target)
        {
            if (target == null)
                return;

            if (target.Side == Side)
                return;

            RiseOnCollide(new MissileCollideArgs(transform.eulerAngles.z, transform.position, target.gameObject));

            target.TakeDamage(CollisionDamage, DamageSource.Missile);

            StartDestroy();
        }

        /// <summary>
        /// Called by the mover when the target is null, if DestroyOnTargetLost is set to true.
        /// </summary>
        private void DestroyMissile(ShmupEventArgs args)
        {
            TrackStateArgs trackerArgs = (TrackStateArgs)args;

            if (trackerArgs.State == TrackState.TargetLost)
                StartDestroy();
        }

        //Overrides TakeDamage to prevent missiles from taking damage if Immunity is set to true.
        public override void TakeDamage(float damage, DamageSource source)
        {

            if (Immunity)
                return;

            base.TakeDamage(damage, source);
        }

        /// <summary>
        /// Handles any Subscription to the agent event by using AllAgentEvents enumerator,
        /// returns true for a successful Subscription and false for a failed one.
        /// </summary>
        /// <param name="method">A method that gets called when events occur.</param>
        /// <param name="eventType">The type of the event.</param>
        public override bool Subscribe(ShmupDelegate method, AllAgentEvents EventType)
        {

            switch (EventType)
            {
                case AllAgentEvents.ImmunityActivate:
                {
                    OnImmunityActivate += method;
                    return true;
                }

                case AllAgentEvents.ImmunityDisabled:
                {
                    OnImmunityDisabled += method;
                    return true;
                }
                
                case AllAgentEvents.DealCollision:
                {
                    OnCollide += method;
                    return true;
                }

            }

            return base.Subscribe(method, EventType);
        }

        /// <summary>
        ///  Handles the rise of OnCollide event.
        /// </summary>
        private void RiseOnCollide(MissileCollideArgs args)
        {
            if (OnCollide != null)
                OnCollide(args);
        }
        /// <summary>
        /// Handles the rise of OnImmunityActivate event.
        /// </summary>
        private void RiseOnImmunityActivate()
        {
            if (OnImmunityActivate != null)
                OnImmunityActivate(null);
        }
        /// <summary>
        /// Handles the rise of OnImmunityDisabled event.
        /// </summary>
        private void RiseOnImmunityDisabled()
        {
            if (OnImmunityDisabled != null)
                OnImmunityDisabled(null);
        }

    }

}