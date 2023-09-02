using UnityEngine;

namespace ShmupBaby{

    /// <summary>
    /// any class which inherits from PersistentSingleton will have only one instance in the scene,
    /// and that instance will live through scenes, other instance will get destroyed on awake,
    /// also if no instance exists an instance will be created.
    /// </summary>
    /// <typeparam name="T">The type of class that inherit PersistentSingleton</typeparam>
    [AddComponentMenu("")]
    public class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{

        /// <summary>
        /// Checks if there is already an instance.
        /// </summary>
        public static bool IsInitialize
        {
            get { return _instance != null; }
        }

        private static T _instance;
        /// <summary>
        /// Returns the instance, if there is no instance in the scene or the Awake of that
        /// instance hasn't been called yet, a new instance will be created.
        /// </summary>
		public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<T>();

                if (_instance != null)
                    return _instance;

                GameObject go = new GameObject();
                _instance = go.AddComponent<T>();
                DontDestroyOnLoad(go);
                return _instance;

            }
        }

        /// <summary>
        /// Any class that inherits from Singleton, needs to overrides Awake if its going to use it
        /// it will make sure that there is only one instance in the scene.
        /// </summary>
		protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
                return;
            }
            else
                _instance = this as T;

            DontDestroyOnLoad(gameObject);
        }
        

    }

}