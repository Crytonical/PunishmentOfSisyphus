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
        enemySpawnEvent.levelEnd.AddListener(ToggleArrow);
    }
    private void OnDisable()
    {
        enemySpawnEvent.levelEnd.RemoveListener(ToggleArrow);
    }

    private void Awake()
    {
        toggled = false;
        movingUpwards = false;
        rectT = GetComponent<RectTransform>();
        rectT.localPosition = new Vector3(0.0f, 700f, 0.0f);
    }

    private void Update()
    {
        if (toggled)
        {
            moveSpeed = Mathf.Abs(moveSpeed);

            if (rectT.position.y >= 370)
            {
                movingUpwards = false;
            }
            if (rectT.position.y <= 300)
            {
                movingUpwards = true;
            }

            if (!movingUpwards)
            {
                moveSpeed = -moveSpeed;
            }
            rectT.localPosition = new Vector3(rectT.position.x, rectT.position.y + moveSpeed, rectT.position.z);
        }
    }

    private void ToggleArrow()
    {
        Debug.Log("Yumps");
        toggled = !toggled;

        if (toggled)
        {
            rectT.localPosition = new Vector3(0.0f, 370f, 0.0f);
        }
        else
        {
            rectT.localPosition = new Vector3(0.0f, 700f, 0f);
        }
    }
}
