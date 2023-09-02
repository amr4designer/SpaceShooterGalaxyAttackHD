using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Defines the agent ability to do collision damage.
    /// </summary>
    public interface IDoCollision
    {
        /// <summary>
        /// This event should rise whenever the agent collides with another opposite agent.
		/// I.e. when the enemy collides with the player, or the player collides with the enemy. 
        /// </summary>
        event ShmupDelegate OnCollide;
        /// <summary>
        /// The amount of collision damage that this agent will deal.
        /// </summary>
        float CollisionDamage { get; }
        /// <summary>
        /// Damages the agent with the CollisionDamage.
        /// </summary>
        /// <param name="target">An agent that you collide with.</param>
        void DoCollisionDamage(Agent target);

    }

    /// <summary>
    /// Defines the agent's ability to get Immunity depending on a region.
    /// </summary>
    public interface IImmunityByRegion : IImmunity
    {
        /// <summary>
        /// Controls the agent Immunity depending on an agent position in a given region
        /// this method should be used in Update.
        /// </summary>
        /// <param name="region">Region of Immunity</param>
        /// <param name="inRegion">Gives an Immunity effect when the agent is inside the region</param>
        void ImmunityByRegion(Rect region, bool inRegion);

    }

    #region Drop

    /// <summary>
    /// Defines the drop item that gets dropped by the enemy.
    /// </summary>
    [System.Serializable]
    public sealed class Drop
    {
        /// <summary>
        /// The drop possibility from 0% to 100%.
        /// </summary>
        [Tooltip("A probability for the DropObject to be dropped")]
        [Range(0, 100)]
        public int DropChance;
        /// <summary>
        /// The drop Object that will get instantiated when the Enemy is destroyed.
        /// </summary>
        [Space]
        [Tooltip("A prefab for the drop item, make sure it has a PickUp Component and PickUpMover")]
        public Object DropObject;

    }

    /// <summary>
    /// Defines what data to be passed when an enemy drops a PickUp.
    /// </summary>
    public sealed class DropArgs : ShmupEventArgs
    {
        /// <summary>
        /// The spawn position of the drop item when it's dropped.
        /// </summary>
        public Vector3 SpawnPosition;
        /// <summary>
		/// The position which the PickUp item will travel to after spawning,
        /// and before starting the PickUp behavior.
        /// </summary>
        public Vector3 DropPosition;

        /// <summary>
        /// DropArgs constructor.
        /// </summary>
        /// <param name="spawnPosition">The spawn position of the drop item when it's dropped.</param>
        /// <param name="dropPosition">The position which the PickUp item will travel to after spawning, and before starting the PickUp behavior.</param>
        public DropArgs(Vector3 spawnPosition, Vector3 dropPosition)
        {
            SpawnPosition = spawnPosition;
            DropPosition = dropPosition;
        }
    }

    /// <summary>
    /// Defines the Drop behavior for the Enemy.
    /// </summary>
    public interface IDrop
    {
        /// <summary>
        /// The radius that the drops will be dropped inside.
        /// </summary>
        float DropRadius { get; set; }
        /// <summary>
        /// The item that wil be dropped when Drop is called.
        /// </summary>
        Drop[] Drops { get; }
        /// <summary>
        /// This event should rise whenever the Enemy Drops a PickUp Item. 
        /// </summary>
        event ShmupDelegate OnDrop;
        /// <summary>
        /// Instantiates and Drops all the items in the DropRadius.
        /// </summary>
        void Drop();

    }

    #endregion

    /// <summary>
    /// Defines the Enemy agent. 
    /// </summary>
    public interface IEnemy : IDrop, IDoCollision, IPool, IImmunityByRegion
    {

    }

}