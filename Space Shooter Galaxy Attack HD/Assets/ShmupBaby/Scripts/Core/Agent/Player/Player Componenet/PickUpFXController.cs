using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Defines actions for given objects that take place on agent pickup events.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Player/Component/Pick Up FX Controller")]
    public sealed class PickUpFXController : MonoBehaviour {

        /// <summary>
        /// Defines an action for a given object to take place on an agent pickup event.
        /// </summary>
		[System.Serializable]
		public sealed class PickUpFX {

            /// <summary>
            /// The type of pickUp event.
            /// </summary>
			[Space]
            public PickUpType Event;
            /// <summary>
            /// The action that will take place when the event is a trigger.
            /// </summary>
			[Space]
			public ObjectSettings Settings;

            /// <summary>
            /// Subscribes the action (Settings) to the given pickUp event (Event).
            /// </summary>
            /// <param name="agent">The agent that you are going to subscribe to</param>
            /// <returns>Returns true if the Subscription is successful and false if it failed</returns>
			public bool SubscribeFX ( Agent agent ) {

				Settings.Initialize (agent);
				return agent.Subscribe (StartAction,AllAgentEvents.PickUp);

			}

            /// <summary>
            /// This method gets called when an agent OnPick event is triggered, then it's checked if the event
            /// is triggered by the given pickup event so the action could take place.
            /// </summary>
            private void StartAction ( ShmupEventArgs args ) {

                //Unboxing the pickup argument
				PickArgs Args = args as PickArgs;

				if (Args == null)
					return;

                //If the pickup argument have a pickup type that matches the 
				//Event; the Action will take a place.
                if (Event == Args.PickUpType)
					Settings.Start (null);

			}

		}

        /// <summary>
        /// The Player agent that will trigger the actions.
        /// </summary>
		[Space]
        [Tooltip("Connect here the player item. (This is added so you " +
                 "can add the Pick Up FX Controller as a component of a " +
                 "child object and not necessarily the Player parent game object)")]
		public Agent agent;

        /// <summary>
        /// The actions that will take place when the agent triggers their events.
        /// </summary>
		[Space]
        [Tooltip("List of actions that will take place when their event is triggered")]
        public PickUpFX[] FX;

        /// <summary>
        /// One of Unity's messages that act the same way as start but gets called before start.
        /// </summary>
        void Awake()
        {

            //Subscribes all the actions to the agent, prints a message if the subscription failed.
			for (int i = 0; i < FX.Length; i++) {

				if (!FX[i].SubscribeFX( agent ) )
					Debug.Log ("Subscribe failed");

			}

		}

	}

}