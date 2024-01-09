using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public bool Paused = false;
    public GameObject pauseMenu;
    public GameObject gameOver;
    public ScoreManager scoreManager;
    public Stopwatch stopwatch;

    void Awake()
    {
        if (instance == null) instance = this;
    }
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
                Pause();
			}
		}
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        GameManager.instance.StopTime();
        Paused = true; 
        Cursor.lockState = CursorLockMode.None;
    }

    public void Play()
    {
        pauseMenu.SetActive(false);
        GameManager.instance.PlayTime();
        Paused = false; 
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MainMenuButton()
    {
        scoreManager.AddScore(new Score(stopwatch.currentTime));
        scoreManager.SaveScore();
        // Debug.Log(PlayerPrefs.GetString("Scores", "{}"));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}

    public void GameOver()
    {
        gameOver.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        GameManager.instance.StopTime();
	}

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
        gameOver.SetActive(false);
	}
}
