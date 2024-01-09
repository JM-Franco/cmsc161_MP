using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    public void PlayTime()
    {
        Time.timeScale = 1f;
    }

    public void ChangeTimeScale(float targetTimeScale, float duration)
    {
        StartCoroutine(GradualTimeScaleChange(targetTimeScale, duration));
    }

    private IEnumerator GradualTimeScaleChange(float targetTimeScale, float duration)
    {
        float currentTimeScale = Time.timeScale;
        float startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime < duration)
        {
            float progress = (Time.realtimeSinceStartup - startTime) / duration;
            Time.timeScale = Mathf.Lerp(currentTimeScale, targetTimeScale, progress);

            yield return null; // Wait for the next frame
        }

        Time.timeScale = targetTimeScale;
    }
}
