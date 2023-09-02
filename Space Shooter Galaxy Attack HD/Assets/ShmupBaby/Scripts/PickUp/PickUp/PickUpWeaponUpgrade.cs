using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// PickUp item that the player can pick to provide a weapon upgrade and a visual upgrade.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Pick Up/Pick Up Weapon Upgrade")]
    public class PickUpWeaponUpgrade : PickUp
    {     
		protected override void PickUpEffect ()
        {
			target.RiseOnPickUp (PickUpType.WeaponUpgrade);

		    //Raises the OnPick event for the player.
            target.WeaponsUpgrade ();
		}

	}

}
