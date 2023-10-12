using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Ephymeral.Events
{
    [CreateAssetMenu(fileName = "SceneEvent", menuName = "EventObjects/SceneEvent")]
    public class SceneEvent : ScriptableObject
    {
        #region FIELDS

        #region EVENTS
        [System.NonSerialized]
        public UnityEvent<string> gameOverEvent;
        #endregion
        #endregion

        #region PROPERTIES
        #endregion

        private void OnEnable()
        {
            #region CREATE EVENTS
            if (gameOverEvent == null)
            {
                gameOverEvent = new UnityEvent<string>();
            }
            #endregion
        }

        public void GameOver(string scene)
        {
            gameOverEvent.Invoke(scene);
        }
    }
}
