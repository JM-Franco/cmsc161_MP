using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private List<Score> scores;
    
    void Awake()
    {
        var json = PlayerPrefs.GetString("Scores", "{}");
        scores = JsonUtility.FromJson<List<Score>>(json);
    }

    public IEnumerable<Score> GetHighScores()
    {
        return scores.OrderByDescending(x => x.score);
    }

    public void AddScore(Score score)
    {
        scores.Add(score);
    }

    public void OnDestroy()
    {
        SaveScore();
    }

    public void SaveScore()
    {
        var json = JsonUtility.ToJson(scores);
        PlayerPrefs.SetString("scores", json);
    }
}
