using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    public PipeSpawner pipeSpawner;
    public GameObject player;
    public Texture2D birdGameOver;
    public Sprite birdGameOverSprite;
    public HighestScoreScript highestScoreScript;
    
    public int score = 0;
    public Text scoreText;
    public double spawnDelay = 70;
    public double speed = -2.5f;

    public bool gameIsOver = false;
    public GameObject gameOverScreen;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        highestScoreScript = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<HighestScoreScript>();
        
        Rect rect = new Rect(0, 0, birdGameOver.width, birdGameOver.height);
        Vector2 vec = new Vector2(0.5f, 0.5f);
        birdGameOverSprite = Sprite.Create(birdGameOver, rect, vec);
    }

    // Update is called once per frame
    void Update()
    {
        if (score == 10 && speed.Equals(-2.5f))
        {
            speed = -3.5f;
            spawnDelay = 45;
        }

        if (score == 25 && speed.Equals(-3.5f))
        {
            speed = -5f;
            spawnDelay = 30;
        }
        
        if (score == 50 && speed.Equals(-5f))
        {
            speed = -6.5f;
            spawnDelay = 23;
        }
        
        if (score == 80 && speed.Equals(-6.5f))
        {
            speed = -7.5f;
            spawnDelay = 18;
        }
        
        if (score == 130 && speed.Equals(-7.5f))
        {
            speed = -8.5f;
            spawnDelay = 14;
        }
        
        if (score == 200 && speed.Equals(-8.5f))
        {
            speed = -10f;
            spawnDelay = 11;
        }
        
        if (score == 300 && speed.Equals(-10f))
        {
            speed = -11f;
            spawnDelay = 10;
        }
    }

    public void addScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void gameOver()
    {
        if (gameIsOver == false)
        {
            Destroy(player, 3f);
            player.GetComponent<SpriteRenderer>().sprite = birdGameOverSprite;
            gameOverScreen.SetActive(true);
            gameIsOver = true;
            highestScoreScript.scoreUpdate(score);
        }
    }

    public void restartGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void backToMenu()
    {
        SceneManager.LoadSceneAsync("StartMenu", LoadSceneMode.Single);
    }
}
