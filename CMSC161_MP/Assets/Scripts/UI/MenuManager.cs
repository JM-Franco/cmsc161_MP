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
    public GameObject nextLevel;
    public ScoreManager scoreManager;
    public AudioClip gameOverBGM;

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
        if (nextLevel.activeSelf) return;
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
        SceneManager.LoadScene(0);
	}

    public void GameOver()
    {
        gameOver.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        GameManager.instance.StopTime();
	}

    public void NextLevel()
    {
        GameObject.Find("ClickSound").GetComponent<AudioSource>().Play();
        nextLevel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        GameManager.instance.StopTime();
    }

    public void NextLevelButton()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void RestartGame()
    {
        ScoreManager.instance.AddScore(new Score(GameManager.instance.currentTime));
        Destroy(GameManager.instance.gameObject);
        Debug.Log(GameManager.instance.keysRequired);
        SceneManager.LoadScene(1);
        Debug.Log(GameManager.instance.keysRequired);
        gameOver.SetActive(false);
	}
}
