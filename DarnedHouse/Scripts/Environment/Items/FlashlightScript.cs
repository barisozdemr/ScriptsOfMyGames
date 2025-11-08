using System.Collections;
using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    public LayerMask surroundingLayer;
    
    public GameObject spotLight;
    
    public float flashlightGroundCheckDistance = 0.08f;
    
    public int charge = 12000; // 3000 * 4 minutes

    public bool isFlashlightOn;
    
    public bool isInInventory = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        surroundingLayer = LayerMask.GetMask("Default");
        
        foreach (Transform child in transform)
        {
            if (child.name == "Spotlight")
            {
                spotLight = child.gameObject;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (spotLight.activeSelf)
        {
            if (charge > 0)
            {
                charge --;
            }
            else if (charge == 0)
            {
                isFlashlightOn = false;
                spotLight.SetActive(false);
            }
        }
    }

    public void useItem()
    {
        if (spotLight.activeSelf == true)
        {
            isFlashlightOn = false;
            spotLight.SetActive(false);
        }
        else if(charge > 0)
        {
            isFlashlightOn = true;
            spotLight.SetActive(true);
        }
    }
    
    public bool isGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, flashlightGroundCheckDistance, surroundingLayer);
    }
    
    public void heightCorrection()
    {
        RaycastHit hit;
        
        Vector3 position = transform.position;
        
        if(Physics.Raycast(position, Vector3.down, out hit, 5f, surroundingLayer))
        {
            float distance = Vector3.Distance(position, hit.point);
            
            Vector3 pos = transform.position;
            
            pos.y -= distance - flashlightGroundCheckDistance + 0.01f;
            
            transform.position = pos;
        }
    }
    
    public IEnumerator fall()
    {
        float fallingVelocity = 0;
        
        while(true)
        {
            if(isInInventory){
                break;
            }
            
            fallingVelocity += -9.81f * Time.deltaTime;
            
            Vector3 pos = transform.position;
            
            pos.y += fallingVelocity * Time.deltaTime;
            
            transform.position = pos;
            
            if(isGrounded())
            {
                heightCorrection();
                break;
            }
            
            yield return null;
        }
    }
}
