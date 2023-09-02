using UnityEngine;

namespace ShmupBaby {

    /// <summary>
    /// Controls the scale of a UI element that represents a bar to demonstrate an agent property.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Agent/Component/Bar Control")]
    public sealed class BarControl : MonoBehaviour {

        /// <summary>
        /// Defines Options for agent properties that can be demonstrated on a bar.
        /// </summary>
        public enum BarType
        {
            HealthBar,
            ShieldBar
        }

        /// <summary>
        /// The target agent that has a desired property to represent.
        /// </summary>
        [Tooltip("The target agent")]
        public Agent Target;
        /// <summary>
        /// The desired property to represent.
        /// </summary>
        [Tooltip("The desired property to represent")]
        public BarType Type;
        /// <summary>
        /// The UI element that represents the bar.
        /// </summary>
        [Tooltip("Reference to the bar")]
        public RectTransform Bar;

        //The max bar width, this value is assigned on start.
        private float _barMaxValue;
        //the max x offset of the rect transform
        private float _startMaxX;
        //Checks if the agent is a player.
        private Player _player {
            get { return Target as Player; }
        }

        /// <summary>
        /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
		private void Start() {

            CashBarWidth();

            Initialize();

        }

        /// <summary>
        /// cash the width of the bar.
        /// </summary>
        public void CashBarWidth()
        {
            if (Bar == null)
                return;

            //Sets the bar maximum value to its width in the start.
            _barMaxValue = Bar.rect.width;
            _startMaxX = Bar.offsetMax.x;
        }

        /// <summary>
        /// Resets the bar and create a connection between the bar and the target agent.
        /// </summary>
        public void Initialize()
        {
            if (Target == null)
                return;

            SubscribeToAgent();

            SetBarToInitialValue();
        }

        /// <summary>
        /// Sets the bar to an agent property on the start
        /// </summary>
	    public void SetBarToInitialValue()
	    {
	        if (Type == BarType.HealthBar && Target.MaxHealth > 0)
	            UpdateBar(Target.CurrentHealth,Target.MaxHealth);

	        if (Type == BarType.ShieldBar && Target.MaxShield > 0)
	            UpdateBar(Target.CurrentShield, Target.MaxShield);
        }

        /// <summary>
        /// Subscribes to an agent event that is related to the chosen property. 
        /// </summary>
	    private void SubscribeToAgent()
	    {
            //If the target is a player we subscribe to the OnPick event as well.
	        switch (Type)
	        {
	            case BarType.HealthBar:
	            {
	                Target.OnTakeHealthDamage += UpdateBarOnDamage;
	                if (_player != null)
	                    _player.OnPick += CheckForHealPickUp;
	                break;
	            }
	            case BarType.ShieldBar:
	            {
	                Target.OnTakeShieldDamage += UpdateBarOnDamage;
	                if (_player != null)
	                    _player.OnPick += CheckForShieldPickUp;
	                break;
	            }
	        }
	    }

        /// <summary>
        /// Updates the bar when the health changes due to pickup, used to subscribe to the agent OnPick event.
        /// </summary>
	    private void CheckForHealPickUp(ShmupEventArgs args)
	    {
	        PickArgs pickUp = args as PickArgs;

	        if (pickUp.PickUpType == PickUpType.Heal || pickUp.PickUpType == PickUpType.HealthUpgrade)
	            UpdateBar(Target.CurrentHealth, Target.MaxHealth);

	    }

        /// <summary>
        /// Updates the bar when the shield changes due to a pickup upgrade, used to subscribe to the agent OnPick event.
        /// </summary>
        private void CheckForShieldPickUp(ShmupEventArgs args)
	    {
	        PickArgs pickUp = args as PickArgs;

	        if (pickUp.PickUpType == PickUpType.Shield || pickUp.PickUpType == PickUpType.ShieldUpgrade)
	            UpdateBar(Target.CurrentShield, Target.MaxShield);

	    }

        /// <summary>
        /// Updates the bar when the agent takes damage, used to subscribe to the agent OnTakeDamage event.
        /// </summary>
        private void UpdateBarOnDamage ( ShmupEventArgs args ) {

           TakeDamageArgs TakeDamageData = (TakeDamageArgs)args;

			if (TakeDamageData == null)
				return;
            
            UpdateBar(TakeDamageData.CurrentBar, TakeDamageData.MaxBar);
            
		}

        /// <summary>
        /// Scales the bar.
        /// </summary>
        /// <param name="maxValue">Maximum value for the bar.</param>
	    private void UpdateBar(float currentValue, float maxValue)
	    {
	        float barPercentage = currentValue / maxValue;
	        SetRectTransformWidth(Bar, (1-barPercentage) * _barMaxValue);
        }

        /// <summary>
        /// Scales a rectangular transform to a given width.
        /// </summary>
        /// <param name="trans">The target rectangle.</param>
        /// <param name="width">The new width for the rectangle.</param>
		private void SetRectTransformWidth ( RectTransform trans , float width ) {

			Vector2 transOffsetMax = trans.offsetMax;

			transOffsetMax.x = _startMaxX - width;

			trans.offsetMax = transOffsetMax;

		}
	}

}