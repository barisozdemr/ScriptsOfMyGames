using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
    public string SETTINGSDATAPATH = "settings.txt";
    
    public AudioManagerScript audioManagerScript;
    
    public GameObject fpsTexts;
    public Text FPS;
    
    public Text stageCountText;
    public int stageCount;
    
    public Text levelCountText;
    public int levelCount;
    
    public GameObject levelFailedScreen;
    public Image[] failedScreenImages;
    public Image failedScreenBackgroundImage;
    
    public GameObject levelPassedScreen;
    public Image[] passedScreenImages;
    public Image passedScreenBackgroundImage;

    public float coroutineTimer = 0;
    public int coroutineLoopCounter = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManagerScript = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManagerScript>();
        
        if (File.Exists(SETTINGSDATAPATH)) //-------FPS
        {
            string[] lines = File.ReadAllLines(SETTINGSDATAPATH);
            
            string data;
            
            data = lines[5].Split(":")[1];
            Application.targetFrameRate = int.Parse(data);
            
            data = lines[4].Split(":")[1];
            if (data == "on")
            {
                fpsTexts.SetActive(true);
                StartCoroutine(fps());
            }
        }
        
        levelCount = int.Parse(levelCountText.text);
        stageCount = int.Parse(stageCountText.text);

        failedScreenImages = levelFailedScreen.GetComponentsInChildren<Image>();
        
        passedScreenImages = levelPassedScreen.GetComponentsInChildren<Image>();
        
        Color color = new Color(255, 255, 255, 0);
        
        for (int i=0 ; i < failedScreenImages.Length ; i++) // make alphas zero
        {
            if (failedScreenImages[i].transform.name == "BlackBackground")
            {
                failedScreenBackgroundImage = failedScreenImages[i];
                failedScreenBackgroundImage.color = color;
            }
            failedScreenImages[i].color = color;
        }
        
        for (int i=0 ; i < passedScreenImages.Length ; i++) // make alphas zero
        {
            if (passedScreenImages[i].transform.name == "BlackBackground")
            {
                passedScreenBackgroundImage = passedScreenImages[i];
                passedScreenBackgroundImage.color = color;
            }
            passedScreenImages[i].color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //================================================================================
    public void levelSelectButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        Invoke("loadLevelSelectScene", audioManagerScript.defaultButtonSoundClipLength);
    }
    
    public void loadLevelSelectScene()
    {
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
    }
    //================================================================================
    //================================================================================
    public void menuButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        Invoke("loadMenuScene", audioManagerScript.defaultButtonSoundClipLength);
    }
    
    public void loadMenuScene()
    {
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }
    //================================================================================
    //================================================================================
    public void nextLevelButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        Invoke("nextLevel", audioManagerScript.defaultButtonSoundClipLength);
    }
    
    public void nextLevel()
    {
        string stageText = "Stage";
        string levelText = "Level";

        string stage = string.Concat(stageText, stageCount); //StageX
        
        levelCount = int.Parse(levelCountText.text) + 1;
        string level = string.Concat(levelText, levelCount); //LevelY
        
        string nextLevel = string.Concat(stage, level); //StageXLevelY

        if (stageCount == 1 && levelCount == 5)
        {
            SceneManager.LoadScene("Stage2Level1", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
        }
    }
    //================================================================================
    //================================================================================
    public void tryOrPlayAgainButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        Invoke("reloadLevel", audioManagerScript.defaultButtonSoundClipLength);
    }
    
    public void reloadLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    //================================================================================
    
    public IEnumerator showLevelFailedScreen()
    {
        levelFailedScreen.SetActive(true);
        
        Color color = new Color(1f, 1f, 1f, 0f);
        Color backgroundColor = new Color(0f, 0f, 0f, 0f);

        while (true)
        {
            if (coroutineLoopCounter >= 50)
            {
                yield break;
            }
            
            coroutineTimer += Time.deltaTime;
            
            if (coroutineTimer >= 0.01f)
            {
                coroutineTimer = 0;
                
                color.a += 0.02f;
                backgroundColor.a += 0.017f;
                
                for (int i=0 ; i<failedScreenImages.Length ; i++)
                {
                    failedScreenImages[i].color = color;
                }
                failedScreenBackgroundImage.color = backgroundColor;

                coroutineLoopCounter += 1;
            }
            
            yield return null;
        }
    }

    public IEnumerator showLevelPassedScreen()
    {
        levelPassedScreen.SetActive(true);
        
        Color color = new Color(1f, 1f, 1f, 0f);
        Color backgroundColor = new Color(0f, 0f, 0f, 0f);

        while (true)
        {
            if (coroutineLoopCounter >= 50)
            {
                yield break;
            }
            
            coroutineTimer += Time.deltaTime;
            
            if (coroutineTimer >= 0.01f)
            {
                coroutineTimer = 0;
                
                color.a += 0.02f;
                backgroundColor.a += 0.017f;
                
                for (int i=0 ; i<passedScreenImages.Length ; i++)
                {
                    passedScreenImages[i].color = color;
                }
                passedScreenBackgroundImage.color = backgroundColor;

                coroutineLoopCounter += 1;
            }
            
            yield return null;
        }
    }
    
    public IEnumerator fps()
    {
        while (true)
        {
            FPS.text = Mathf.RoundToInt(1 / Time.deltaTime).ToString();
            
            yield return new WaitForSeconds(0.05f);
        }
    }
}
