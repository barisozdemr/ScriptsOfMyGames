using Unity.VisualScripting;
using UnityEngine;

public class OpenColliderScript : MonoBehaviour
{
    public DoorScript doorScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform parentTransform = transform.parent.transform;
        int childCount = parentTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            if (parentTransform.GetChild(i).transform.name == "kapı")
            {
                GameObject kapi = parentTransform.GetChild(i).GetChild(0).gameObject;
                doorScript = kapi.GetComponent<DoorScript>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        doorScript.openColliderEnterTrigger();
    }
    
    void OnTriggerStay(Collider col)
    {
        doorScript.openColliderStayTrigger();
    }

    void OnTriggerExit(Collider col)
    {
        doorScript.openColliderExitTrigger();
    }
}
