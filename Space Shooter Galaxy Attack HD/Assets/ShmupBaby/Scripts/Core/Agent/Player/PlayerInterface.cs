using System.Collections;

namespace ShmupBaby
{

    #region Weapons

    /// <summary>
    /// Defines the player ability to Upgrade Weapons
    /// </summary>
    public interface IUpgradeWeapon : IHandleWeapons
    {
        /// <summary>
        /// This event should rise whenever the Player weapons get upgraded.
        /// </summary>
        event ShmupDelegate OnWeaponUpgrade;
        /// <summary>
        /// This event should rise whenever the Player weapons get downgraded
        /// </summary>
        event ShmupDelegate OnWeaponDowngrade;

        /// <summary>
        /// The current stage of the weapon starting by 0.
        /// </summary>
        int CurrentStage { get; }

        /// <summary>
        /// Upgrade the weapon to the next stage.
        /// </summary>
        void WeaponsUpgrade();

        /// <summary>
        /// Returns the weapon to the previous stage
        /// </summary>
        void WeaponsDowngrade();

        /// <summary>
        /// Sets the weapon to a specific stage.
        /// </summary>
        /// <param name="value">Stage index</param>
        void SetWeaponsStage(int value);

    }

    #endregion
    
    #region PickUp

    /// <summary>
    /// List of all kinds of pickup in this package and any new defintions for future pickups.
    /// should be added in here.
    /// </summary>
    public enum PickUpType
    {
        Heal,
        Shield,
        HealthUpgrade,
        ShieldUpgrade,
        Speed,
        SpeedUpgrade,
        Point,
        WeaponUpgrade,
        ExtraLife
    }

    /// <summary>
    /// Defines what data to be passed when the OnPick event is activated.
    /// </summary>
    public class PickArgs : ShmupEventArgs
    {

        /// <summary>
        /// The type of effect that the pickUp gives.
        /// </summary>
        public PickUpType PickUpType;

        public PickArgs(PickUpType pickup)
        {
            PickUpType = pickup;
        }

    }

    /// <summary>
    /// define the player ability to pick a pickup.
    /// </summary>
    public interface IPick
    {
        /// <summary>
        ///  this event should be rise whenever the Player pick a pickup.
        /// </summary>
        event ShmupDelegate OnPick;

        /// <summary>
        ///  handle the rise of OnPick event
        /// </summary>
        void RiseOnPickUp(PickUpType pickup);
    }

    #endregion

    /// <summary>
    /// Defines the agent ability to be immune from damage for a given time.
    /// </summary>
    public interface IImmunityByTime : IImmunity
    {

        /// <summary>
        /// Prevents any damage to take place on the agent.
        /// </summary>
        /// <param name="time">Duration of immunity</param>
        IEnumerator ActiveImmunity(float time);

    }
    
    /// <summary>
    /// Defines the Player agent 
    /// </summary>
    public interface IPlayer : IUpgradeWeapon, IImmunityByTime, IPick
    {

        PlayerMover mover { get; }

    }
}
