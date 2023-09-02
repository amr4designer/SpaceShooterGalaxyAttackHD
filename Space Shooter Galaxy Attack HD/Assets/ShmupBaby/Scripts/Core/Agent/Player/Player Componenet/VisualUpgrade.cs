using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Changes the visual of the player when it gets a weapon upgrade.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Player/Component/Visual Upgrade")]
    public sealed class VisualUpgrade : MonoBehaviour , IUpgradeable
    {
        /// <summary>
        /// The Player agent that will trigger the Upgrade.
        /// </summary>
        [Space]
        [Tooltip("Connect here the player item. (This is added so you " +
                 "can add the visual upgrade as a component of a " +
                 "child object and not necessarily the Player parent game object)")]
        public Agent Target;

        /// <summary>
        /// Reference to the GameObjects that represent the upgrade stages.
        /// </summary>
        [Space]
        [Tooltip("List of the GameObjects that represent the upgrade stages in order")]
        public GameObject[] UpgradeStages;

        /// <summary>
        /// This event will be a trigger when the visual upgrade occurs.
        /// </summary>
        public event ShmupDelegate OnStageChanged;

        /// <summary>
        /// The index of the current stage.
        /// </summary>
        public int StageIndex
        {
            get { return _stageIndex; }
            set
            {
                //Checks to see if the value that we set has changed from the last time it was set.
                if (value != _stageIndex)
                {
                    //if the value change then the visual will be changed.
                    SetToStage(value);
                }

                _stageIndex = value;
            }
        }

        //The backend field for StageIndex.
        private int _stageIndex;

        /// <summary>
        /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
        private void Start()
        {
            //Sets the visual to the first stage.
            SetToStage(0);

            //Subscribes to the agent weapons upgrade and downgrade events.
            Target.Subscribe(Upgrade, AllAgentEvents.WeaponUpgrade);
            Target.Subscribe(Downgrade, AllAgentEvents.WeaponDowngrade);
        }

        /// <summary>
        /// This method is only used to subscribe to the agent event.
        /// </summary>
        private void Upgrade(ShmupEventArgs args)
        {
            Upgrade();
        }
        /// <summary>
        /// This method is only used to subscribe to the agent event.
        /// </summary>
        void Downgrade(ShmupEventArgs args)
        {
            Downgrade();
        }

        /// <summary>
        /// Upgrades the ship visual to the next stage.
        /// </summary>
        public void Upgrade()
        {
            StageIndex++;
        }
        /// <summary>
        /// Returns the ship visual to the previous stage. 
        /// </summary>
        public void Downgrade()
        {
            StageIndex--;
        }

        /// <summary>
        /// Handles the rise of the OnStageChanged event.
        /// </summary>
        private void RiseStageChanged()
        {
            if (OnStageChanged != null)
                OnStageChanged(null);
        }

        /// <summary>
        /// Changes the ship's visual.
        /// </summary>
        /// <param name="index">The index for the visual</param>
        void SetToStage(int index)
        {
            if (index < 0)
                index = 0;

            if (index >= UpgradeStages.Length)
                index = UpgradeStages.Length - 1;


            //Raise the OnStageChanged event.
            RiseStageChanged();

            //Disables all the visuals except the one with the given index.
            for (int i = 0; i < UpgradeStages.Length; i++)
            {
                UpgradeStages[i].SetActive(false);
            }

            UpgradeStages[index].SetActive(true);
        }

    }

}