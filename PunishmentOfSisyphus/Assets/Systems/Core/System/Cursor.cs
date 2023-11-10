using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] GameObject cursorSprite;
    #endregion

    #region FIELDS
    #endregion

    private void Awake()
    {
        cursorSprite = Instantiate(cursorSprite, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        cursorSprite.transform.position = Input.mousePosition;
    }
}
