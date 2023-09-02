using System.Collections.Generic;

namespace ShmupBaby
{
    //to make monoBehavior use the pool manager all you need to do is :
    // 1- implement the IPool interface.
    // 2- use the extension method AddToPool when the object appears in the scene ( OnEnable() )
    // 3- use the extension method RemoveFromPool when the object disappears from the scene ( OnDisable() )
    // 4- you can use GetList to get all the object in the scene.

    /// <summary>
    /// Defines the general behavior for the object in the pool.  
    /// </summary>
    public interface IPool
    {
        /// <summary>
        /// Should be triggered when the inherited object is added to the pool. 
        /// </summary>
        event ShmupDelegate OnAddToPool;
        /// <summary>
        /// Should be triggered when the inherited object is removed from the pool. 
        /// </summary>
        event ShmupDelegate OnRemoveFromPool;
    }

    /// <summary>
    /// Creates and manages the generic pools. 
    /// </summary>
    public static class PoolManager
    {
        /// <summary>
        /// Dictionary with all pools by their type.
        /// </summary>
        private static readonly Dictionary<System.Type, List<IPool>> _pools;

        static PoolManager()
        {
            _pools = new Dictionary<System.Type, List<IPool>>();
        }

        /// <summary>
        /// Returns a list with all the pool objects if it exists .
        /// </summary>
        /// <typeparam name="T">the type of the objects.</typeparam>
        /// <returns>list with all the pool objects</returns>
        public static List<IPool> GetList<T>()
        {
            if (_pools.ContainsKey(typeof(T)))
                return _pools[typeof(T)];
            else
                return null;
        }

        /// <summary>
		/// adds an object to the pool, if no pool exists; then a new one will be created.
        /// </summary>
        /// <typeparam name="T">the type of pool.</typeparam>
        /// <param name="poolObject">the objects that need to be added.</param>
        public static void AddToPool<T>(this IPool poolObject)
        {
            System.Type poolType = typeof(T);

            if (!_pools.ContainsKey(poolType))
                _pools.Add(poolType, new List<IPool>());

            _pools[poolType].Add(poolObject);
        }

        /// <summary>
        /// remove object from the pool.
        /// </summary>
        /// <typeparam name="T">the type of pool.</typeparam>
        /// <param name="poolObject">the target object that need to be removed</param>
        /// <returns>true if this is the last object in the pool.</returns>
        public static bool RemoveFromPool<T>(this IPool poolObject)
        {
            System.Type poolType = typeof(T);

            if (!_pools.ContainsKey(poolType))
                return true;

            List<IPool> poolObjectList = _pools[poolType];

            if (poolObjectList.Count == 1)
            {
                poolObjectList.Clear();
                return true;
            }

            poolObjectList.Remove(poolObject);
            
            return false;
        }

    }

}