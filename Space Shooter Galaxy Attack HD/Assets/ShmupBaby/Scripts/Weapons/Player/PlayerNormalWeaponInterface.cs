using UnityEngine;

namespace ShmupBaby
{

    public interface IUpgradeable
    {
        event ShmupDelegate OnStageChanged;

        int StageIndex { get; set; }

        void Upgrade();
        void Downgrade();
    }

    public interface IPlayerWeapon : IUpgradeable
    {

    }

    public static class PlayerWeaponExtension
    {

        public static string GetPlayerWeaponTargetLayer(this IPlayerWeapon playerWeapon)
        {
            return "Enemy";
        }
        
    }
}