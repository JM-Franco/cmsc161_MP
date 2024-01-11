using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public GameObject entity;
    public int health;
    public GameObject hurtOverlay;
    public AudioClip[] hurtSounds; 
    public AudioClip deathSound;
    public bool isAlive = true;

    public void takeDamage()
    {
        health -= 1;
        SoundFXManager.instance.PlayRandomSoundFXClip(hurtSounds, transform, 1f);
        ShowHurtOverlay();
    }

    public void Start()
    {
        entity = gameObject;

        GameObject ui = GameObject.Find("UI");
        foreach (Transform t in ui.GetComponentInChildren<Transform>())
        {
            if (t.name == "HurtOverlay")
            {
                hurtOverlay = t.gameObject;
            }
        }
    }

    public void Update()
    {
        Image image = hurtOverlay.GetComponent<Image>();
        if (image.color.a == 0f)
        {
            hurtOverlay.SetActive(false);
        }
        Color newColor = image.color;
        newColor.a = Mathf.Clamp01(newColor.a - (Time.deltaTime / 2f));
        image.color = newColor;

        //if (hurtOverlay.activeSelf == true) canvasGroup.alpha -= Time.deltaTime / 2f;

        // if (canvasGroup.alpha <= 0f)
        // {
        //     hurtOverlay.SetActive(false);
        //     canvasGroup.alpha = 1f;
        // }

        if (health == 0)
        {
            if (isAlive) // If player has just died
            {
                Debug.Log("Playing GameOverBGM");
                GameObject.Find("GameOverBGM").GetComponent<AudioSource>().Play();
                SoundFXManager.instance.PlaySoundFXClip(deathSound, transform, 1f);
                ScoreManager.instance.AddScore(new Score(GameManager.instance.currentTime));
                ScoreManager.instance.SaveScore();
                isAlive = false;
            }
            MenuManager.instance.GameOver();
        }
    }

    private void ShowHurtOverlay()
    {
        Image image = hurtOverlay.GetComponent<Image>();
        if (hurtOverlay.activeSelf == false) hurtOverlay.SetActive(true);
        image.color = new Color(255,255,255,1);
    }
}
