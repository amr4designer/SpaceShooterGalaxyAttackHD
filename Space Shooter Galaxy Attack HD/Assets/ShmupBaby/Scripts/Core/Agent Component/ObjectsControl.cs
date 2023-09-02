using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Defines one of the general actions for a GameObject that includes (Instantiate,Destroy and Parenting) 
    /// </summary>
    [System.Serializable]
	public class ObjectSettings
    {
        /// <summary>
        /// General GameObject actions.
        /// </summary>
		public enum ControlOption
        {
            Enable,
            Instantiate,
            DestroyImmediate,
            Disable
        }
        
        /// <summary>
        /// Options for GameObject Parenting.
        /// </summary>
        public enum ParentOption
        {
            None,
            PointConstrain,
            Parent,
            Unparent
        }

        /// <summary>
        /// The controlled GameObject.
        /// </summary>
        [Space]
        [Tooltip("The object that is going to be controlled")]
        public GameObject TargetObject;

        /// <summary>
        /// General actions that will take place when Start is called.
        /// </summary>
		[Space]
        [Tooltip("The action that will take place when the event is triggered")]
        public ControlOption OnEventAction;
        /// <summary>
        /// Parenting the action that will take place when Start is called.
        /// </summary>
        public ParentOption ParentingOption;
        /// <summary>
        /// Counter of the object to get destroyed from when Start gets called.
        /// </summary>
        [Space]
        [Tooltip("Destroys the control object after time, set it to zero if you do not wish to destroy it after time")]
        public float DestroyAfterTime ;

        /// <summary>
        /// Reference to the component that contains this Object.
        /// </summary>
        private MonoBehaviour _controller;

        public void Initialize( MonoBehaviour cont )
        {
            _controller = cont;
        }

        /// <summary>
        /// Starts the action on the TargetObject.
        /// </summary>
        public void Start ( ShmupEventArgs args )
        {

            if (TargetObject == null)
                return;

            GameObject currentObject = TargetObject;
            //Applies the desired general action
            switch (OnEventAction)
            {
                case ControlOption.Enable:
                    TargetObject.SetActive(true);
                    break;
                case ControlOption.Instantiate:
                    currentObject = GameObject.Instantiate(TargetObject);
                    currentObject.SetActive(true);
                    currentObject.transform.position = _controller.transform.position;
                    break;
                case ControlOption.DestroyImmediate:
                    GameObject.Destroy(TargetObject);
                    return;
                case ControlOption.Disable:
                    TargetObject.SetActive(false);
                    break;
            }

            //As you can see the currentObject will always be assigned to the TargetObject,
            //except in the case of Instantiate where it will be assigned to the new instance

            //Apply the parenting action.
            switch (ParentingOption)
            {
                case ParentOption.Unparent:
                    currentObject.transform.parent = null;
                    currentObject.transform.rotation = Quaternion.identity;
                    break;
                case ParentOption.Parent:
                    currentObject.transform.parent = _controller.transform;
                    break;
                case ParentOption.PointConstrain:
                    PointConstrain pointConstrain = currentObject.AddComponent<PointConstrain>();
                    pointConstrain.Target = _controller.transform;
                    break;
            }

            //Apply the destroy after time action.
            if (DestroyAfterTime != 0)
                Object.Destroy(currentObject, DestroyAfterTime);
               
         }
        
	}

    /// <summary>
    /// Defines actions for GameObjects based on agent events.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Component/Objects Control")]
    public sealed class ObjectsControl : MonoBehaviour
	{
        /// <summary>
        /// Defines the GameObject Action for ObjectsControl.
        /// </summary>
		[System.Serializable]
		public class AgentObjectControlSettings {

            /// <summary>
            /// An agent event that will trigger the action.
            /// </summary>
            [Tooltip("An agent event that will trigger the action defined in the Settings")]
            [Space]
			public AllAgentEvents Event;
            /// <summary>
            /// The action that will take place when the event is a trigger.
            /// </summary>
            [Tooltip("The action that will take place when the event is a trigger")]
            [Space]
			public ObjectSettings Settings;

            public AgentObjectControlSettings(ObjectSettings settings)
            {
                Settings = settings;
            }

            /// <summary>
            /// Subscribes the action to the agent.
            /// </summary>
            /// <param name="agent">The agent that will trigger the action</param>
            /// <returns>Returns true if the subscription is successful and false if it's not.</returns>
            public bool SubscribeFX ( Agent agent ) {

				return agent.Subscribe (Settings.Start, Event);

			}

		}


	    /// <summary>
	    /// The agent that will trigger the flash effect.
	    /// </summary>
	    [Space]
		[HelpBox("Please use Enable if your target object is a game object (nested in the heirarchy) and use Instantiate if your target object is a prefab. If you have an Objects Control script Destroy event; make sure you select Unparent.")]
	    [Tooltip("Connect here the agent item. (This is added so you " +
                 "can add the ObjectsControl as a component of a " +
	             "child object and not necessarily the agent root Game Object)")]
        public Agent agent;
	    
	    /// <summary>
	    /// The actions that will take place when the agent triggers their events.
	    /// </summary>
	    [Space]
	    [Tooltip("List of actions that will take place when their event is a trigger")]
        public AgentObjectControlSettings[] ObjectControlSettings;

	    /// <summary>
	    /// One of Unity's messages that act the same way as start but gets called before start.
	    /// </summary>
        private void Awake ()
		{

		    if (agent == null)
		        agent = GetComponent<Agent>();

		    if (agent == null)
		    {
                Debug.Log("you don't have agent");
		        return;
		    }

            //Calling Initialize for all ObjectControlSettings then subscribes them to the agent.

            for (int i = 0; i < ObjectControlSettings.Length; i++) {

                ObjectControlSettings[i].Settings.Initialize(this);

				if (!ObjectControlSettings[i].SubscribeFX( agent ) )
					Debug.Log ("Subscribe failed");

			}

		}

	}
}