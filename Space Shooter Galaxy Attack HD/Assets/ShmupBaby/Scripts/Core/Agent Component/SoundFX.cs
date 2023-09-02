using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Defines an array of SfXs to be played on agent events .
    /// </summary>
	[RequireComponent(typeof(Agent))]
    [AddComponentMenu("Shmup Baby/Agent/Component/Sound FX")]
    public sealed class SoundFX : MonoBehaviour {

        /// <summary>
        /// The agent that will trigger the SFX.
        /// </summary>
        private Agent _targetAgent;

        /// <summary>
        /// The SFX that will be played when the agent triggers their events.
        /// </summary>
        [Space]
        [Tooltip("List of SFXs that will be played when their event is triggered")]
        public AgentClip[] Sounds = new AgentClip[] {
			new AgentClip ( AllAgentEvents.Destroy ) ,
		    new AgentClip ( AllAgentEvents.TakeHealthDamage ) ,
		    new AgentClip ( AllAgentEvents.TakeShieldDamage ) 
        };

        /// <summary>
        /// One of Unity's messages that act the same way as start but gets called before start.
        /// </summary>
        private void Awake ()
		{
            _targetAgent = GetComponent<Agent>();

            //Subscribe all the SFX to the agent, prints a message if the subscription failed.
            for (int i = 0; i < Sounds.Length; i++)
		    {
                //Passing the agent type to AgentSoundClip objects, this is because agents may have default sound for it.
		        Sounds[i].SetAgentType(GetAgentType());

		        if ( !_targetAgent.Subscribe(Sounds[i].PlayAgentClip, Sounds[i].Event) )
                    Debug.Log("SFX fail to subscribe to the agent event :"+ Sounds[i].Event.ToString());

		    }

        }

        /// <summary>
        /// Return the type of Target Agent.
        /// </summary>
        /// <returns>Type of Target Agent</returns>
        System.Type GetAgentType()
        {
            System.Type agentType = null;

            if (_targetAgent is Player)
                agentType = typeof(Player);

            if (_targetAgent is Enemy)
                agentType = typeof(Enemy);

            return agentType;
        }

    }

}