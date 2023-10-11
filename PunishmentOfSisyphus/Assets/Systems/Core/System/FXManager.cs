using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    // Basic singleton framework generated from Chat GPT rather than written manually
    // Static instance variable to hold the singleton instance
    private static FXManager _instance;

    // Public property to access the singleton instance
    public static FXManager Instance
    {
        get
        {
            // If the instance is null, find or create the FXManager object
            if (_instance == null)
            {
                _instance = FindObjectOfType<FXManager>();

                // If there is no FXManager object in the scene, create one
                if (_instance == null)
                {
                    GameObject obj = new GameObject("FXManager");
                    _instance = obj.AddComponent<FXManager>();
                }
            }

            return _instance;
        }
    }

    // Hit FX
    #region Hit FX
    public void CreateHitFX(int pauseFrames, int shakeFrames, int shakeIntensity)
    {
        StartCoroutine(HitFXCo(pauseFrames, shakeFrames, shakeIntensity));
    }

    private IEnumerator HitFXCo(int pauseFrames, int shakeFrames, int shakeIntensity)
    {
        // Freeze Game
        Time.timeScale = 0f;

        // Wait for duration
        for (int i = 0; i < pauseFrames; i++)
        {
            yield return null;
        }

        // Resume game
        Time.timeScale = 1f;

        //Shake Screen

    }

    #endregion

    //Screen Freeze
    #region Screen Freeze
    public void ScreenFreeze(int frames)
    {
        StartCoroutine(ScreenFreezeCo(frames));
    }

    private IEnumerator ScreenFreezeCo(int numberOfFrames)
    {
        // Freeze Game
        Time.timeScale = 0f;

        // Wait for duration
        for (int i = 0; i < numberOfFrames; i++)
        {
            yield return null;
        }

        // Resume game
        Time.timeScale = 1f;

        //Shake Screen
        ShakeScreen(0.001f, 2);
    }

    #endregion

    #region Screen Shake
    public void ShakeScreen(float shakeIntensity, int duration)
    {
        StartCoroutine(ShakeScreenCo(shakeIntensity, duration));
    }

    private IEnumerator ShakeScreenCo(float shakeIntensity, int duration)
    {
        Vector3 originalPosition = Camera.main.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = originalPosition.x + Random.Range(-1f, 1f); //* shakeIntensity;
            float y = originalPosition.y + Random.Range(-1f, 1f); //* shakeIntensity;
            float z = originalPosition.z;

            Camera.main.transform.position = new Vector3(x, y, z);

            elapsed += 1;

            yield return null;
        }

        Debug.Log("Screen Shake Over");

        // Reset the camera position
        Camera.main.transform.position = originalPosition;
    }

    #endregion

    // Ensure the instance is destroyed when the game is closed
    private void OnApplicationQuit()
    {
        _instance = null;
    }
}
