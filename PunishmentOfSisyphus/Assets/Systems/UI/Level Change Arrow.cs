using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ephymeral.Events;

public class LevelChangeArrow : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private EnemySpawnerEvent enemySpawnEvent;
    #endregion

    #region FIELDS
    private bool toggled, movingUpwards;
    [SerializeField] float moveSpeed;
    #endregion

    #region PROPERTIES
    private RectTransform rectT;
    #endregion

    private void OnEnable()
    {
        enemySpawnEvent.levelEnd.AddListener(EnableArrow);
        enemySpawnEvent.spawnWave.AddListener(DisableArrow);
    }
    private void OnDisable()
    {
        enemySpawnEvent.levelEnd.RemoveAllListeners();
        enemySpawnEvent.spawnWave.RemoveAllListeners();
    }

    private void Awake()
    {
        toggled = false;
        movingUpwards = false;
        rectT = GetComponent<RectTransform>();
        rectT.position = new Vector3(0.0f, 700f, 0.0f);
    }

    private void Update()
    {
        if (toggled)
        {
            moveSpeed = Mathf.Abs(moveSpeed);

            if (rectT.position.y >= 3)
            {
                movingUpwards = false;
            }
            if (rectT.position.y <= 2)
            {
                movingUpwards = true;
            }

            if (!movingUpwards)
            {
                moveSpeed = -moveSpeed;
            }
            rectT.position = new Vector3(rectT.position.x, rectT.position.y + moveSpeed, rectT.position.z);
        }
    }

    private void DisableArrow()
    {
        Debug.Log("Disable Arrow");
        toggled = false;

        rectT.position = new Vector3(0.0f, 70000f, 0f);
    }

    private void EnableArrow()
    {
        Debug.Log("Enable Arrow");
        toggled = true;
        rectT.position = new Vector3(0.0f, 3f, 0.0f);
    }
}
