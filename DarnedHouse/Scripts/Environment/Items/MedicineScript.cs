using System.Collections;
using UnityEngine;

public class MedicineScript : MonoBehaviour
{
    public LayerMask surroundingLayer;
    
    public float medicineGroundCheckDistance = 0.08f;
    
    public bool isInInventory = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        surroundingLayer = LayerMask.GetMask("Default");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void useItem()
    {
        
    }
    
    public bool isGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, medicineGroundCheckDistance, surroundingLayer);
    }
    
    public void heightCorrection()
    {
        RaycastHit hit;
        
        Vector3 position = transform.position;
        
        if(Physics.Raycast(position, Vector3.down, out hit, 5f, surroundingLayer))
        {
            float distance = Vector3.Distance(position, hit.point);
            
            Vector3 pos = transform.position;
            
            pos.y -= distance - medicineGroundCheckDistance + 0.01f;
            
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
