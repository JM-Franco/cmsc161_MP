using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public GameObject pauseMenu;
    public GameObject hurtOverlayCanvas;

    public void takeDamage()
    {
        health -= 1;
        hurtOverlayCanvas.SetActive(true);

        
    }

    public void Start()
    {
        GameObject hud = GameObject.Find("HUD");
        foreach (Transform t in hud.GetComponentInChildren<Transform>())
        {
            if (t.name == "HurtOverlayCanvas")
            {
                hurtOverlayCanvas = t.gameObject;
            }
        }
    }

    public void Update()
    {
        CanvasGroup canvasGroup = hurtOverlayCanvas.GetComponent<CanvasGroup>();
        if (hurtOverlayCanvas.activeSelf == true) canvasGroup.alpha -= Time.deltaTime / 2f;

        if (canvasGroup.alpha <= 0f)
        {
            hurtOverlayCanvas.SetActive(false);
            canvasGroup.alpha = 1f;
        }

        if (health == 0)
        {
            pauseMenu.GetComponent<PauseMenu>().GameOver();
        }
    }
}
