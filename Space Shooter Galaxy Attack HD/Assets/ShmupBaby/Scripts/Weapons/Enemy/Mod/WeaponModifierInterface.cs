using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// the pivots used by weapon modifiers. 
    /// </summary>
    public enum WeaponModPivot
    {
        WeaponParent,
        Weapon
    }

    /// <summary>
    /// defines the behavior for the weapon modifier.
    /// </summary>
    public interface IWeaponModifier
    {
        /// <summary>
        /// the weapon modified by this modifier.
        /// </summary>
        NormalWeapon MyWeapon { get; }
        /// <summary>
        /// the parent for this weapon.
        /// </summary>
        Transform MyWeaponParent { get; }
    }

}
