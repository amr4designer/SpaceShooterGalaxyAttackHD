//this is the Agent main interfaces and other core element for Shmup Baby.

namespace ShmupBaby
{
    /// <summary>
    /// A list of all events for the Agent class and every class derived from it.
    /// </summary>
	//  If any new derived classes were created, any new events they define should have a member in this enumerator. 
    public enum AllAgentEvents
    {
        Awake,
        Destroy,
        TakeHealthDamage,
        TakeShieldDamage,
        TakeCollision,
        DealCollision,
        ImmunityActivate,
        ImmunityDisabled,
        WeaponsStartFire,
        WeaponsStopFire,
        WeaponUpgrade,
        WeaponDowngrade,
        Drop,
        PickUp,
        DetonationStart
    }
    
    #region Agent Base

    /// <summary>
    /// The possible types for the agent. 
    /// </summary>
    public enum AgentSide
    {
        Allied,
        Player,
        Enemy
    }

    /// <summary>
    /// Defines the agent health points members.
    /// </summary>
    public interface IHealth
    {
        /// <summary>
		/// The current Health points value.
        /// </summary>
        float CurrentHealth { get; set; }

        /// <summary>
        /// The value for the max Health points
        /// </summary>
        float MaxHealth { get; set; }

    }

    /// <summary>
    /// Defines the agent ability to take health damage.
    /// </summary>
    public interface ITakeHealthDamage : IHealth
    {
        /// <summary>
        /// This event should rise whenever the agent gets destroyed
        /// for being out of health points.
        /// </summary>
        event ShmupDelegate OnDestroyStart;

        /// <summary>
        /// This event should rise whenever the agent takes health damage.
        /// </summary>
        event ShmupDelegate OnTakeHealthDamage;

        /// <summary>
        /// Damages the agent directly on health then checks if
        /// the health is below zero to start agent destruction.
        /// </summary>
        /// <param name="damage">The amount of damage that the agent takes.</param>
        /// <param name="source">The damage source that deals this damage.</param>
        void TakeDamageOnHealth(float damage, DamageSource source);

        /// <summary>
        /// Damage the agents by considering any type of defenses that the agent have
        /// </summary>
        /// <param name="damage">The amount of damage that the agent takes.</param>
        /// <param name="source">The damage source that deals this damage.</param>
        void TakeDamage(float damage, DamageSource source);

        /// <summary>
        /// This method gets called when the agent takes lethal damage that leaves him out of health.
        /// </summary>
        void StartDestroy();

    }

    /// <summary>
    /// Defines what data to be passed when the agent takes damage by the OnTakeDamage event.
    /// </summary>
    public class TakeDamageArgs : ShmupEventArgs
    {
        /// <summary>
        /// The amount of damage that the agent takes.
        /// </summary>
        public float Damage;

        /// <summary>
        /// The agent's maximum value for the affected bar.
        /// </summary>
        public float MaxBar;

        /// <summary>
        /// The agent affected bar value.
        /// </summary>
        public float CurrentBar;

        /// <summary>
        /// The damage source that deals this damage.
        /// </summary>
        public DamageSource Source;

        /// <summary>
        /// TakeDamageArgs constructor
        /// </summary>
        /// <param name="damage">The amount of damage that the agent takes.</param>
        /// <param name="maxBar">The agent maximum value for the affected bar.</param>
        /// <param name="currentBar">The agent Affected bar value.</param>
        /// <param name="source">The damage source that deals this damage.</param>
        public TakeDamageArgs(float damage, float maxBar, float currentBar, DamageSource source)
        {
            Damage = damage;
            MaxBar = maxBar;
            CurrentBar = currentBar;
            Source = source;
        }

    }

    /// <summary>
    /// Defines the agent shield points members.
    /// </summary>
    public interface IShield
    {
        /// <summary>
        /// The current value for Shield points.
        /// </summary>
        float CurrentShield { get; }

        /// <summary>
        /// The value for the max Shield points
        /// </summary>
        float MaxShield { get; }

    }

    /// <summary>
    /// All types of damage sources that the agent could be damaged from.
    /// </summary>
	// This enumerator is used to pass the damage type for the TakeDamage events.
    public enum DamageSource
    {
        Bullet,
        Missile,
        Collision,
        Mine
    }

    /// <summary>
    /// Defines the agent ability to take shield damage.
    /// </summary>
    public interface ITakeShieldDamage : IShield, ITakeHealthDamage
    {

        /// <summary>
        /// This event should rise whenever the agent takes shield damage.
        /// </summary>
        event ShmupDelegate OnTakeShieldDamage;

        /// <summary>
        /// Damages the agent directly on the shield then returns the
        /// remaining damage points.
        /// </summary>
        /// <param name="damage">The amount of damage that the agent takes.</param>
        /// <param name="source">The damage source that deals this damage.</param>
        float TakeDamageOnShield(float damage, DamageSource source);

    }

    #region Collision

    /// <summary>
    /// Defines what data to be passed when the player takes damage by the OnTakeCollision event.
    /// </summary>
    public class TakeCollisionArgs : ShmupEventArgs
    {

        /// <summary>
        /// How many seconds for the next collision to take place.
        /// </summary>
        public float TimeBetweenCollision;

        public TakeCollisionArgs(float timeBetweenCollision)
        {
            TimeBetweenCollision = timeBetweenCollision;
        }

    }

    /// <summary>
    /// Defines the agent ability to take collision damage.
    /// </summary>
    public interface ITakeCollision
    {
        /// <summary>
        /// Is this agent allowed to take collision damage or not.
        /// </summary>
        bool AllowCollisionDamage { get; }
        /// <summary>
        /// Returns the side of this agent.
        /// </summary>
        AgentSide Side { get; }

        /// <summary>
        /// This event should rise whenever the Player takes collision damage.
        /// </summary>
        event ShmupDelegate OnTakeCollision;
        
    }

    #endregion

    #endregion

    #region Agent Extention

    /// <summary>
    /// Defines the agent ability to handle a weapon.
    /// </summary>
    public interface IHandleWeapons
    {
        /// <summary>
        /// This event should rise whenever the agent starts shooting.
        /// </summary>
        event ShmupDelegate OnStartFire;

        /// <summary>
        /// This event should rise whenever the agent stops shooting.
        /// </summary>
        event ShmupDelegate OnStopFire;

        /// <summary>
        /// Toggle the weapons fire On/Off
        /// </summary>
        bool IsFiring { get; set; }

        /// <summary>
        /// Fires all the weapons once.
        /// </summary>
        void Fire();

        /// <summary>
        /// A safe way to destroy all weapons
        /// </summary>
        void DestroyWeapon();
    }

    /// <summary>
    /// Defines the agent ability to be Immune from damage.
    /// </summary>
    public interface IImmunity
    {
        /// <summary>
        /// This event should rise whenever the agent Immunity effect is activated.
        /// </summary>
        event ShmupDelegate OnImmunityActivate;

        /// <summary>
        /// This event should rise whenever the agent immunity is disabled.
        /// </summary>
        event ShmupDelegate OnImmunityDisabled;

        /// <summary>
        /// Return true if immunity is activated.
        /// </summary>
        bool Immunity { get; }

    }

    /// <summary>
    /// Defines what data to be passed when the agent activates immunity from damage.
    /// </summary>
    public class ImmunityArgs : ShmupEventArgs
    {

        /// <summary>
        /// Remaining time for the Immunity effect.
        /// </summary>
        public float time;

        /// <summary>
        /// ImmunityArgs constructor
        /// </summary>
        /// <param name="Time">Remaining time for the Immunity effect.</param>
        public ImmunityArgs(float Time)
        {
            time = Time;
        }

    }

    #endregion
    
}