using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Stage1LogicScript : MonoBehaviour
{
    public string LEVELDATAPATH = "levelData.txt";
    
    public UIManagerScript uiManagerScript;
    
    public SlingScript slingScript;

    public Text goodBirdCountText;
    public Text badBirdCountText;

    public Text levelCountText;
    public string levelCount;
    
    public bool level1AlreadyPassed = false;
    public bool level2AlreadyPassed = false;
    public bool level3AlreadyPassed = false;
    public bool level4AlreadyPassed = false;
    
    public GameObject[] badBirds;
    
    public GameObject birdToBeThrown;
    
    public GameObject[] goodBirds;
    public int goodBirdsLeftCount;
    public bool[] goodBirdsAreInPositions;
    
    public Vector3 birdToBeThrownPos = new Vector3(-4.277f, -1.46f, 0f);
    public Vector3 goodBirdPos1 = new Vector3(-5.384f, -3.335f, 0f);
    public Vector3 goodBirdPos2 = new Vector3(-6.174f, -3.335f, 0f);
    public Vector3 goodBirdPos3 = new Vector3(-6.964f, -3.335f, 0f);
    public Vector3 goodBirdPos4 = new Vector3(-7.754f, -3.335f, 0f);
    public Vector3 goodBirdPos5 = new Vector3(-8.543f, -3.335f, 0f);
    public Vector3 goodBirdPos6 = new Vector3(-9.332f, -3.335f, 0f);
    
    public Vector3[] goodBirdPositions = new Vector3[7];
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManagerScript = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManagerScript>();
        
        slingScript = GameObject.FindGameObjectWithTag("SlingManager").GetComponent<SlingScript>();

        if (File.Exists(LEVELDATAPATH))
        {
            checkAlreadyPassedLevels();
        }
        
        levelCount = levelCountText.text;
        
        goodBirds = GameObject.FindGameObjectsWithTag("goodBird");
        if (goodBirds.Length > 6)
        {
            goodBirdsAreInPositions = new bool[6];
        }
        else
        {
            goodBirdsAreInPositions = new bool[goodBirds.Length];
        }
        
        goodBirdsLeftCount = goodBirds.Length;

        for (int i = 0; i < goodBirdsAreInPositions.Length; i++)
        {
            goodBirdsAreInPositions[i] = false;
        }

        goodBirdPositions[0] = birdToBeThrownPos;
        goodBirdPositions[1] = goodBirdPos1;
        goodBirdPositions[2] = goodBirdPos2;
        goodBirdPositions[3] = goodBirdPos3;
        goodBirdPositions[4] = goodBirdPos4;
        goodBirdPositions[5] = goodBirdPos5;
        goodBirdPositions[6] = goodBirdPos6;
        
        badBirds = GameObject.FindGameObjectsWithTag("badBird");
        
        updateBadBirdCount();
        updateGoodBirdCount();
        
        getBirdsToPositions();

        StartCoroutine(startDelay()); //nextTurn
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void levelPassed()
    {
        updateLevelDatas();
    }

    void checkAlreadyPassedLevels()
    {
        if (level4AlreadyPassed != true)
        {
            string[] lines = File.ReadAllLines(LEVELDATAPATH);

            if (lines.Length >= 2)
            {
                level4AlreadyPassed = true;
            }

            string[] data = lines[0].Split(';');

            string[] levels = data[1].Split(',');

            foreach (string level in levels)
            {
                if (level == "2")
                {
                    level1AlreadyPassed = true;
                }

                if (level == "3")
                {
                    level2AlreadyPassed = true;
                }

                if (level == "4")
                {
                    level3AlreadyPassed = true;
                }
            }
        }
    }

    void updateLevelDatas()
    {
        if (File.Exists(LEVELDATAPATH))
        {
            if (levelCount == "1" && !level1AlreadyPassed)
            {
                string level = (int.Parse(levelCount) + 1).ToString();
                
                File.AppendAllText(LEVELDATAPATH, level);
            }
            else if (levelCount == "2" && !level2AlreadyPassed)
            {
                string level = (int.Parse(levelCount) + 1).ToString();
                
                File.AppendAllText(LEVELDATAPATH, ","+level);
            }
            else if (levelCount == "3" && !level3AlreadyPassed)
            {
                string level = (int.Parse(levelCount) + 1).ToString();
                
                File.AppendAllText(LEVELDATAPATH, ","+level);
            }
            else if (levelCount == "4" && !level4AlreadyPassed)
            {
                File.AppendAllText(LEVELDATAPATH, Environment.NewLine + "2;1");
            }
        }
    }

    public void updateGoodBirdCount()
    {
        goodBirdCountText.text = goodBirdsLeftCount.ToString();
    }

    public void updateBadBirdCount()
    {
        int count = 0;
        foreach (GameObject badBird in badBirds)
        {
            BadBirdScript badBirdScript = badBird.GetComponent<BadBirdScript>();
            if (!badBirdScript.isDead)
            {
                count++;
            }
        }
        badBirdCountText.text = count.ToString();
    }
    
    public void shooted() //start checking badBirds for impacts
    {
        foreach (GameObject badBird in badBirds)
        {
            BadBirdScript badBirdScript = badBird.GetComponent<BadBirdScript>();
            badBirdScript.updateShootingBird(birdToBeThrown);
        }
    }

    bool checkIfGameIsOver()
    {
        bool g = true;
        foreach (GameObject badBird in badBirds)
        {
            BadBirdScript badBirdScript = badBird.GetComponent<BadBirdScript>(); 
            if (!badBirdScript.isDead)
            {
                g = false;
            }
        }

        if (g) //--- win
        {
            levelPassed();
            StartCoroutine(uiManagerScript.showLevelPassedScreen());
            return true;
        }
        
        if (goodBirdsLeftCount == 0) //--- lost
        {
            StartCoroutine(uiManagerScript.showLevelFailedScreen());
            return true;
        }
        
        return false;
        
    }

    void nextTurn()
    {
        if (!checkIfGameIsOver())
        {
            birdToBeThrown.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            StartCoroutine(getSlingReadyToShoot());
        }
    }

    void getBirdsToPositions()
    {
        for (int i=0 ; i<goodBirds.Length ; i++)
        {
            if (i >= 6)
            {
                goodBirds[i].transform.position = goodBirdPos6;
            }
            else
            {
                goodBirds[i].transform.position = goodBirdPositions[i+1];
            }
        }
    }

    public IEnumerator endOfTurnControl()
    {
        yield return new WaitForSeconds(3f);
        
        float threshold = 0.3f;
        int controlCount = 0;

        while (true)
        {
            bool g = true;
            
            foreach (GameObject badBird in badBirds)
            {
                if (badBird.GetComponent<BadBirdScript>().isDead) continue;
                
                Rigidbody2D rb = badBird.GetComponent<Rigidbody2D>();

                if (rb.linearVelocity.magnitude >= threshold && Mathf.Abs(rb.angularVelocity) >= threshold)
                {
                    g = false;
                }
            }
            
            if (g)
            {
                controlCount++;
                
                if (controlCount >= 8)
                {
                    updateBadBirdCount();
                    updateGoodBirdCount();
                    nextTurn();
                    break;
                }
            }
            else
            {
                controlCount = 0;
            }
        
            yield return new WaitForSeconds(0.25f);
        }
    }

    public IEnumerator getSlingReadyToShoot()
    {
        while (true)
        {
            float velocityFactor = 5f * Time.deltaTime;
            
            for (int i=0 ; i<goodBirdsLeftCount ; i++)
            {
                CircleCollider2D birdCollider = goodBirds[i].GetComponent<CircleCollider2D>();
                birdCollider.enabled = true;
                
                if (i == 0)
                {
                    if ( ! birdCollider.bounds.Contains(birdToBeThrownPos))
                    {
                        goodBirds[i].transform.position += //move bird
                            (birdToBeThrownPos - goodBirds[i].transform.position).normalized * velocityFactor;
                    }
                    else
                    {
                        goodBirds[i].transform.position = birdToBeThrownPos;
                        goodBirdsAreInPositions[i] = true;
                    }
                }
                else if(i <= 5)
                {
                    if ( ! birdCollider.bounds.Contains(goodBirdPositions[i]))
                    {
                        goodBirds[i].transform.position += new Vector3(1, 0, 0) * velocityFactor; //move bird
                    }
                    else
                    {
                        goodBirds[i].transform.position = goodBirdPositions[i];
                        goodBirdsAreInPositions[i] = true;
                    }
                }
                
                birdCollider.enabled = false;
            }

            bool g = true;
            for (int i = 0; i < goodBirds.Length; i++)
            {
                if (i < 6)
                {
                    if (goodBirdsAreInPositions[i] == false)
                    {
                        g = false;
                    }
                }
            }

            if (g)
            {
                birdToBeThrown = goodBirds[0];
                
                for (int i = 0; i < goodBirds.Length-1; i++)
                {
                    goodBirds[i] = goodBirds[i+1];
                }

                goodBirdsLeftCount --;

                for (int i = 0; i < goodBirdsLeftCount; i++)
                {
                    if (i < 6)
                    {
                        goodBirdsAreInPositions[i] = false;
                    }
                }
                slingScript.updateBird(birdToBeThrown);
                
                yield break;
            }
            
            yield return null;
        }
    }

    public IEnumerator startDelay()
    {
        yield return new WaitForSeconds(3f);
        
        StartCoroutine(getSlingReadyToShoot());
    }
}