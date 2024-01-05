using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool Paused = false;
    public GameObject PauseMenuCanvas;
    public GameObject gameOverCanvas;
    public ScoreManager scoreManager;
    public Stopwatch stopwatch;

   // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { 
            if (Paused)
            {
                Play();
			}
            else
            {
                Stop();
			}
		}
    }

    void Stop()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        Paused = true; 
        Cursor.lockState = CursorLockMode.None;
    }

    public void Play()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Paused = false; 
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MainMenuButton()
    {
        scoreManager.AddScore(new Score(stopwatch.currentTime));
        scoreManager.SaveScore();
        Debug.Log(PlayerPrefs.GetString("Scores", "{}"));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}

    public void GameOver()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
	}

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
        gameOverCanvas.SetActive(false);
	}
}
