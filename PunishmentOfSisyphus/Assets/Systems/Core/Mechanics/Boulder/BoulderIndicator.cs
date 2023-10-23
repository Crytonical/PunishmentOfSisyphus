using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderIndicator : MonoBehaviour
{
    [SerializeField] GameObject boulder;
    [SerializeField] GameObject levelBounds;
    [SerializeField] private const float DEFAULT_SCALE = 1.838f;
    [SerializeField] private const float scalingFactor = 0.14f;
    [SerializeField] private const float MIN_SCALE = 0.5f;

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
            transform.position = new Vector2(boulder.transform.position.x, bounds.yMax - 1.0f);

            float distance = boulder.transform.position.y - bounds.yMax;
            float newScale = DEFAULT_SCALE - scalingFactor * distance;
            newScale = Mathf.Max(newScale, MIN_SCALE);
            transform.localScale = new Vector3(newScale, newScale, 1.0f);
        }
        else
        {
            transform.position = new Vector2(500, 500);
        }
    }
}
