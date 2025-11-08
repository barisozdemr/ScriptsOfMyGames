using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SlingScript : MonoBehaviour
{
    public AudioManagerScript audioManagerScript;

    public bool rubberStrechSoundAlreadyPlayed = false;
    
    public Stage1LogicScript logicScript1;
    public Stage2LogicScript logicScript2;
    public Text stageCountText;
    public int stageCount;
    
    public Camera cam;
    public bool isSlingUpdated = false;
    public bool isShooted = true;
    public float velocityFactor;

    public Vector3 distance;
    public float RADIUS = 0.35f;

    public GameObject bird;
    public GameObject chair;

    public Vector3 birdStartPosition;
    public Vector3 chairStartPosition;
    
    public Vector3 birdStartPositionStage1 = new Vector3(-4.282f, -1.46f, 0f);
    public Vector3 chairStartPositionStage1 = new Vector3(-4.363f, -1.555f, -0.1f);
    public Vector3 birdStartPositionStage2 = new Vector3(-4.867f, -1.756f, 0f);
    public Vector3 chairStartPositionStage2 = new Vector3(-4.925f, -1.823f, -0.1f);

    public GameObject startRubbers;
    public GameObject frontRubber;
    public GameObject rearRubber;

    public Texture2D rubberTexture;
    public Sprite rubberSprite;

    public Vector3 frontRubberSlingHinge;
    public Vector3 rearRubberSlingHinge;
    
    public Vector3 frontRubberSlingHingeStage1 = new Vector3(-4.68f, -1.42f, -0.2f);
    public Vector3 rearRubberSlingHingeStage1 = new Vector3(-4.1f, -1.4f, 0.1f);
    public Vector3 frontRubberSlingHingeStage2 = new Vector3(-5.16f, -1.66f, -0.2f);
    public Vector3 rearRubberSlingHingeStage2 = new Vector3(-4.71f, -1.67f, 0.1f);

    public float mouseLimitUpX;
    public float mouseLimitDownX;
    public float mouseLimitUpY;
    public float mouseLimitDownY;
    
    public float mouseLimitUpXStage1 = -4.3f;
    public float mouseLimitDownXStage1 = -8.65f;
    public float mouseLimitUpYStage1 = 1f;
    public float mouseLimitDownYStage1 = -3.7f;
    
    public float mouseLimitUpXStage2 = -4.84f;
    public float mouseLimitDownXStage2 = -8.7f;
    public float mouseLimitUpYStage2 = 1f;
    public float mouseLimitDownYStage2 = -4f;

    public GameObject aimTrace;
    public Texture2D aimTraceDotsTexture;
    public Sprite aimTraceDotsSprite;
    public int stagePointCount;
    public float gravity = 9.81f;

    public Button aimAssistButton;
    public Text aimAssistText;
    public int aimAssistCount;
    public bool aimAssistActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManagerScript = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManagerScript>();
        
        stageCount = int.Parse(stageCountText.text);
        if (stageCount == 1)
        {
            logicScript1 = GameObject.FindGameObjectWithTag("LogicManager").GetComponent<Stage1LogicScript>();
            birdStartPosition = birdStartPositionStage1;
            chairStartPosition = chairStartPositionStage1;
            frontRubberSlingHinge = frontRubberSlingHingeStage1;
            rearRubberSlingHinge = rearRubberSlingHingeStage1;
            stagePointCount = 6;
            velocityFactor = 4.5f;
            
            mouseLimitUpX = mouseLimitUpXStage1;
            mouseLimitDownX = mouseLimitDownXStage1;
            mouseLimitUpY = mouseLimitUpYStage1;
            mouseLimitDownY = mouseLimitDownYStage1;
        }
        else if (stageCount == 2)
        {
            logicScript2 = GameObject.FindGameObjectWithTag("LogicManager").GetComponent<Stage2LogicScript>();
            birdStartPosition = birdStartPositionStage2;
            chairStartPosition = chairStartPositionStage2;
            frontRubberSlingHinge = frontRubberSlingHingeStage2;
            rearRubberSlingHinge = rearRubberSlingHingeStage2;
            stagePointCount = 7;
            velocityFactor = 5f;
            
            mouseLimitUpX = mouseLimitUpXStage2;
            mouseLimitDownX = mouseLimitDownXStage2;
            mouseLimitUpY = mouseLimitUpYStage2;
            mouseLimitDownY = mouseLimitDownYStage2;
        }
        
        rubberSprite = Sprite.Create(
            rubberTexture,
            new Rect(0, 0, rubberTexture.width, rubberTexture.height),
            new Vector2(0.5f, 0.5f)
        );

        aimTraceDotsSprite = Sprite.Create(
            aimTraceDotsTexture,
            new Rect(0, 0, aimTraceDotsTexture.width, aimTraceDotsTexture.height),
            new Vector2(0.5f, 0.5f)
        );
        
        cam = Camera.main;
        
        aimAssistCount = int.Parse(aimAssistText.text);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseUp()
    {
        if (isSlingUpdated)
        {
            shoot();
        }
    }

    void OnMouseDown()
    {

    }

    void OnMouseDrag()
    {
        if (!isShooted)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(cam.transform.position.z - transform.position.z))
            );
        
            if (mousePos.x > mouseLimitUpX)
            {
                mousePos.x = mouseLimitUpX;
            }
            else if (mousePos.x < mouseLimitDownX)
            {
                mousePos.x = mouseLimitDownX;
            }
        
            if (mousePos.y > mouseLimitUpY)
            {
                mousePos.y = mouseLimitUpY;
            }
            else if (mousePos.y < mouseLimitDownY)
            {
                mousePos.y = mouseLimitDownY;
            }
        
            distance = transform.position - mousePos;

            if (distance.magnitude > RADIUS)
            {
                if (!rubberStrechSoundAlreadyPlayed)
                {
                    audioManagerScript.playRubberStrechingSound();
                    rubberStrechSoundAlreadyPlayed = true;
                }
                updateBirdAndChair(mousePos);
                updateRubber();
                showAimTrace(mousePos);
            }
            else if (isSlingUpdated)
            {
                resetSling();
                isSlingUpdated = false;
                rubberStrechSoundAlreadyPlayed = false;
            }
        }
    }

    void updateBirdAndChair(Vector3 mousePos)
    {
        if (bird.transform.localScale.ToString() == "(0.55, 0.55, 1.00)") // check if its goodBird2
        {
            bird.transform.position = new Vector3(mousePos.x+0.06f, mousePos.y+0.06f, -0.1f);
        }
        else
        {
            bird.transform.position = new Vector3(mousePos.x, mousePos.y, -0.1f);
        }
        
        chair.transform.position = new Vector3(mousePos.x, mousePos.y, -0.2f);
    }

    void updateRubber()
    {
        startRubbers.SetActive(false);

        Vector3 chairPos = chair.transform.position;

        Vector3 frontRubberTip = chairPos;
        frontRubberTip.z = -0.2f;
        Vector3 frontRubberPos = (frontRubberTip + frontRubberSlingHinge) / 2;
        float frontRubberStrechDistance = Vector3.Distance(frontRubberTip, frontRubberSlingHinge);

        Vector3 rearRubberTip = chairPos;
        rearRubberTip.z = 0.1f;
        Vector3 rearRubberPos = (rearRubberTip + rearRubberSlingHinge) / 2;
        float rearRubberStrechDistance = Vector3.Distance(rearRubberTip, rearRubberSlingHinge);

        if (frontRubber == null)
        {
            frontRubber = new GameObject("FrontRubber");
            SpriteRenderer rubberRenderer = frontRubber.AddComponent<SpriteRenderer>();
            rubberRenderer.sprite = rubberSprite;
        }

        if (rearRubber == null)
        {
            rearRubber = new GameObject("RearRubber");
            SpriteRenderer rubberRenderer = rearRubber.AddComponent<SpriteRenderer>();
            rubberRenderer.sprite = rubberSprite;
        }

        frontRubber.transform.position = frontRubberPos;
        frontRubber.transform.right = (frontRubberSlingHinge - frontRubberTip).normalized;
        frontRubber.transform.localScale = new Vector3(frontRubberStrechDistance * 100f / 15f, 1, 1);

        rearRubber.transform.position = rearRubberPos;
        rearRubber.transform.right = ((rearRubberSlingHinge - rearRubberTip).normalized);
        rearRubber.transform.localScale = new Vector3(rearRubberStrechDistance * 100f / 15f, 1, 1);

        isSlingUpdated = true;
    }

    void resetSling()
    {
        destroyRubbers();
        destroyAimTrace();

        bird.transform.position = birdStartPosition;
        chair.transform.position = chairStartPosition;
    }

    void destroyRubbers()
    {
        startRubbers.SetActive(true);
        
        if (frontRubber != null)
        {
            Destroy(frontRubber);
        }

        if (rearRubber != null)
        {
            Destroy(rearRubber);
        }
    }

    void shoot()
    {
        audioManagerScript.playFlybyWhooshSound();
        
        bird.GetComponent<GoodBirdScript>().makeGroundImpactSoundAvailableDelay();

        rubberStrechSoundAlreadyPlayed = false;
        
        isSlingUpdated = false;
        isShooted = true;

        aimAssistActive = false;
        aimAssistButton.image.color = Color.white;
        
        destroyAimTrace();

        Vector3 chairPos = chair.transform.position;
        chair.transform.position = new Vector3(chairPos.x, chairPos.y, -0.1f);
        Vector3 birdPos = chair.transform.position;
        bird.transform.position = new Vector3(birdPos.x, birdPos.y, 0f);
        
        float velocityX = distance.x * velocityFactor;
        float velocityY = distance.y * velocityFactor*(1.2f);

        Rigidbody2D rbBird = bird.GetComponent<Rigidbody2D>();
        rbBird.simulated = true;
        rbBird.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rbBird.linearVelocity = new Vector2(velocityX, velocityY);

        Rigidbody2D rbChair = chair.GetComponent<Rigidbody2D>();
        rbChair.simulated = true;
        rbChair.linearVelocity = new Vector2(velocityX, velocityY);
        
        StartCoroutine(simulateChairAfterShoot());
        if (stageCount == 1)
        {
            logicScript1.shooted();
        }
        else if (stageCount == 2)
        {
            logicScript2.shooted();
        }
    }

    void showAimTrace(Vector3 mousePos)
    {
        destroyAimTrace();

        float velocityX = distance.x * velocityFactor;
        float velocityY = distance.y * velocityFactor*(1.2f);

        float timeDelay = 0.04f;

        int pointCount;

        if (aimAssistActive)
        {
            pointCount = 65;
        }
        else
        {
            pointCount = stagePointCount;
        }

        for (int i = 0; i < pointCount; i++)
        {
            float time = timeDelay * (i + 4);

            float x = mousePos.x + (velocityX * time); // x = x0 + vt
            float y = mousePos.y + (velocityY * time) - (0.5f * gravity * time * time); // y = y0 + vt + 1/2at^2

            Vector3 point = new Vector3(x, y, 0);

            GameObject dot = new GameObject("dot");
            if (stageCount > 1)
            {
                dot.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
            dot.transform.position = point;
            SpriteRenderer dotRenderer = dot.AddComponent<SpriteRenderer>();
            dotRenderer.sprite = aimTraceDotsSprite;
            dot.transform.SetParent(aimTrace.transform);
        }
    }

    void destroyAimTrace()
    {
        foreach (Transform child in aimTrace.transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    public void useAimAssist()
    {
        audioManagerScript.playDefaultButtonSound();
        
        if (!aimAssistActive)
        {
            if (aimAssistCount > 0)
            {
                aimAssistActive = true;
                aimAssistCount--;
                aimAssistText.text = aimAssistCount.ToString();
                aimAssistButton.image.color = new Color32(0x5F, 0x71, 0x7F, 0xFF);
            }
        }
        else
        {
            aimAssistActive = false;
            aimAssistCount++;
            aimAssistText.text = aimAssistCount.ToString();
            aimAssistButton.image.color = Color.white;
        }
    }

    public void updateBird(GameObject newBird)
    {
        bird = newBird;
        
        Collider2D[] collider2D = bird.GetComponents<Collider2D>();
        foreach (Collider2D collider in collider2D)
        {
            collider.enabled = true;
        }
        
        isShooted = false;
    }

    IEnumerator simulateChairAfterShoot()
    {
        float coroutineTimer = 0;
        
        Rigidbody2D rbChair = chair.GetComponent<Rigidbody2D>();

        CircleCollider2D circleCollider = this.GetComponent<CircleCollider2D>();
        
        while (true)
        {
            coroutineTimer += Time.deltaTime;
            
            Vector2 forceDirection = (transform.position - chair.transform.position).normalized;
            float rubberStrech = (transform.position - chair.transform.position).magnitude + 1f;
            float forceFactor = rubberStrech * 40f * Time.deltaTime;

            if (coroutineTimer >= 0.2f)
            {
                rbChair.AddForce(forceDirection * forceFactor, ForceMode2D.Impulse);
            }
            
            Vector3 chairPos = chair.transform.position;

            Vector3 frontRubberTip = chairPos;
            frontRubberTip.z = -0.2f;
            Vector3 frontRubberPos = (frontRubberTip + frontRubberSlingHinge) / 2;
            float frontRubberStrechDistance = Vector3.Distance(frontRubberTip, frontRubberSlingHinge);

            Vector3 rearRubberTip = chairPos;
            rearRubberTip.z = 0.1f;
            Vector3 rearRubberPos = (rearRubberTip + rearRubberSlingHinge) / 2;
            float rearRubberStrechDistance = Vector3.Distance(rearRubberTip, rearRubberSlingHinge);
            
            frontRubber.transform.position = frontRubberPos;
            frontRubber.transform.right = (frontRubberSlingHinge - frontRubberTip).normalized;
            frontRubber.transform.localScale = new Vector3(frontRubberStrechDistance * 100f / 15f, 1, 1);
            
            rearRubber.transform.position = rearRubberPos;
            rearRubber.transform.right = (rearRubberSlingHinge - rearRubberTip).normalized;
            rearRubber.transform.localScale = new Vector3(rearRubberStrechDistance * 100f / 15f, 1, 1);

            if (coroutineTimer >= 1f)
            {
                if (circleCollider.IsTouching(chair.GetComponent<CircleCollider2D>()))
                {
                    rbChair.simulated = false;
                    chair.transform.position = chairStartPosition;
                    destroyRubbers();
                    if (stageCount == 1)
                    {
                        StartCoroutine(logicScript1.endOfTurnControl());
                    }
                    else if (stageCount == 2)
                    {
                        StartCoroutine(logicScript2.endOfTurnControl());
                    }
                    yield break;
                }
            }

            yield return null;
        }
    }
}