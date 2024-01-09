using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighScoreContent : MonoBehaviour
{
    public HighScoreEntry highScoreEntry;
    public ScoreManager scoreManager;

    void Start()
    {
        Debug.Log("Setting up High Score Board");
        string json = PlayerPrefs.GetString("Scores", "{}");
        Debug.Log("HERE: " + json);
        var scores = scoreManager.GetHighScores().ToArray();
        string allScores = "";
        foreach (Score scoreObject in scores)
        {
            allScores += scoreObject.score.ToString("F1") + ", "; // "F1" specifies one decimal place
        }
        allScores = allScores.TrimEnd(',', ' ');
        Debug.Log("Content of scores: " + allScores);

        for (int i = 0; i < scores.Length; i++)
        {
            var row = Instantiate(highScoreEntry, transform).GetComponent<HighScoreEntry>();
            row.rank.text = (i + 1).ToString();
            row.score.text = TimeSpan.FromSeconds(scores[i].score).ToString(@"mm\:ss");
        }
        
    }
}
