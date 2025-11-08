using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[DefaultExecutionOrder(0)]
public class MouseMovementScript : MonoBehaviour {
    
    public InventoryManagerScript inventoryManagerScript;
    
    public Camera cam;

    public LayerMask interactiveLayerMask = new LayerMask();
    
    public GameObject lastHitObject = null;
    
    public Transform bodyTransform;
    public Transform cameraTransform;
    public Transform cameraHolderTransform;

    public Transform flashlightHoldingRigTargets;
    public Transform interactionRigTargets;
    
    public float mouseSensitivity = 80f;
    
    public float reachDistance = 1.5f;
    
    private float xRotation = 0f;
    private float yRotation = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        inventoryManagerScript = GetComponent<InventoryManagerScript>();
        
        interactiveLayerMask = LayerMask.GetMask("InteractiveDoor", "InteractiveItem", "OutlineDoor", "OutlineItem");
    }

    // Update is called once per frame
    void Update()
    {
        //================================================================================== Camera Movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 80f);
    
        yRotation += mouseX;
    
        bodyTransform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        cameraHolderTransform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        //================================================================================== Raycast
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, reachDistance, interactiveLayerMask)) // hit
        {
            if (hit.collider.gameObject != lastHitObject)
            {
                if (lastHitObject != null)
                {
                    setLayerToOutline();
                }
                
                lastHitObject = hit.collider.gameObject;
                
                setLayerToInteractive();
            }
        }
        else // no hit
        {
            if (lastHitObject != null)
            {
                setLayerToOutline();
            }
        }
        
        if(lastHitObject != null)
        {
            if (Input.GetKey(KeyCode.E)) //=========================== E pressed
            {
                if (lastHitObject.transform.name != "kapı")
                {
                    inventoryManagerScript.ePressed(lastHitObject);
                }
                else
                {
                    DoorScript doorScript = lastHitObject.GetComponent<DoorScript>();
                    doorScript.openOrClose();
                    StartCoroutine(inventoryManagerScript.interactionRigPlay());
                }
            }
        }
    }

    void setLayerToInteractive()
    {
        if (lastHitObject.transform.name == "kapı")
        {
            lastHitObject.layer = LayerMask.NameToLayer("InteractiveDoor");
        }
        else if(lastHitObject.transform.name == "fener_mesh")
        {
            setLayerRecursively(LayerMask.NameToLayer("InteractiveItem"), lastHitObject);
        }
        else
        {
            lastHitObject.layer = LayerMask.NameToLayer("InteractiveItem");
        }
    }

    void setLayerToOutline()
    {
        if (lastHitObject.transform.name == "fener_mesh")
        {
            setLayerRecursively(LayerMask.NameToLayer("OutlineItem"), lastHitObject);
            lastHitObject = null;
        }
        else if (lastHitObject.transform.name == "kapı")
        {
            lastHitObject.layer = LayerMask.NameToLayer("OutlineDoor");
            lastHitObject = null;
        }
        else
        {
            lastHitObject.layer = LayerMask.NameToLayer("OutlineItem");
            lastHitObject = null;
        }
    }

    void LateUpdate()
    {
        flashlightHoldingRigTargets.localRotation = Quaternion.Euler(xRotation/3f, 0f, 0f);// flashlight direction
        interactionRigTargets.localRotation = Quaternion.Euler(xRotation/3f, 0f, 0f);// right arm direction
    }

    public void setLayerRecursively(LayerMask layer, GameObject obj)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = layer;
            setLayerRecursively(layer, child.gameObject);
        }
    }
}
