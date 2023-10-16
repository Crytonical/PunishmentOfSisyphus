using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderIndicator : MonoBehaviour
{
    [SerializeField] GameObject boulder;
    [SerializeField] GameObject levelBounds;

    Rect bounds;

    private void Start()
    {
        bounds = levelBounds.GetComponent<RectTransform>().rect;
    }

    // Update is called once per frame
    void Update()
    {
        if(boulder.transform.position.y > bounds.yMax)
        {
            transform.position = new Vector2(boulder.transform.position.x, bounds.yMax - 1.2f);

            float distance = boulder.transform.position.y - bounds.yMax;
            float scalingFactor = 0.04f;
            float newScale = 1.0f - scalingFactor * distance;
            newScale = Mathf.Max(newScale, 0.1f);
            transform.localScale = new Vector3(newScale, newScale, 1.0f);
        }
        else
        {
            transform.position = new Vector2(500, 500);
        }
    }
}
