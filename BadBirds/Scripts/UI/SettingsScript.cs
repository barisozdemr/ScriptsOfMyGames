using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SettingsScript : MonoBehaviour
{
    public string SETTINGSDATAPATH = "settings.txt";
    
    public AudioManagerScript audioManagerScript;
    
    public Camera cam;

    public GameObject fpsTexts;
    public Text FPS;
    public Coroutine fpsCoroutine;
    
    public Transform sliderTraceParentTransform;

    public Sprite sliderButtonTraceSprite;
    
    //=============================================================== Music
    public Button musicMuteButton;
    
    public GameObject musicSliderButton;
    public RectTransform musicSliderButtonTransform;
    public float musicSliderLeftAnchor = -706;
    public float musicSliderRightAnchor = -112;
    public float musicSliderYAnchor = 43;
    
    public Text musicCountText;
    public int musicCount = 50;
    
    public bool musicOn = true;
    public Sprite musicOpenSprite;
    public Sprite musicMutedSprite;
    
    public GameObject musicSliderTrace;
    public float musicSliderTraceLeftAnchor = -732;
    public float musicSliderTraceAltitude = 43;

    //=============================================================== Sound Effects
    public Button soundEffectsMuteButton;
    
    public GameObject soundEffectsSliderButton;
    public RectTransform soundEffectsSliderButtonTransform;
    public float soundEffectsSliderLeftAnchor = -706;
    public float soundEffectsSliderRightAnchor = -112;
    public float soundEffectsSliderYAnchor = -100.5f;
    
    public Text soundEffectsCountText;
    public int soundEffectsCount = 50;
    
    public bool soundEffectsOn = true;
    public Sprite soundEffectsOpenSprite;
    public Sprite soundEffectsMutedSprite;
    
    public GameObject soundEffectsSliderTrace;
    public float soundEffectsSliderTraceLeftAnchor = -732;
    public float soundEffectsSliderTraceAltitude = -100.5f;

    //=============================================================== Fps
    public Button showFpsButton;
    
    public GameObject maxFpsSliderButton;
    public RectTransform maxFpsSliderButtonTransform;
    public float maxFpsSliderLeftAnchor = -579;
    public float maxFpsSliderRightAnchor = 112;
    public float maxFpsSliderYAnchor = -383;
    
    public Text maxFpsCountText;
    public int maxFpsCount = 500;
    
    public bool fpsShowing = false;
    public Sprite showFpsSprite;
    public Sprite dontShowFpsSprite;

    public GameObject maxFpsSliderTrace;
    public float maxFpsSliderTraceLeftAnchor = -601.5f;
    public float maxFpsSliderTraceAltitude = -383;
    
    void Start()
    {
        soundEffectsSliderButtonTransform = soundEffectsSliderButton.GetComponent<RectTransform>();
        musicSliderButtonTransform = musicSliderButton.GetComponent<RectTransform>();
        maxFpsSliderButtonTransform = maxFpsSliderButton.GetComponent<RectTransform>();
        
        audioManagerScript = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManagerScript>();
        
        readSettings();
        setSettings();

        if (fpsShowing)
        {
            fpsTexts.SetActive(true);
            fpsCoroutine = StartCoroutine(fps());
        }
    }
    
    void Update()
    {
        
    }

    public void playButtonSound()
    {
        audioManagerScript.playDefaultButtonSound();
    }

    //================================================================================
    public void menuButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        Invoke("backToMenu", audioManagerScript.defaultButtonSoundClipLength);
    }
    
    public void backToMenu()
    {
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }
    //================================================================================

    public void saveSettings()
    {
        if (! File.Exists(SETTINGSDATAPATH))
        {
            File.WriteAllText(SETTINGSDATAPATH, "");
        }

        string text = "";
        //========================================== Music
        text = "music:";
        text += musicOn ? "on\n" : "off\n";

        text += "musicCount:";
        text += musicCount+"\n";
        
        //========================================== SoundEffects
        text += "soundEffects:";
        text += soundEffectsOn ? "on\n" : "off\n";
        
        text += "soundEffectsCount:";
        text += soundEffectsCount+"\n";
        
        //========================================== Fps
        text += "showFps:";
        text += fpsShowing ? "on\n" : "off\n";
        
        text += "maxFps:";
        text += maxFpsCount;
        
        File.WriteAllText(SETTINGSDATAPATH,text);
        
        audioManagerScript.setVolumeSettings();
        audioManagerScript.setVolumeLevels();
    }

    public void readSettings()
    {
        if (!File.Exists(SETTINGSDATAPATH))
        {
            return;
        }
        
        string[] lines = File.ReadAllLines(SETTINGSDATAPATH);

        for (int i = 0; i < 6; i++)
        {
            if (i == 0)
            {
                string data = lines[i].Split(":")[1];
                if (data == "on")
                {
                    musicOn = true;
                }
                else if (data == "off")
                {
                    musicOn = false;
                }
            }
            else if (i == 1)
            {
                musicCount = int.Parse(lines[i].Split(":")[1]);
            }
            else if (i == 2)
            {
                string data = lines[i].Split(":")[1];
                if (data == "on")
                {
                    soundEffectsOn = true;
                }
                else if (data == "off")
                {
                    soundEffectsOn = false;
                }
            }
            else if (i == 3)
            {
                soundEffectsCount = int.Parse(lines[i].Split(":")[1]);
            }
            else if (i == 4)
            {
                string data = lines[i].Split(":")[1];
                if (data == "on")
                {
                    fpsShowing = true;
                }
                else if (data == "off")
                {
                    fpsShowing = false;
                }
            }
            else if (i == 5)
            {
                maxFpsCount = int.Parse(lines[i].Split(":")[1]);
            }
        }
    }

    public void setSettings()
    {
        toggleMusic();
        toggleMusic();
        
        toggleSoundEffects();
        toggleSoundEffects();
        
        toggleShowFps();
        toggleShowFps();
        
        musicCountText.text = musicCount.ToString();
        
        soundEffectsCountText.text = soundEffectsCount.ToString();
        
        maxFpsCountText.text = maxFpsCount.ToString();
        Application.targetFrameRate = maxFpsCount;

        //============================================================================= Set Slider Buttons
        float sliderButtonRightAnchor;
        
        sliderButtonRightAnchor = calculateSliderRightAnchorByPercent( // music
            musicSliderLeftAnchor,
            musicSliderRightAnchor,
            musicCount
        );
        musicSliderButtonTransform.anchoredPosition = new Vector2(sliderButtonRightAnchor, musicSliderYAnchor);
        
        sliderButtonRightAnchor = calculateSliderRightAnchorByPercent( // soundEffects
            soundEffectsSliderLeftAnchor,
            soundEffectsSliderRightAnchor,
            soundEffectsCount
        );
        soundEffectsSliderButtonTransform.anchoredPosition = new Vector2(sliderButtonRightAnchor, soundEffectsSliderYAnchor);
        
        sliderButtonRightAnchor = calculateSliderRightAnchorByFps( // fps
            maxFpsSliderLeftAnchor,
            maxFpsSliderRightAnchor,
            maxFpsCount
        );
        maxFpsSliderButtonTransform.anchoredPosition = new Vector2(sliderButtonRightAnchor, maxFpsSliderYAnchor);

        //============================================================================= Set Slider Traces
        float traceRightAnchor;
        
        traceRightAnchor = calculateSliderRightAnchorByPercent( // music
            musicSliderTraceLeftAnchor,
            musicSliderRightAnchor,
            musicCount
            );
        updateSliderTrace(musicSliderTraceLeftAnchor, traceRightAnchor, musicSliderTraceAltitude, ref musicSliderTrace);
        
        traceRightAnchor = calculateSliderRightAnchorByPercent( // soundEffects
            soundEffectsSliderTraceLeftAnchor,
            soundEffectsSliderRightAnchor,
            soundEffectsCount
        );
        updateSliderTrace(soundEffectsSliderTraceLeftAnchor, traceRightAnchor, soundEffectsSliderTraceAltitude, 
            ref soundEffectsSliderTrace);
        
        traceRightAnchor = calculateSliderRightAnchorByFps( // fps
            maxFpsSliderTraceLeftAnchor,
            maxFpsSliderRightAnchor,
            maxFpsCount
        );
        updateSliderTrace(maxFpsSliderTraceLeftAnchor, traceRightAnchor, maxFpsSliderTraceAltitude, ref maxFpsSliderTrace);
    }

    //======================================================================== Music
    public void musicMuteButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        toggleMusic();
    }
    
    public void toggleMusic()
    {
        if (musicOn)
        {
            musicMuteButton.image.sprite = musicMutedSprite;
            musicOn = false;
        }
        else
        {
            musicMuteButton.image.sprite = musicOpenSprite;
            musicOn = true;
        }
    }

    public void musicDragged()
    {
        Vector2 mousePosition = Input.mousePosition;
        
        Vector2 uiPos = new Vector2(mousePosition.x-960, mousePosition.y-540);
        
        float clampedX;
        if (uiPos.x < musicSliderLeftAnchor)
        {
            clampedX = musicSliderLeftAnchor;
        }
        else if (uiPos.x > musicSliderRightAnchor)
        {
            clampedX = musicSliderRightAnchor;
        }
        else
        {
            clampedX = uiPos.x;
        }
        
        musicCount = calculatePercent(musicSliderLeftAnchor, musicSliderRightAnchor, clampedX);
        musicCountText.text = musicCount.ToString();
        
        musicSliderButtonTransform.anchoredPosition = new Vector2(clampedX, musicSliderYAnchor);
        
        updateSliderTrace(musicSliderTraceLeftAnchor, clampedX, musicSliderTraceAltitude, ref musicSliderTrace);
    }
    
    //======================================================================== Sound Effects
    public void soundEffectsMuteButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        toggleSoundEffects();
    }
    
    public void toggleSoundEffects()
    {
        if (soundEffectsOn)
        {
            soundEffectsMuteButton.image.sprite = soundEffectsMutedSprite;
            soundEffectsOn = false;
        }
        else
        {
            soundEffectsMuteButton.image.sprite = soundEffectsOpenSprite;
            soundEffectsOn = true;
        }
    }
    
    public void soundEffectsDragged()
    {
        Vector2 mousePosition = Input.mousePosition;
        
        Vector2 uiPos = new Vector2(mousePosition.x-960, mousePosition.y-540);

        float clampedX;
        if (uiPos.x < soundEffectsSliderLeftAnchor)
        {
            clampedX = soundEffectsSliderLeftAnchor;
        }
        else if (uiPos.x > soundEffectsSliderRightAnchor)
        {
            clampedX = soundEffectsSliderRightAnchor;
        }
        else
        {
            clampedX = uiPos.x;
        }
        
        soundEffectsCount = calculatePercent(soundEffectsSliderLeftAnchor, soundEffectsSliderRightAnchor, clampedX);
        soundEffectsCountText.text = soundEffectsCount.ToString();
        
        soundEffectsSliderButtonTransform.anchoredPosition = new Vector2(clampedX, soundEffectsSliderYAnchor);
        
        updateSliderTrace(soundEffectsSliderTraceLeftAnchor, clampedX, soundEffectsSliderTraceAltitude, ref soundEffectsSliderTrace);
    }
    
    //======================================================================== Fps
    public void showFpsButtonClicked()
    {
        audioManagerScript.playDefaultButtonSound();
        toggleShowFps();
    }
    
    public void toggleShowFps()
    {
        if (fpsShowing)
        {
            showFpsButton.image.sprite = dontShowFpsSprite;
            fpsShowing = false;
            if (fpsCoroutine != null)
            {
                StopCoroutine(fpsCoroutine);
                fpsCoroutine = null;
            }
            fpsTexts.SetActive(false);
        }
        else
        {
            showFpsButton.image.sprite = showFpsSprite;
            fpsShowing = true;
            fpsCoroutine = StartCoroutine(fps());
            fpsTexts.SetActive(true);
        }
    }
    
    public void maxFpsDragged()
    {
        Vector2 mousePosition = Input.mousePosition;

        Vector2 uiPos = new Vector2(mousePosition.x-960, mousePosition.y-540);
        
        float clampedX;
        if (uiPos.x < maxFpsSliderLeftAnchor)
        {
            clampedX = maxFpsSliderLeftAnchor;
        }
        else if (uiPos.x > maxFpsSliderRightAnchor)
        {
            clampedX = maxFpsSliderRightAnchor;
        }
        else
        {
            clampedX = uiPos.x;
        }
        
        maxFpsCount = calculateFps(maxFpsSliderLeftAnchor, maxFpsSliderRightAnchor, clampedX);
        maxFpsCountText.text = maxFpsCount.ToString();
        Application.targetFrameRate = maxFpsCount;
        
        maxFpsSliderButtonTransform.anchoredPosition = new Vector2(clampedX, maxFpsSliderYAnchor);
        
        updateSliderTrace(maxFpsSliderTraceLeftAnchor, clampedX, maxFpsSliderTraceAltitude, ref maxFpsSliderTrace);
    }

    int calculateFps(float min, float max, float value)
    {
        float distance = max - min;
        
        float valueDistance = value - min;
        
        return Mathf.RoundToInt((valueDistance / distance) * 470 ) + 30;
    }
    
    int calculatePercent(float min, float max, float value)
    {
        float distance = max - min;

        float valueDistance = value - min;
        
        return Mathf.RoundToInt((valueDistance / distance) * 100 );
    }

    void updateSliderTrace(float left, float right, float altitudeY, ref GameObject trace)
    {
        if (trace == null)
        {
            trace = new GameObject("trace");
            trace.transform.SetParent(sliderTraceParentTransform);
            
            RectTransform traceRectTransform = trace.AddComponent<RectTransform>();
            
            Image traceImage = trace.AddComponent<Image>();
            traceImage.sprite = sliderButtonTraceSprite;
        }
        
        RectTransform rectT = trace.GetComponent<RectTransform>();
        
        Vector2 position = new Vector2(right+(-(right-left) / 2), altitudeY);
        rectT.anchoredPosition = position;
        
        Vector2 scale = new Vector2((right-left) / 100, 36f / 100);
        rectT.localScale = scale;
    }

    float calculateSliderRightAnchorByPercent(float minLeft, float maxRight, int percent)
    {
        return maxRight - (maxRight - minLeft) * (1-(percent / 100f));
    }
    
    float calculateSliderRightAnchorByFps(float minLeft, float maxRight, int fps)
    {
        return maxRight - (maxRight - minLeft) * (1-((fps-30) / 470f));
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
