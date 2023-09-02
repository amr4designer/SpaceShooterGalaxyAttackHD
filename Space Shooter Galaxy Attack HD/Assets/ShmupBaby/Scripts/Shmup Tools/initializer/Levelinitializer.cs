using System.Collections.Generic;
using UnityEngine;

namespace ShmupBaby
{
    //LevelInitializer is used to call methods in order for a given frame.
    //work as following :
    // 1- all the components that need to be called in order should
    //    subscribe to the level initializer in their awake method.
    // 2- the given index that's been used to subscribe will define the 
    //    order in which the subscriber methods will be called in the start.

    /// <summary>
    /// Manages the Initialization of scene components in order. 
    /// </summary>
    [AddComponentMenu("")]
    public sealed class LevelInitializer : Singleton<LevelInitializer>
    {

        /// <summary>
        /// defines the stage for the LevelInitializer
        /// </summary>
        private sealed class InitializeStage : System.IComparable<InitializeStage>
        {
            /// <summary>
            /// the lower the index number the sooner it will be called.
            /// </summary>
            public int StageIndex { get; private set; }

            /// <summary>
            /// the event that will contain the initialize methods for this stage.
            /// </summary>
            private event System.Action OnStageCall;

            /// <summary>
            /// InitializeStage constructor.
            /// </summary>
            /// <param name="stageIndex">the index of the stage</param>
            /// <param name="method">the method that will be called when Execute is called.</param>
            public InitializeStage(int stageIndex, System.Action method)
            {
                StageIndex = stageIndex;

                SubscribeToStage(method);
            }

            /// <summary>
            /// Adds an initialize method to this stage. 
            /// </summary>
            /// <param name="method">The method that will get called when the stage is called.</param>
            public void SubscribeToStage(System.Action method)
            {
                if (method != null)
                {
                    OnStageCall += method;
                }
            }

            /// <summary>
            /// Calls the initialize method for this stage.
            /// </summary>
            public void Execute()
            {
                if (OnStageCall != null)
                    OnStageCall();
            }

            public int CompareTo(InitializeStage other)
            {
                if (ReferenceEquals(this, other))
                    return 0;
                if (ReferenceEquals(null, other))
                    return 1;

                return StageIndex.CompareTo(other.StageIndex);
            }
        }

        /// <summary>
        /// lists of all stages for this frame.
        /// </summary>
        private readonly List<InitializeStage> _stages = new List<InitializeStage>();

        /// <summary>
        /// the Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
        void Start()
        {
            //Sort the stages in ascending order.
            _stages.Sort();

            //Executes the stages in order.
            foreach (var stage in _stages)
            {
                stage.Execute();
            }
            
            Destroy(gameObject);
        }

        /// <summary>
        /// subscribes a method to run in the current frame in a given order.
        /// </summary>
        /// <param name="stageIndex">the index of the stage which this method will be called at.</param>
        /// <param name="method">the method which will be called at the given stage.</param>
        /// <returns>true if the stage has no other method to call</returns>
        public bool SubscribeToStage(int stageIndex, System.Action method)
        {
            int index = -1;

            //Checks if there is already a stage with the given index.
            for (int i = 0; i < _stages.Count; i++)
            {
                if (_stages[i].StageIndex == stageIndex)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                //Creates a stage if there isn't any other stage.
                _stages.Add(new InitializeStage(stageIndex,method));
                return true;
            }
            else
            {
                _stages[index].SubscribeToStage(method);
                return false;
            }
        }
        
    }

}