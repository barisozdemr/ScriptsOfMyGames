using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    public string SETTINGSDATAPATH = "settings.txt";
    
    public AudioManagerScript audioManagerScript;
    
    public GameObject fpsTexts;
    public Text FPS;
    
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //================================================================================
    public void playButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        Invoke("loadLevelSelect", audioManagerScript.defaultButtonSoundClipLength);
    }

    public void loadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
    }
    
    //================================================================================
    
    public void settingsButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        Invoke("loadSettings", audioManagerScript.defaultButtonSoundClipLength);
    }
    
    public void loadSettings()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Single);
    }

    //================================================================================
    
    public void quitButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        Invoke("quitGame", audioManagerScript.defaultButtonSoundClipLength);
    }
    
    public void quitGame()
    {
        Application.Quit();
    }
    //================================================================================
    
    public IEnumerator fps()
    {
        while (true)
        {
            FPS.text = Mathf.RoundToInt(1 / Time.deltaTime).ToString();
            
            yield return new WaitForSeconds(0.05f);
        }
    }
}
