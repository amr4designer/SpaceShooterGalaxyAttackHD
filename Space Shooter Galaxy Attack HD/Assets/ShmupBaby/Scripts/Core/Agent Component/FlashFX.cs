using System.Collections;
using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Defines actions of a flash effect that takes place on the renderer for a given agent event.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Component/Flash FX")]
    public sealed class FlashFX : MonoBehaviour {

        /// <summary>
        /// Defines the flash action.
        /// </summary>
        [System.Serializable]
        public class FlashObject
        {

            #if UNITY_EDITOR

            /// <summary>
            /// The flash name is only available in the editor mode.
            /// </summary>
            [Tooltip("The flash name isn't necessary, it's only used for organizing inside the editor")]
            public string FlashName;

            #endif

            public AllAgentEvents Event;
            /// <summary>
            /// The flash color, this color will overlay the material then fade out when the flash is completed.
            /// </summary>
            [Tooltip("The flash color")]
            public Color color;
            /// <summary>
            /// Time in seconds of the flash duration.
            /// </summary>
            [Tooltip("Time in seconds of the flash duration")]
            public float FlashTime;

            /// <summary>
            /// Reference to the component that contains this Object.
            /// </summary>
            public FlashFX FlashFXInstance { get; set; }
            /// <summary>
            /// Reference to the renderer that is targeted by the flash.
            /// </summary>
            public Renderer TargetRender { get; set; }

            #if UNITY_EDITOR

            public FlashObject(string flashName, AllAgentEvents agentEvent, Color flashColor, float flashFlashTime)
            {

                FlashName = flashName;
                Event = agentEvent;
                color = flashColor;
                FlashTime = flashFlashTime;

            }

            #endif

            /// <summary>
            /// Initializes the FlashObject. 
            /// </summary>
            /// <param name="instance">The component that's contained in this Object.</param>
            /// <param name="target">The renderer that is targeted by the flash.</param>
            /// <param name="targetAgent">The Agent with the event.</param>
            public void Initialize(FlashFX instance, Renderer target, Agent targetAgent)
            {

                TargetRender = target;

                FlashFXInstance = instance;

                if (!targetAgent.Subscribe(StartFlash, Event))
                    Debug.Log("Subscribe failed");

            }

            /// <summary>
            /// Starts the flash, it gets called by an agent event.
            /// </summary>
            private void StartFlash(ShmupEventArgs args)
            {
                if (FlashFXInstance!=null)
                //Calling the Coroutine using the component that's contained in this Object.
                FlashFXInstance.StartCoroutine(Flash());
            }

            /// <summary>
            /// The Coroutine that is started by StartFlash().
            /// </summary>
            IEnumerator Flash()
            {

                if (TargetRender != null)
                {
                    float currentTime = Time.time;

                    float flashStartTime = currentTime;

                    float flashEndTime = flashStartTime + FlashTime;

                    //This Coroutine will run every frame until flash time ends.
                    while (flashEndTime >= currentTime)
                    {
                        //Every frame, a color will be sampled based on the remaining time.
                        SetRendererColor(SampleFlashColor((currentTime - flashStartTime) / FlashTime));

                        yield return null;

                        currentTime = Time.time;

                    }

                    SetRendererColor(Color.white);

                }
                else

                    yield return null;

            }

            /// <summary>
            /// Sets an overlay color to the renderer (for both Mesh and Sprite renderer)
            /// </summary>
            /// <param name="color">Overlay color for the material</param>
            private void SetRendererColor(Color color)
            {
                SpriteRenderer myRenderer = TargetRender as SpriteRenderer;

                if (myRenderer != null)
                    myRenderer.color = color;
                else
                    TargetRender.material.SetColor("_Color",color);
            }

            /// <summary>
            /// Sample color between flash color and white color.
            /// </summary>
            /// <param name="percentage">A number between 0 and 1 where 0 is white.</param>
            /// <returns>The sample color</returns>
            private Color SampleFlashColor(float percentage)
            {
                //linearly interpolates between the user defined color and white color in a given time.
                return Color.Lerp(new Color(1, 1, 1, 1), color, percentage);
            }

        }

        /// <summary>
        /// The agent that will trigger the flash.
        /// </summary>
        [Space]
        [Tooltip("Connect here the agent item. (This is added so you " +
                 "can add the FlashFx Controller as a component of a " +
                 "child object and not necessarily the agent root game object)")]
        public Agent TargetAgent;

        /// <summary>
        /// The renderer that will receive the flash.
        /// </summary>
        [Tooltip("The renderer that will receive the flash, this could be a mesh or a sprite renderer")]
		public Renderer TargetRenderer ;

        //Creating a default value for the editor
        #if UNITY_EDITOR

        /// <summary>
        /// The flash that will take place when the agent triggers their events.
        /// </summary>
        [Space]
        [Tooltip("List of flashes that will take place when their event is triggered")]
        public FlashObject[] FlashsFX  = new FlashObject[] {
			new FlashObject ( "Health Damage Flash" , AllAgentEvents.TakeHealthDamage , Color.red , 0.5f ) ,
			new FlashObject ( "Shield Damage Flash" , AllAgentEvents.TakeShieldDamage , Color.cyan , 0.5f )  };

        #else

		public FlashObject[] FlashsFX;

        #endif


        /// <summary>
        /// One of Unity's messages that act the same way as start but gets called before start.
        /// </summary>
        private void Awake()
		{

			if (TargetRenderer == null || TargetAgent == null)
				return;

            //Calling Initialize for all Flash Objects, this will handle the subscription to the agent.
            for (int i = 0; i < FlashsFX.Length; i++) {
				FlashsFX [i].Initialize (this, TargetRenderer, TargetAgent);
			}

		}

	}

}