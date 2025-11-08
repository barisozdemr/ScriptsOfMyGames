using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManagerScript : MonoBehaviour
{
    public string scorePath = "score.txt";
    public Text score;
    
    void Start()
    {
        if (File.Exists(scorePath))
        {
            score.text = File.ReadAllText(scorePath);
        }
        else
        {
            score.text = "0";
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            startGame();
        }
    }
    
    public void startGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void exitGame()
    {
        Application.Quit();
    }
}