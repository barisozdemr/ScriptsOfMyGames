using UnityEngine;

public class WoodBlockScript : MonoBehaviour
{
    public AudioManagerScript audioManagerScript;

    public bool isMuted = true;
    public float muteDuration = 3f;
    
    public bool woodImpactSoundAvailable = true;
    public float woodImpactSoundAvailableDelay = 1.5f;
    
    public bool groundImpactSoundAvailable = true;
    public float groundImpactSoundAvailableDelay = 2f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManagerScript = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManagerScript>();
        
        Invoke("unmute", muteDuration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void makeGroundImpactSoundAvailable()
    {
        groundImpactSoundAvailable = true;
    }
    
    void makeWoodImpactSoundAvailable()
    {
        woodImpactSoundAvailable = true;
    }

    void unmute()
    {
        isMuted = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isMuted)
        {
            int random = Random.Range(1, 3);
            
            if (collision.gameObject.CompareTag("GroundBoxCollider"))
            {
                if (groundImpactSoundAvailable)
                {
                    groundImpactSoundAvailable = false;
                    audioManagerScript.playGroundImpactSound();
                    Invoke("makeGroundImpactSoundAvailable", groundImpactSoundAvailableDelay);
                }
            }
            else if(woodImpactSoundAvailable)
            {
                woodImpactSoundAvailable = false;
                audioManagerScript.playWoodImpactSound();
                Invoke("makeWoodImpactSoundAvailable", woodImpactSoundAvailableDelay);
            }
        }
    }
}