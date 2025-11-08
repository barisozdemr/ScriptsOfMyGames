using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class InventoryManagerScript : MonoBehaviour {
    
    public GameObject[] inventory = new GameObject[3];
    
    public byte selectedSlot = 0; // 0, 1, 2
    
    //==================================================== UI
    public Image inventorySlots;

    public Sprite inventorySlot1;
    public Sprite inventorySlot2;
    public Sprite inventorySlot3;
    
    public Image inventorySlot1Image;
    public Image inventorySlot2Image;
    public Image inventorySlot3Image;

    public Sprite flashlightPicture;
    public Sprite medicinePicture;
    public Sprite batteryPicture;
    
    public bool haveFlashlight = false;
    public int flashlightSlot = 0;
    
    public Vector2 slot1FlashlightBarPosition = new Vector2(-63, -506.5f);
    public Vector2 slot2FlashlightBarPosition = new Vector2(0, -506.5f);
    public Vector2 slot3FlashlightBarPosition = new Vector2(63, -506.5f);

    public RectTransform flashlightChargeBar;
    public RectTransform flashlightChargeBarInside;
    public float flashlightChargeUpdateTimer;
    
    //===================================================== Animation
    public Camera cam;
    
    public Transform itemObjectHolder;
    public Transform flashlightObjectHolder;
    
    public bool isHoldingItem = false;
    
    public Rig itemHoldingRig;
    
    public Rig flashlightHoldingRig;

    //=====================================
    public Rig interactionRig;

    public MultiAimConstraint intArm;
    public MultiAimConstraint intForearm_second;
    public MultiAimConstraint intForearm2;
    public MultiAimConstraint intHand;
    //=====================================
    
    public Coroutine holdingRigWeightSetCoroutine;

    void Start()
    {
        inventorySlot1Image.enabled = false;
        inventorySlot2Image.enabled = false;
        inventorySlot3Image.enabled = false;
    }

    void FixedUpdate()
    {
        flashlightChargeUpdateTimer += 0.02f;
        
        if (haveFlashlight && flashlightChargeUpdateTimer >= 5)
        {
            updateFlashlightChargeBar();
        }
    }

    public void updateFlashlightChargeBar()
    {
        flashlightChargeUpdateTimer = 0;
        float ratio = (inventory[flashlightSlot].transform.parent.gameObject.GetComponent<FlashlightScript>().charge) / (float)12000;
            
        flashlightChargeBarInside.localScale = new Vector3(ratio, 1, 1);

        Vector2 pos = flashlightChargeBarInside.anchoredPosition;
        pos.x = (ratio - 1) * 22;
        flashlightChargeBarInside.anchoredPosition = pos;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            fPressed();
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            gPressed();
        }
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if(scroll < 0f){
            if(selectedSlot == 2){
                if (inventory[2] != null)
                {
                    hideObject(inventory[2]);
                }
                selectedSlot = 0;
                holdItem();
            }
            else{
                if (inventory[selectedSlot] != null)
                {
                    hideObject(inventory[selectedSlot]);
                }
                selectedSlot++;
                holdItem();
            }
        }
        else if(scroll > 0f){
            if(selectedSlot == 0){
                if (inventory[0] != null)
                {
                    hideObject(inventory[0]);
                }
                selectedSlot = 2;
                holdItem();
            }
            else{
                if (inventory[selectedSlot] != null)
                {
                    hideObject(inventory[selectedSlot]);
                }
                
                selectedSlot--;
                holdItem();
            }
        }

        if (selectedSlot == 0)
        {
            inventorySlots.sprite = inventorySlot1;
        }
        else if (selectedSlot == 1)
        {
            inventorySlots.sprite = inventorySlot2;
        }
        else if (selectedSlot == 2)
        {
            inventorySlots.sprite = inventorySlot3;
        }
    }
    
    public void ePressed(GameObject obj) //=== interact
    {
        if(inventory[selectedSlot] == null)
        {
            inventory[selectedSlot] = obj;

            if (obj.transform.parent.name == "flashlight")
            {
                haveFlashlight = true;
                flashlightSlot = selectedSlot;
                setFlashlightBar(selectedSlot);
            }
            
            hideObject(obj);
            setParent(obj);
            markInInventory(obj);
            StartCoroutine(interactionRigPlay());
            
            updateInventoryPictures(selectedSlot);
        }
        else{
            if(inventory[0] == null){
                
                inventory[0] = obj;
                
                if (obj.transform.parent.name == "flashlight")
                {
                    haveFlashlight = true;
                    flashlightSlot = 0;
                    setFlashlightBar(0);
                }
                
                hideObject(obj);
                setParent(obj);
                markInInventory(obj);
                StartCoroutine(interactionRigPlay());

                updateInventoryPictures(0);
            }
            else if(inventory[1] == null){
                
                inventory[1] = obj;
                
                if (obj.transform.parent.name == "flashlight")
                {
                    haveFlashlight = true;
                    flashlightSlot = 1;
                    setFlashlightBar(1);
                }
                
                hideObject(obj);
                setParent(obj);
                markInInventory(obj);
                StartCoroutine(interactionRigPlay());
                
                updateInventoryPictures(1);
            }
            else if(inventory[2] == null){
                
                inventory[2] = obj;
                
                if (obj.transform.parent.name == "flashlight")
                {
                    haveFlashlight = true;
                    flashlightSlot = 2;
                    setFlashlightBar(2);
                }
                
                hideObject(obj);
                setParent(obj);
                markInInventory(obj);
                StartCoroutine(interactionRigPlay());
                
                updateInventoryPictures(2);
            }
        }
        
        if(!isHoldingItem)
        {
            holdItem();
        }
    }

    public void fPressed() //=== use item
    {
        if (inventory[selectedSlot] != null)
        {
            if (inventory[selectedSlot].transform.parent.name == "flashlight")
            {
                FlashlightScript script = inventory[selectedSlot].transform.parent.GetComponent<FlashlightScript>();
                script.useItem();
            }
            else if (inventory[selectedSlot].transform.parent.name == "medicine")
            {
                MedicineScript script = inventory[selectedSlot].transform.parent.GetComponent<MedicineScript>();
                script.useItem();
                
                updateInventoryPictures(selectedSlot);
            }
            else if (inventory[selectedSlot].transform.parent.name == "battery")
            {
                for (int i = 0 ; i < inventory.Length ; i++)
                {
                    if (inventory[i] != null)
                    {
                        if (inventory[i].transform.parent.name == "flashlight")
                        {
                            FlashlightScript flashlight = inventory[i].transform.parent.GetComponent<FlashlightScript>();
                    
                            BatteryScript battery = inventory[selectedSlot].transform.parent.GetComponent<BatteryScript>();
                            battery.useItem(flashlight);
                            
                            updateFlashlightChargeBar();

                            inventory[selectedSlot] = null;
                            holdItem();
                            
                            updateInventoryPictures(selectedSlot);
                        }
                    }
                }
            }
        }
    }

    public void gPressed() //=== drop item
    {
        if (inventory[selectedSlot] != null)
        {
            Transform cameraTransform = cam.transform;
            
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;
            
            Vector3 targetPos;

            if (Physics.Raycast(ray, out hit, 0.5f))
            {
                targetPos = hit.point;
                targetPos -= cameraTransform.forward * 0.05f;
            }
            else
            {
                targetPos = cameraTransform.position + cameraTransform.forward * 0.5f;
            }

            targetPos.y += 0.04f;

            Transform parentTransform = inventory[selectedSlot].transform.parent;
            
            if (parentTransform.name == "flashlight")
            {
                haveFlashlight = false;
                hideFlashlightBar();
                camForwardToFlashlightRotation(parentTransform);
            }
            else
            {
                parentTransform.rotation = Quaternion.Euler(0,0,0);
            }
            
            parentTransform.position = targetPos;
            
            clearParent(inventory[selectedSlot]);
            markNotInInventory(inventory[selectedSlot]);
            makeItemFall(inventory[selectedSlot]);
            inventory[selectedSlot] = null;
            holdItem();

            updateInventoryPictures(selectedSlot);
        }
    }

    public void makeItemFall(GameObject obj)
    {
        if (obj.transform.parent.name == "flashlight")
        {
            FlashlightScript script = obj.transform.parent.GetComponent<FlashlightScript>();
            StartCoroutine(script.fall());
        }
        else if (obj.transform.parent.name == "medicine")
        {
            MedicineScript script = obj.transform.parent.GetComponent<MedicineScript>();
            StartCoroutine(script.fall());
        }
        else if (obj.transform.parent.name == "battery")
        {
            BatteryScript script = obj.transform.parent.GetComponent<BatteryScript>();
            StartCoroutine(script.fall());
        }
    }

    public void holdItem()
    {
        if (inventory[selectedSlot] != null)
        {
            //============================================================== Rig Set
            Rig rig = null;
            if (inventory[selectedSlot].transform.parent.name == "flashlight")
            {
                rig = flashlightHoldingRig;
            }
            else
            {
                rig = itemHoldingRig;
            }
            
            //======================================================================
            if (isHoldingItem)
            {
                Rig rig2 = null;
                if (itemHoldingRig.weight != 0)
                {
                    rig2 = itemHoldingRig;
                }
                else if (flashlightHoldingRig.weight != 0)
                {
                    rig2 = flashlightHoldingRig;
                }
                holdingRigResetCall(rig2, rig);
            }
            else
            {
                holdingRigSetOneCall(rig);
            }
            isHoldingItem = true;
            showObject(inventory[selectedSlot]);
        }
        else
        {
            if (isHoldingItem)
            {
                //============================================================== Rig Set
                Rig rig = null;
                if(itemHoldingRig.weight != 0)
                {
                    rig = itemHoldingRig;
                }
                else if(flashlightHoldingRig.weight != 0)
                {
                    rig = flashlightHoldingRig;
                }
            
                //======================================================================
                holdingRigSetZeroCall(rig);
                isHoldingItem = false;
            }
        }
    }

    public void hideObject(GameObject obj)
    {
        obj.transform.parent.gameObject.SetActive(false);
    }
    
    public void showObject(GameObject obj)
    {
        obj.transform.parent.gameObject.SetActive(true);
    }

    public void setParent(GameObject obj)
    {
        if (obj.transform.name == "fener_mesh")
        {
            obj.transform.parent.position = flashlightObjectHolder.position;
            obj.transform.parent.rotation = flashlightObjectHolder.rotation;
            obj.transform.parent.parent = flashlightObjectHolder;
        }
        else
        {
            obj.transform.parent.position = itemObjectHolder.position;
            obj.transform.parent.rotation = itemObjectHolder.rotation;
            obj.transform.parent.parent = itemObjectHolder;
        }
    }

    public void clearParent(GameObject obj)
    {
        obj.transform.parent.SetParent(null, worldPositionStays: true);
    }
    
    public void markInInventory(GameObject obj)
    {
        if(obj.transform.parent.name == "flashlight")
        {
            FlashlightScript flashlight = obj.transform.parent.GetComponent<FlashlightScript>();
            flashlight.isInInventory = true;
        }
        else if(obj.transform.parent.name == "medicine")
        {
            MedicineScript medicine = obj.transform.parent.GetComponent<MedicineScript>();
            medicine.isInInventory = true;
        }
        else if(obj.transform.parent.name == "battery")
        {
            BatteryScript battery = obj.transform.parent.GetComponent<BatteryScript>();
            battery.isInInventory = true;
        }
    }
    
    public void markNotInInventory(GameObject obj)
    {
        if(obj.transform.parent.name == "flashlight")
        {
            FlashlightScript flashlight = obj.transform.parent.GetComponent<FlashlightScript>();
            flashlight.isInInventory = false;
        }
        else if(obj.transform.parent.name == "medicine")
        {
            MedicineScript medicine = obj.transform.parent.GetComponent<MedicineScript>();
            medicine.isInInventory = false;
        }
        else if(obj.transform.parent.name == "battery")
        {
            BatteryScript battery = obj.transform.parent.GetComponent<BatteryScript>();
            battery.isInInventory = false;
        }
    }
    
    public void camForwardToFlashlightRotation(Transform fTransform)
    {
        Vector3 camForward = cam.transform.forward;
    
        camForward.y = 0;
    
        camForward.Normalize();
    
        fTransform.rotation = Quaternion.LookRotation(camForward);
        
        Vector3 eulerAngles = fTransform.rotation.eulerAngles;
        eulerAngles.x = -90;
        
        fTransform.rotation = Quaternion.Euler(eulerAngles);
    }
    
    //=============================================================================================================== UI
    
    public void updateInventoryPictures(int slot)
    {
        if (inventory[slot] != null)
        {
            if (inventory[slot].transform.parent.name == "flashlight")
            {
                if (slot == 0)
                {
                    inventorySlot1Image.enabled = true;
                    inventorySlot1Image.sprite = flashlightPicture;
                }
                else if (slot == 1)
                {
                    inventorySlot2Image.enabled = true;
                    inventorySlot2Image.sprite = flashlightPicture;
                }
                else if (slot == 2)
                {
                    inventorySlot3Image.enabled = true;
                    inventorySlot3Image.sprite = flashlightPicture;
                }
            }
            else if (inventory[slot].transform.parent.name == "medicine")
            {
                if (slot == 0)
                {
                    inventorySlot1Image.enabled = true;
                    inventorySlot1Image.sprite = medicinePicture;
                }
                else if (slot == 1)
                {
                    inventorySlot2Image.enabled = true;
                    inventorySlot2Image.sprite = medicinePicture;
                }
                else if (slot == 2)
                {
                    inventorySlot3Image.enabled = true;
                    inventorySlot3Image.sprite = medicinePicture;
                }
            }
            else if (inventory[slot].transform.parent.name == "battery")
            {
                if (slot == 0)
                {
                    inventorySlot1Image.enabled = true;
                    inventorySlot1Image.sprite = batteryPicture;
                }
                else if (slot == 1)
                {
                    inventorySlot2Image.enabled = true;
                    inventorySlot2Image.sprite = batteryPicture;
                }
                else if (slot == 2)
                {
                    inventorySlot3Image.enabled = true;
                    inventorySlot3Image.sprite = batteryPicture;
                }
            }
        }
        else
        {
            if (slot == 0)
            {
                inventorySlot1Image.enabled = false;
            }
            else if (slot == 1)
            {
                inventorySlot2Image.enabled = false;
            }
            else if (slot == 2)
            {
                inventorySlot3Image.enabled = false;
            }
        }
    }

    public void setFlashlightBar(int slot)
    {
        if (slot == 0)
        {
            flashlightChargeBar.anchoredPosition = slot1FlashlightBarPosition;
        }
        else if (slot == 1)
        {
            flashlightChargeBar.anchoredPosition = slot2FlashlightBarPosition;
        }
        else if (slot == 2)
        {
            flashlightChargeBar.anchoredPosition = slot3FlashlightBarPosition;
        }
        
        flashlightChargeBar.gameObject.SetActive(true);
    }

    public void hideFlashlightBar()
    {
        flashlightChargeBar.gameObject.SetActive(false);
    }
    
    //========================================================================================================== Rig Set
    public void holdingRigSetZeroCall(Rig rig)
    {
        if (holdingRigWeightSetCoroutine != null)
        {
            StopCoroutine(holdingRigWeightSetCoroutine);
        }
        holdingRigWeightSetCoroutine = StartCoroutine(holdingRigSetZero(rig));
    }
    
    public void holdingRigSetOneCall(Rig rig)
    {
        if (holdingRigWeightSetCoroutine != null)
        {
            StopCoroutine(holdingRigWeightSetCoroutine);
        }
        holdingRigWeightSetCoroutine = StartCoroutine(holdingRigSetOne(rig));
    }

    public void holdingRigResetCall(Rig rig, Rig rig2) // rig -> 1 to 0 || rig2 -> 0 to 1
    {
        if (holdingRigWeightSetCoroutine != null)
        {
            StopCoroutine(holdingRigWeightSetCoroutine);
        }
        holdingRigWeightSetCoroutine = StartCoroutine(holdingRigChange(rig, rig2));
    }

    public IEnumerator holdingRigSetZero(Rig rig)
    {
        float setTime = 0.15f;
        float newWeight = rig.weight;
        while (true)
        {
            newWeight -= Time.deltaTime / setTime;
            
            rig.weight = newWeight;

            if (newWeight <= 0)
            {
                rig.weight = 0;
                break;
            }
            else
            {
                yield return null;
            }
        }
    }
    
    public IEnumerator holdingRigSetOne(Rig rig)
    {
        float setTime = 0.15f;
        float newWeight = rig.weight;
        while (true)
        {
            newWeight += Time.deltaTime / setTime;
            
            rig.weight = newWeight;

            if (newWeight >= 1)
            {
                rig.weight = 1;
                break;
            }
            else
            {
                yield return null;
            }
        }
    }

    public IEnumerator holdingRigChange(Rig rig, Rig rig2) // change rig to rig2 as 1 to 0 => 0 to 1
    {
        float setTime = 0.1f;  // * 2
        float newWeight = rig.weight;
        bool setFirst = false;
        while (true)
        {
            newWeight -= Time.deltaTime / setTime;

            if (!setFirst)
            {
                rig.weight = newWeight;
            }

            if (newWeight <= 0 && !setFirst)
            {
                setFirst = true;
                rig.weight = 0;
            }

            if (setFirst)
            {
                rig2.weight = -newWeight;

                if (-newWeight >= 1)
                {
                    rig2.weight = 1;
                    break;
                }
                
                yield return null;
            }
            
            yield return null;
        }
    }

    public IEnumerator interactionRigPlay()
    {
        float setTime = 0.1f;
        float aimConstraintTime = 0.1f;
        float setTime2 = 0.1f;
        
        float newWeight = 0;
        int stage = 1;
        while (true)
        {
            if (stage == 1)
            {
                newWeight += Time.deltaTime / setTime;
                
                interactionRig.weight = newWeight;
                
                if (newWeight >= 1)
                {
                    newWeight = 0;
                    
                    interactionRig.weight = 1;
                    
                    stage = 2;
                }
                
                yield return null;
            }
            
            if (stage == 2)
            {
                newWeight += Time.deltaTime / aimConstraintTime;
                
                intArm.weight = newWeight;
                intForearm2.weight = newWeight;
                intHand.weight = newWeight;

                if (newWeight > 0.75f)
                {
                    intForearm_second.weight = newWeight;
                }
                
                if (newWeight >= 1)
                {
                    newWeight = 1;
                    
                    intArm.weight = 1;
                    intForearm2.weight = 1;
                    intHand.weight = 1;
                    
                    stage = 3;
                }
                
                yield return null;
            }

            if (stage == 3)
            {
                newWeight -= Time.deltaTime / setTime2;
                
                interactionRig.weight = newWeight;

                if (newWeight <= 0)
                {
                    interactionRig.weight = 0;
                    
                    intArm.weight = 0;
                    intForearm_second.weight = 0.75f;
                    intForearm2.weight = 0;
                    intHand.weight = 0;
                    break;
                }
                
                yield return null;
            }
            
            yield return null;
        }
    }
}
