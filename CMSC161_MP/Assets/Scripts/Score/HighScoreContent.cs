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
        var scores = scoreManager.GetHighScores().ToArray();
        for (int i = 0; i < scores.Length; i++)
        {
            var row = Instantiate(highScoreEntry, transform).GetComponent<HighScoreEntry>();
            row.rank.text = (i + 1).ToString();
            row.score.text = TimeSpan.FromSeconds(scores[i].score).ToString(@"mm\:ss");
        }
        
    }
}
