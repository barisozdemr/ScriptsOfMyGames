using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HighestScoreScript : MonoBehaviour
{
    public string scorePath = "score.txt";
    public Text score;
    public int previousScore;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        previousScore = int.Parse(File.ReadAllText(scorePath));
        
        if (File.Exists(scorePath))
        {
            score.text = previousScore.ToString();
        }
        else
        {
            score.text = "0";
            File.WriteAllText(scorePath, "0");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void scoreUpdate(int newScore)
    {
        if (newScore > previousScore)
        {
            previousScore = newScore;
            File.WriteAllText(scorePath, newScore.ToString());
            score.text = newScore.ToString();
        }
    }
}
