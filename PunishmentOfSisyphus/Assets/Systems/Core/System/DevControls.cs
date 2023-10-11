using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Ephymeral.EntityNS;
using Ephymeral.BoulderNS;
using Ephymeral.Events;
using Ephymeral.Data;
using Ephymeral.EnemyNS;

public class DevControls : MonoBehaviour
{
    // Static instance variable to hold the singleton instance
    private static DevControls _instance;

    // Public property to access the singleton instance
    public static DevControls Instance
    {
        get
        {
            // If the instance is null, find or create the DevControls object
            if (_instance == null)
            {
                _instance = FindObjectOfType<DevControls>();

                // If there is no DevControls object in the scene, create one
                if (_instance == null)
                {
                    GameObject obj = new GameObject("DevControls");
                    _instance = obj.AddComponent<DevControls>();
                }
            }

            return _instance;
        }
    }

    #region References

    [SerializeField] private BoulderEvent boulderEvent;
    [SerializeField] private PlayerEvent playerEvent;
    [SerializeField] private PlayerMovementData playerMovementData;
    [SerializeField] private SceneEvent sceneEvent;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Return Boulder
        if (Input.GetKeyDown("b"))
        {
            boulderEvent.PickUpBoulder();
        }

        //Reload test scene
        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene("Test");
        }
    }
}
