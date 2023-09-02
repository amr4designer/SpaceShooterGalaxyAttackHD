using UnityEngine;
using UnityEngine.UI;

namespace ShmupBaby
{
    /// <summary>
    /// The level select button.
    /// </summary>
    [AddComponentMenu("Shmup Baby/Main Menu/Level Select Button")]
    public class LevelSelectButton : MonoBehaviour
    {
        /// <summary>
        /// index of the level that will be loaded by the button.
        /// </summary>
        public int LevelIndex;

        /// <summary>
        /// The Start method is one of Unity's messages that gets called when a new object is instantiated.
        /// </summary>
        private void Start()
        {
            Button myButton = GetComponent<Button>();
            myButton.onClick.AddListener(GoToLevel);
        }

        /// <summary>
        /// goes to the level with LevelIndex if it's available. 
        /// </summary>
        public void GoToLevel()
        {
            
            GameManager.Instance.ToLevel( LevelIndex + GameManager.Instance.MinLevelSceneIndex );

        }

    }

}