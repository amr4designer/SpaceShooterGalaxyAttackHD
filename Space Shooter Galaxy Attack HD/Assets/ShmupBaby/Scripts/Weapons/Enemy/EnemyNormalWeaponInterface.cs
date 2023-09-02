using UnityEngine;

namespace ShmupBaby
{
    public interface IEnemyWeapon
    {

    }

    public static class EnemyWeaponExtension
    {
        public static string GetEnemyWeaponTargetLayer(this IEnemyWeapon enemyWeapon)
        {
            return "Player";
        }        
    }
}
