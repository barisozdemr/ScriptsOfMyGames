using UnityEngine;

public class GoodBirdScript : MonoBehaviour
{
    public AudioManagerScript audioManagerScript;
    
    public bool birdImpactSoundAvailable = true;
    public float birdImpactSoundAvailableDelay = 1f;
    
    public bool groundImpactSoundAvailable = false;
    public float groundImpactSoundAvailableDelay = 2f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManagerScript = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void makeBirdImpactSoundAvailable()
    {
        birdImpactSoundAvailable = true;
    }

    //=======================================================
    public void makeGroundImpactSoundAvailableDelay()
    {
        Invoke("makeGroundImpactSoundAvailable", 0.1f);
    }

    void makeGroundImpactSoundAvailable()
    {
        groundImpactSoundAvailable = true;
    }
    //=======================================================

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("goodBird") || !collision.gameObject.CompareTag("badBird"))
        {
            if (collision.gameObject.CompareTag("GroundBoxCollider"))
            {
                if (groundImpactSoundAvailable)
                {
                    groundImpactSoundAvailable = false;
                    audioManagerScript.playGroundImpactSound();
                    Invoke("makeGroundImpactSoundAvailable", groundImpactSoundAvailableDelay);
                }
            }
            else if(birdImpactSoundAvailable)
            {
                birdImpactSoundAvailable = false;
                audioManagerScript.playBirdImpactSound();
                Invoke("makeBirdImpactSoundAvailable", birdImpactSoundAvailableDelay);
            }
        }
    }
}
