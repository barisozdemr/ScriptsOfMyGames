using System.Collections;
using UnityEngine;
using System.IO;

public class AudioManagerScript : MonoBehaviour
{
    public string SETTINGSDATAPATH = "settings.txt";
    
    public bool musicOn = true;
    public int musicVolumePercentage = 50;
    public bool soundEffectsOn = true;
    public int soundEffectsVolumePercentage = 50;
    
    public AudioSource musicAudioSource;
    public AudioSource soundEffectsAudioSource;
    
    public AudioClip musicClip;
        
    public AudioClip defaultButtonSoundClip;
    public float defaultButtonSoundClipLength;
    public AudioClip changeStageButtonSoundClip;
    public float changeStageButtonSoundClipLength;
    public AudioClip slidingStageSoundClip;
    public float slidingStageSoundClipLength;
    
    public AudioClip birdDyingSoundClip;
    
    public AudioClip rubberStrechingSoundClip;

    public AudioClip flybyWhooshSoundClip;

    public AudioClip birdImpactSound;
    public bool birdImpactSoundAvailable = true;
    public float birdImpactSoundAvailableDelay = 0.3f;
    
    public AudioClip groundImpactSound1;
    public AudioClip groundImpactSound2;
    public bool groundImpactSoundAvailable = true;
    public float groundImpactSoundAvailableDelay = 0.1f;
    
    public AudioClip stoneImpactSound1;
    public AudioClip stoneImpactSound2;
    public bool stoneImpactSoundAvailable = true;
    public float stoneImpactSoundAvailableDelay = 0.1f;
    
    public AudioClip woodImpactSound1;
    public AudioClip woodImpactSound2;
    public bool woodImpactSoundAvailable = true;
    public float woodImpactSoundAvailableDelay = 0.1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        
        musicAudioSource = sources[0];
        soundEffectsAudioSource = sources[1];
        
        setClipLengths();
        setVolumeSettings();
        setVolumeLevels();
        
        playMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void playMusic()
    {
        if (musicOn)
        {
            musicAudioSource.clip = musicClip;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
    }
    
    void makeBirdImpactSoundAvailable()
    {
        birdImpactSoundAvailable = true;
    }
    
    void makeStoneImpactSoundAvailable()
    {
        stoneImpactSoundAvailable = true;
    }
    
    void makeWoodImpactSoundAvailable()
    {
        woodImpactSoundAvailable = true;
    }
    
    void makeGroundImpactSoundAvailable()
    {
        groundImpactSoundAvailable = true;
    }
    
    public void setVolumeSettings()
    {
        if (File.Exists(SETTINGSDATAPATH))
        {
            string[] lines = File.ReadAllLines(SETTINGSDATAPATH);

            for (int i = 0; i < 4; i++)
            {
                string data = lines[i].Split(":")[1];

                if (i == 0)
                {
                    if (data == "on")
                    {
                        if (musicOn == false)
                        {
                            musicOn = true;
                            playMusic();
                        }
                    }
                    else if (data == "off")
                    {
                        if (musicOn == true)
                        {
                            musicOn = false;
                            musicAudioSource.Stop();
                        }
                    }
                }
                else if (i == 1)
                {
                    musicVolumePercentage = int.Parse(data);
                }
                else if (i == 2)
                {
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
                    soundEffectsVolumePercentage = int.Parse(data);
                }
            }
        }
    }

    public void setVolumeLevels()
    {
        soundEffectsAudioSource.volume = soundEffectsVolumePercentage/100f;
        musicAudioSource.volume = musicVolumePercentage/100f;
    }

    public void setClipLengths()
    {
        defaultButtonSoundClipLength = defaultButtonSoundClip.length;
        changeStageButtonSoundClipLength = changeStageButtonSoundClip.length;
        slidingStageSoundClipLength = slidingStageSoundClip.length;
    }

    public void playDefaultButtonSound()
    {
        if (soundEffectsOn)
        {
            soundEffectsAudioSource.PlayOneShot(defaultButtonSoundClip);
        }
    }
    
    public void playChangeStageButtonSound()
    {
        if (soundEffectsOn)
        {
            soundEffectsAudioSource.PlayOneShot(changeStageButtonSoundClip);
        }
    }
    
    public void playSlidingStageSound()
    {
        if (soundEffectsOn)
        {
            soundEffectsAudioSource.PlayOneShot(slidingStageSoundClip);
        }
    }
    
    public void playBirdDyingSound()
    {
        if (soundEffectsOn)
        {
            soundEffectsAudioSource.PlayOneShot(birdDyingSoundClip);
        }
    }

    public void playRubberStrechingSound()
    {
        if (soundEffectsOn)
        {
            soundEffectsAudioSource.PlayOneShot(rubberStrechingSoundClip);
        }
    }

    public void playFlybyWhooshSound()
    {
        if (soundEffectsOn)
        {
            soundEffectsAudioSource.PlayOneShot(flybyWhooshSoundClip);
        }
    }
    
    //==================================================================== Impact Sound Effects
    public void playBirdImpactSound()
    {
        if (soundEffectsOn && birdImpactSoundAvailable)
        {
            birdImpactSoundAvailable = false;
            soundEffectsAudioSource.PlayOneShot(birdImpactSound);
            Invoke("makeBirdImpactSoundAvailable", birdImpactSoundAvailableDelay);
        }
    }

    public void playGroundImpactSound()
    {
        if (soundEffectsOn && groundImpactSoundAvailable)
        {
            groundImpactSoundAvailable = false;
            
            soundEffectsAudioSource.PlayOneShot(groundImpactSound2);
            
            Invoke("makeGroundImpactSoundAvailable", groundImpactSoundAvailableDelay);
        }
    }

    public void playStoneImpactSound()
    {
        if (soundEffectsOn && stoneImpactSoundAvailable)
        {
            stoneImpactSoundAvailable = false;
            
            int random = Random.Range(1, 3);
            if (random == 1)
            {
                soundEffectsAudioSource.PlayOneShot(stoneImpactSound1);
            }
            else
            {
                soundEffectsAudioSource.PlayOneShot(stoneImpactSound2);
            }
            
            Invoke("makeStoneImpactSoundAvailable", stoneImpactSoundAvailableDelay);
        }
    }

    public void playWoodImpactSound()
    {
        if (soundEffectsOn && woodImpactSoundAvailable)
        {
            woodImpactSoundAvailable = false;
            
            int random = Random.Range(1, 3);
            if (random == 1)
            {
                soundEffectsAudioSource.PlayOneShot(woodImpactSound1);
            }
            else
            {
                soundEffectsAudioSource.PlayOneShot(woodImpactSound2);
            }
            
            Invoke("makeWoodImpactSoundAvailable", woodImpactSoundAvailableDelay);
        }
    }
    //===========================================================================================
}
