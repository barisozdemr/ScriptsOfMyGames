using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class LevelSelectionScript : MonoBehaviour
{
    public string SETTINGSDATAPATH = "settings.txt";
    public string LEVELDATAPATH = "levelData.txt";
    
    public AudioManagerScript audioManagerScript;
    
    public GameObject fpsTexts;
    public Text FPS;

    public RectTransform stage1;
    public RectTransform stage2;
    public RectTransform[] stages = new RectTransform[2];
    
    public Vector2 leftGap = new Vector2(-1920, 0);
    public Vector2 rightGap = new Vector2(1920, 0);
    public Vector2 center = new Vector2(0, 0);

    public Coroutine slideStageCoroutine;
    
    public float stageMovementSpeed = 5000;
    
    public int stageCount=1;

    public int levelCount=1;

    public GameObject stage1Level2LockedButton;
    public GameObject stage1Level3LockedButton;
    public GameObject stage1Level4LockedButton;
    
    public GameObject stage2Level1LockedButton;
    public GameObject stage2Level2LockedButton;
    public GameObject stage2Level3LockedButton;
    public GameObject stage2Level4LockedButton;
    public GameObject stage2Level5LockedButton;
    public GameObject stage2Level6LockedButton;
    
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
        
        stages[0] = stage1;
        stages[1] = stage2;
        
        setUnlockedLevels();
        
        Invoke("startFromStage2IfUnlocked", 0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void startFromStage2IfUnlocked()
    {
        if (! stage2Level1LockedButton.activeSelf)
        {
            goNextStageButtonClicked();
        }
    }

    public void setUnlockedLevels()
    {
        if (! File.Exists(LEVELDATAPATH))
        {
            File.WriteAllText(LEVELDATAPATH, "1;");
        }
        else
        {
            string[] lines = File.ReadLines(LEVELDATAPATH).ToArray();

            foreach (string line in lines)
            {
                string[] data = line.Split(";");
                if (data[1] != null)
                {
                    string[] levelData = data[1].Split(",");
                    foreach (string level in levelData)
                    {
                        if (data[0] == "1") //--- stage control
                        {
                            if (level == "2")
                            {
                                stage1Level2LockedButton.SetActive(false);
                            }
                            else if (level == "3")
                            {
                                stage1Level3LockedButton.SetActive(false);
                            }
                            else if (level == "4")
                            {
                                stage1Level4LockedButton.SetActive(false);
                            }
                        }
                        
                        if (data[0] == "2")
                        {
                            if (level == "1")
                            {
                                stage2Level1LockedButton.SetActive(false);
                            }
                            if (level == "2")
                            {
                                stage2Level2LockedButton.SetActive(false);
                            }
                            if (level == "3")
                            {
                                stage2Level3LockedButton.SetActive(false);
                            }
                            if (level == "4")
                            {
                                stage2Level4LockedButton.SetActive(false);
                            }
                            if (level == "5")
                            {
                                stage2Level5LockedButton.SetActive(false);
                            }
                            if (level == "6")
                            {
                                stage2Level6LockedButton.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }

    //================================================================================
    public void levelButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        Invoke("loadLevel", audioManagerScript.defaultButtonSoundClipLength);
    }
    
    public void loadLevel()
    {
        string stageText = "Stage";
        string stage = string.Concat(stageText, stageCount);
        
        string levelText = "Level";
        string level = string.Concat(levelText, levelCount);

        string scene = string.Concat(stage, level);
        
        SceneManager.LoadScene(scene);
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

    public void goNextStageButtonClicked()
    {
        audioManagerScript.playChangeStageButtonSound();
        if (stageCount < stages.Length)
        {
            stageCount++;
            audioManagerScript.playSlidingStageSound();
            
            if (slideStageCoroutine != null)
            {
                StopCoroutine(slideStageCoroutine);
                slideStageCoroutine = StartCoroutine(slideToNextStage());
            }
            else
            {
                slideStageCoroutine = StartCoroutine(slideToNextStage());
            }
        }
    }
    
    public void goPreviousStageButtonClicked()
    {
        audioManagerScript.playChangeStageButtonSound();
        if (stageCount > 1)
        {
            stageCount--;
            audioManagerScript.playSlidingStageSound();
            
            if (slideStageCoroutine != null)
            {
                StopCoroutine(slideStageCoroutine);
                slideStageCoroutine = StartCoroutine(slideToPreviousStage());
            }
            else
            {
                slideStageCoroutine = StartCoroutine(slideToPreviousStage());
            }
        }
    }

    // ----------------------------------------------------------------------------- Update Level Count
    public void level1Selected()
    {
        levelCount=1;
    }
    
    public void level2Selected()
    {
        levelCount=2;
    }
    
    public void level3Selected()
    {
        levelCount=3;
    }
    
    public void level4Selected()
    {
        levelCount=4;
    }
    
    public void level5Selected()
    {
        levelCount=5;
    }
    
    public void level6Selected()
    {
        levelCount=6;
    }
    // -----------------------------------------------------------------------------------------------------------------

    IEnumerator slideToNextStage()
    {
        float centerX = center.x;
        
        while(true)
        {
        
            for (int i=(stageCount-1) ; i>=(stageCount-2) ; i--)
            {
                float newX = stages[i].anchoredPosition.x - stageMovementSpeed * Time.deltaTime;;
                Vector2 newVector = new Vector2(newX, stages[i].anchoredPosition.y);
                stages[i].anchoredPosition = newVector;
            }
        
            if (stages[stageCount-1].anchoredPosition.x <= centerX)
            {
                stages[stageCount-1].anchoredPosition = center;
                stages[stageCount-2].anchoredPosition = leftGap;

                slideStageCoroutine = null;
                yield break;
            }

            yield return null;
        }
    }
    
    IEnumerator slideToPreviousStage()
    {
        float centerX = center.x;

        while(true) 
        {
        
            for (int i=(stageCount) ; i>=(stageCount-1) ; i--)
            {
                float newX = stages[i].anchoredPosition.x + stageMovementSpeed * Time.deltaTime;;
                Vector2 newVector = new Vector3(newX, stages[i].anchoredPosition.y);
                stages[i].anchoredPosition = newVector;
            }
        
            if (stages[stageCount-1].anchoredPosition.x >= centerX)
            {
                stages[stageCount-1].anchoredPosition = center;
                stages[stageCount].anchoredPosition = rightGap;

                slideStageCoroutine = null;
                yield break;
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