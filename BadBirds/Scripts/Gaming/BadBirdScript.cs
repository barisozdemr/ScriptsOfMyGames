using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BadBirdScript : MonoBehaviour
{
    public AudioManagerScript audioManagerScript;

    public bool isMuted = true;
    public float muteDuration = 3f;

    public GameObject birdDyingParticle;
    
    public bool birdImpactSoundAvailable = true;
    public float birdImpactSoundAvailableDelay = 1f;
    
    public bool groundImpactSoundAvailable = true;
    public float groundImpactSoundAvailableDelay = 2f;
    
    public CircleCollider2D ownCollider;
    public SpriteRenderer ownRenderer;

    public GameObject groundBoxCollider;
    
    public GameObject shootingBird;
    
    public Texture2D deadBirdTexture;
    public Sprite deadBirdSprite;
    
    public bool isDead = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManagerScript = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManagerScript>();
        
        Invoke("unmute", muteDuration);
        
        ownCollider = this.GetComponent<CircleCollider2D>();
        ownRenderer = this.GetComponent<SpriteRenderer>();

        groundBoxCollider = GameObject.FindGameObjectWithTag("GroundBoxCollider");

        deadBirdSprite = Sprite.Create(
            deadBirdTexture,
            new Rect(0, 0, deadBirdTexture.width, deadBirdTexture.height),
            new Vector2(0.5f, 0.5f)
            );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void makeBirdImpactSoundAvailable()
    {
        birdImpactSoundAvailable = true;
    }
    
    void makeGroundImpactSoundAvailable()
    {
        groundImpactSoundAvailable = true;
    }

    void unmute()
    {
        isMuted = false;
    }

    public void updateShootingBird(GameObject bird)
    {
        shootingBird = bird;
    }

    void killBird()
    {
        isDead = true;
        ownRenderer.sprite = deadBirdSprite;
        Instantiate(birdDyingParticle, transform.position, Quaternion.identity);
        audioManagerScript.playBirdDyingSound();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isMuted)
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
        
        if (isDead) return;

        foreach (var contact in collision.contacts)
        {
            GameObject otherGameObject = contact.collider.GameObject();
            
            if (otherGameObject == groundBoxCollider) // fall to the ground
            {
                float impactThreshold = 5.1f;
                
                float impactForce = calculateFallingImpactForce(contact);
                
                if (impactForce >= impactThreshold)
                {
                    killBird();
                }
            }
            
            else if (otherGameObject == shootingBird) //hit by shootingBird
            {
                float impactThreshold = 15f;
                
                float impactForce = otherObjectsImpactForce(contact);
                
                if (impactForce >= impactThreshold)
                {
                    killBird();
                }
            }
            
            else // hit by other objects
            {
                float impactThreshold = 25f;
                
                float impactForce = otherObjectsImpactForce(contact);
                
                if ( impactForce >= impactThreshold)
                {
                    killBird();
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;
        
        foreach (var contact in collision.contacts)
        {
            GameObject otherGameObject = contact.collider.GameObject();
            
            if (otherGameObject != groundBoxCollider && otherGameObject != shootingBird) // hit by other objects
            {
                // ========================================================================== force transfer calculation
                if (contact.relativeVelocity.magnitude > 0.1f)
                {
                    float impactThreshold = 2.6f;
                
                    float impactForce = otherObjectsImpactForce(contact);
                
                    if ( impactForce >= impactThreshold)
                    {
                        killBird();
                    }
                }
            }
        }
    }

    float otherObjectsImpactForce(ContactPoint2D contact)
    {
        Rigidbody2D otherGameObjectRb = contact.rigidbody;
        
        float otherObjectMass = otherGameObjectRb.mass;
        float ownMass = 1;
        
        float totalMass = otherObjectMass+ownMass;
        
        float totalImpactForce = contact.relativeVelocity.magnitude;

        // Normal vector perpendicular to the surface
        Vector2 normal = contact.normal.normalized;
        
        // Direction of relativeVelocity
        Vector2 velocityDirection = contact.relativeVelocity.normalized;
        
        // Calculate the impact force in the direction of the surface normal by scalar multiplication
        float normalImpactForce = (Mathf.Abs(Vector2.Dot(velocityDirection, normal)) * totalImpactForce);
        
        float ownImpactForce = normalImpactForce * (otherObjectMass/totalMass);
        
        return ownImpactForce * 10;
    }
    
    float calculateFallingImpactForce(ContactPoint2D contact)
    {
        float totalImpactForce = contact.relativeVelocity.magnitude;

        // Normal vector perpendicular to the surface
        Vector2 normal = Vector2.up;
        
        // Direction of relativeVelocity
        Vector2 velocityDirection = contact.relativeVelocity.normalized;
        
        // Calculate the impact force in the direction of the surface normal by scalar multiplication
        float normalImpactForce = (Mathf.Abs(Vector2.Dot(velocityDirection, normal)) * totalImpactForce);
        
        return normalImpactForce;
    }
}
