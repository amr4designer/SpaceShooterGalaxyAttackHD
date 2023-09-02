using UnityEngine;
using System.Collections;

namespace ShmupBaby
{
    /// <summary>
    /// component for the normal weapon, plays a sound effect for each shot fired.
    /// according to a ShotEvent.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Weapons/Component/Shot SFX")]
    [RequireComponent(typeof(NormalWeapon))]
    public class ShotSFX : MonoBehaviour
    {
        /// <summary>
        /// the type of shot event that will play the clip.
        /// </summary>
        [Space]
        [Tooltip("The type of shot event that will play the clip")]
        public ShotEvent shotEvent;
        /// <summary>
        /// Clip that will be played on shotEvent.
        /// </summary>
        [Tooltip("reference to the clip that will be played on shotEvent.")]
        public ShmupAudioCip ShotSound;
        /// <summary>
        /// The cooldown time before playing the shot clip again.
        /// </summary>
        [Tooltip("The cooldown time before playing the shot clip again.")]
        public float CoolDown;

        /// <summary>
        /// the weapon this component attached to.
        /// </summary>
        public INormalWeapon MyWeapon;

        /// <summary>
        /// indicates if the SFX is in cooldown mode.
        /// </summary>
        private bool _inCoolDown ;

        /// <summary>
	    /// the Start method is one of Unity's messages that gets called when a new object is instantiated.
	    /// </summary>
        private void Start()
        {

            if (MyWeapon == null)
                MyWeapon = GetComponent<INormalWeapon>();

            //if there is a cool down for the SFX we hook the PlayShot to the weapon event.
            if (CoolDown != 0)
            {
                if (shotEvent == ShotEvent.Fire)
                    MyWeapon.OnShotFire += PlayShot;
                if (shotEvent == ShotEvent.Land)
                    MyWeapon.OnShotLand += PlayShot;

                return;
            }

            if (shotEvent == ShotEvent.Fire)
                MyWeapon.OnShotFire += ShotSound.PlayClip;

            if (shotEvent == ShotEvent.Land)
                MyWeapon.OnShotLand += ShotSound.PlayClip;

        }

        /// <summary>
        /// plays the shot clip if it's not on cooldown time.
        /// </summary>
        private void PlayShot (ShmupEventArgs args)
        {
            if (_inCoolDown)
                return;

            ShotSound.PlayClip(null);

            StartCoroutine(CoolingDown());

        }

        /// <summary>
        /// sets the SFX on cooldown mode.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CoolingDown()
        {
            _inCoolDown = true;
            yield return new WaitForSeconds(CoolDown);
            _inCoolDown = false;
        }
        
    }

}