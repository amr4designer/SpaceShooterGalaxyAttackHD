using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShmupBaby
{

    /// <summary>
    /// PickUp item that the player can pick to provide it with one extra live.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Pick Up/Pick Up Life")]
    public class PickUpAddLife : PickUp
    {

        protected override void PickUpEffect()
        {

            target.RiseOnPickUp(PickUpType.ExtraLife);


            LevelController.Instance.AddPlayerLife();
        }
    }
}