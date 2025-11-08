using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorScript : MonoBehaviour
{
    public bool doorState = false; // false = closed,  -  true = open

    public bool doorMoving = false;

    public GameObject doorHinge;

    public float doorOpeningSpeed = 100;

    public float timer = 1;

    public float openLimit;
    public float closeLimit;
    
    public Coroutine currentCoroutine;

    public bool openColliderEnter = false;
    public bool openColliderStay = false;
    
    public bool closeColliderEnter = false;
    public bool closeColliderStay = false;

    public BoxCollider openCollider;
    public BoxCollider closeCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorHinge = transform.parent.parent.gameObject;
        
        openLimit = doorHinge.transform.rotation.y - 110;
        closeLimit = doorHinge.transform.rotation.y;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }

    //===================================================
    public void openColliderEnterTrigger()
    {
        if (!closeColliderStay)
        {
            openColliderEnter = true;
        }
    }
    
    public void openColliderStayTrigger()
    {
        if (!closeColliderStay)
        {
            openColliderStay = true;
        }
    }
    
    public void openColliderExitTrigger()
    {
        openColliderStay = false;
        openColliderEnter = false;
    }
    
    //===================================================
    
    public void closeColliderEnterTrigger()
    {
        if (!openColliderStay)
        {
            closeColliderEnter = true;
        }
    }
    
    public void closeColliderStayTrigger()
    {
        if (!openColliderStay)
        {
            closeColliderStay = true;
        }
    }
    
    public void closeColliderExitTrigger()
    {
        closeColliderStay = false;
        closeColliderEnter = false;
    }
    //===================================================

    void resetColliders()
    {
        openColliderStay = false;
        openColliderEnter = false;
        closeColliderStay = false;
        closeColliderEnter = false;
    }

    public void openOrClose()
    {
        if (timer >= 0.5f)
        {
            if (doorState == false)
            {
                timer = 0;
                doorState = true;
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                doorMoving = true;
                currentCoroutine = StartCoroutine(open());
            }
            else
            {
                timer = 0;
                doorState = false;
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                doorMoving = true;
                currentCoroutine = StartCoroutine(close());
            }
        }
    }

    void resetTimerDelay()
    {
        Invoke("resetTimer", 0.3f);
    }

    void resetTimer()
    {
        timer = 1;
    }

    public IEnumerator open()
    {
        Quaternion targetRot = Quaternion.Euler(0, openLimit, 0);
        while (true)
        {
            doorHinge.transform.localRotation = Quaternion.RotateTowards(
                doorHinge.transform.localRotation,
                targetRot,
                doorOpeningSpeed * Time.deltaTime
            );

            if (openColliderEnter)
            {
                doorState = false;
                openColliderEnter = false;
                resetTimerDelay();
                StartCoroutine(close());
                break;
            }
            
            if (openColliderStay)
            {
                doorState = false;
                openColliderStay = false;
                resetTimerDelay();
                StartCoroutine(close());
                break;
            }

            if (Quaternion.Angle(doorHinge.transform.localRotation, targetRot) <= 0.1f)
            {
                doorHinge.transform.localRotation = targetRot;
                doorMoving = false;
                resetColliders();
                break;
            }
            else
            {
                yield return null;
            }
        }
    }

    public IEnumerator close()
    {
        Quaternion targetRot = Quaternion.Euler(0, closeLimit, 0);
        while (true)
        {
            doorHinge.transform.localRotation = Quaternion.RotateTowards(
                doorHinge.transform.localRotation,
                targetRot,
                doorOpeningSpeed * Time.deltaTime
            );
            
            if (closeColliderEnter)
            {
                doorState = true;
                closeColliderEnter = false;
                resetTimerDelay();
                StartCoroutine(open());
                break;
            }
            
            if (closeColliderStay)
            {
                doorState = true;
                closeColliderStay = false;
                resetTimerDelay();
                StartCoroutine(open());
                break;
            }

            if (Quaternion.Angle(doorHinge.transform.localRotation, targetRot) <= 0.1f)
            {
                doorHinge.transform.localRotation = targetRot;
                doorMoving = false;
                resetColliders();
                break;
            }
            else
            {
                yield return null;
            }
        }
    }
}
