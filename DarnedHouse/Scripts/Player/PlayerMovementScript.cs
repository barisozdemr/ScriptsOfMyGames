using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class PlayerMovementScript : MonoBehaviour
{
    //===================================================================================== Jumping
    public LayerMask groundLayer;
    public Transform headTransform;
    public float headCheckDistance = 0.09f;
    
    public Transform footTransform;
    public float footDistance = 0.07f;
    
    public float groundCheckDistance = 0.09f;
    public float ceilingCheckDistance = 0.08f;
    
    public float jumpFactor = 3f;
    public float jumpVelocity = 0;
    public float gravity = -9.81f;
    public float jumpGravity = -9.81f * 1f;
    
    public bool isJumping = false;
    
    public bool isFalling = false;
    
    //===================================================================================== Movement
    public LayerMask surroundingLayer;
    public float surroundingCheckRadius = 0.4f;
    
    public bool alreadyRunning = false;
    public float stamina;
    public float maxRunningTime = 7;
    
    public RectTransform staminaBar;
    public RectTransform staminaBarInside;
    
    public Transform bodyTransform;  //==================== for player direction
    public float movementSpeed = 2.4f;
    public Transform movementTransform; //================= for collision detection

    public float heightCorrectionDistance = 1.05f;
    
    public Vector3 collisionBoxHalfExtents = new Vector3(0.5f, 0.6f, 0.5f);
    
    public List<Collider> collisions = new List<Collider>();
    
    public Transform cameraHolderTransform;
    public float cameraHolderCrouchDistance = 0.35f;
    
    //public Transform headTransform;
    public float headTransformCrouchDistance = 0.4f;
    
    //public Transform movementTransform;
    public float movementTransformCrouchDistance = 0.2f;
    
    public float crouchSetTime = 0.2f;
    
    public bool crouchSet = false;
    
    public Coroutine crouchingCoroutine;
    
    public bool downCrouchCoroutineDone = true;
    public bool upCrouchCoroutineDone = true;

    public bool upCrouchCoroutineCompleted = true;
    
    public bool upCrouchCoroutineInterrupted = false;
    
    //===================================================================================== Animation
    public Animator playerAnimator;
    public GameObject womanMesh;

    public float womanMeshCrouchingOffsetDistance = 0.3f;
    public Coroutine womanMeshMoveCoroutine;
    public bool womanMeshIsBackwards = false;
    
    public InventoryManagerScript inventoryManager;

    public Transform flashlightHoldingRigTargets;
    public Transform flashlightHoldingRig2Targets;

    public GameObject itemRigLeftArmObject;
    public MultiAimConstraint itemRigLeftArmConstraint;
    public GameObject itemRigLeftForeArmObject;
    public MultiAimConstraint itemRigLeftForeArmConstraint;
    public GameObject itemRigLeftForeArm2Object;
    public MultiAimConstraint itemRigLeftForeArm2Constraint;

    public Coroutine walkingRigWeightSetCoroutine;

    public bool isHoldingItem;
    
    //==========================================
    public int isWalkingStraightHash;
    public int isWalkingBackwardsHash;
    
    public int isWalkingRightwardsHash;
    public int isWalkingLeftwardsHash;
    
    public int isRunningHash;
    
    public int isCrouchingStraightHash;
    public int isCrouchingBackwardsHash;
    
    public int isCrouchingRightwardsHash;
    public int isCrouchingLeftwardsHash;
    
    public int isCrouchingHash;
    
    public int isIdleHash;
    
    //==========================================
    public bool isWalkingStraight = false;
    public bool isWalkingBackwards = false;
    
    public bool isWalkingRightwards = false;
    public bool isWalkingLeftwards = false;
    
    public bool isRunning = false;
    
    public bool isCrouchingStraight = false;
    public bool isCrouchingBackwards = false;
    
    public bool isCrouchingRightwards = false;
    public bool isCrouchingLeftwards = false;
    
    public bool isCrouching = false;
    
    public bool isIdle = false;
    
    //==========================================
    public bool wPressed = false;
    public bool sPressed = false;
    public bool dPressed = false;
    public bool aPressed = false;
    
    public bool ctrlPressed = false;
    
    
    //==================================================================================================================
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        surroundingLayer = LayerMask.GetMask("Default", "OutlineDoor", "InteractiveDoor");
        
        playerAnimator = womanMesh.GetComponent<Animator>();
        
        inventoryManager = GetComponent<InventoryManagerScript>();
        
        itemRigLeftArmConstraint = itemRigLeftArmObject.GetComponent<MultiAimConstraint>();
        itemRigLeftForeArmConstraint = itemRigLeftForeArmObject.GetComponent<MultiAimConstraint>();
        itemRigLeftForeArm2Constraint = itemRigLeftForeArm2Object.GetComponent<MultiAimConstraint>();
        
        isWalkingStraightHash = Animator.StringToHash("isWalkingStraight");
        isWalkingBackwardsHash = Animator.StringToHash("isWalkingBackwards");
        
        isWalkingRightwardsHash = Animator.StringToHash("isWalkingRightwards");
        isWalkingLeftwardsHash = Animator.StringToHash("isWalkingLeftwards");
        
        isRunningHash = Animator.StringToHash("isRunning");
        
        isCrouchingStraightHash = Animator.StringToHash("isCrouchingStraight");
        isCrouchingBackwardsHash = Animator.StringToHash("isCrouchingBackwards");
        
        isCrouchingRightwardsHash = Animator.StringToHash("isCrouchingRightwards");
        isCrouchingLeftwardsHash = Animator.StringToHash("isCrouchingLeftwards");
        
        isCrouchingHash = Animator.StringToHash("isCrouching");
        
        isIdleHash = Animator.StringToHash("isIdle");

        stamina = maxRunningTime;
    }

    void FixedUpdate()
    {
        updateStaminaBar();
    }

    public void updateStaminaBar()
    {
        float ratio = stamina / maxRunningTime;
        
        staminaBarInside.localScale = new Vector3(ratio, 1, 1);
        
        Vector2 pos = staminaBarInside.anchoredPosition;
        pos.x = (ratio - 1) * 100;
        staminaBarInside.anchoredPosition = pos;
    }

    // Update is called once per frame
    void Update()
    {
        //=============================================================================== Movement & Animation
        Vector3 forward = bodyTransform.forward;
        Vector3 right = bodyTransform.right;

        Vector3 movement = Vector3.zero;
            
        if (Input.GetKey(KeyCode.W))  // W
        {
            wPressed = true;
            if (!isCollisionDetected())
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    movement += forward * 1.7f;
                }
                else
                {
                    movement += forward;
                }
            }
            else
            {
                movement += movementClip(forward);
            }
        }
        else if(wPressed)
        {
            wPressed = false;
        }
        
        if (Input.GetKey(KeyCode.S) && !wPressed)  // S
        {
            sPressed = true;
            if(!isCollisionDetected())
            {
                movement -= forward/2;
            }
            else
            {
                movement += movementClip(-forward)/2;
            }
        }
        else if(sPressed)
        {
            sPressed = false;
        }
        
        
        if (Input.GetKey(KeyCode.D) && !aPressed)  // D
        {
            dPressed = true;
            if(!isCollisionDetected())
            {
                movement += right;
            }
            else
            {
                movement += movementClip(right);
            }
        }
        else if(dPressed)
        {
            dPressed = false;
        }
        
        
        if (Input.GetKey(KeyCode.A) && !dPressed)  // A
        {
            aPressed = true;
            if(!isCollisionDetected())
            {
                movement -= right;
            }
            else
            {
                movement += movementClip(-right);
            }
        }
        else if(aPressed)
        {
            aPressed = false;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            ctrlPressed = true;
        }
        else
        {
            ctrlPressed = false;
        }

        //==================================================================================================== Animation
        if (movement != Vector3.zero)
        {
            isIdle = false;
            playerAnimator.SetBool(isIdleHash, false);
            
            isCrouching = false;
            playerAnimator.SetBool(isCrouchingHash, false);
            
            if (ctrlPressed)
            {
                transform.position += Vector3.ClampMagnitude(movement, 0.5f) * (movementSpeed * Time.deltaTime);
                
                crouch();
                
                if (stamina < maxRunningTime)
                {
                    stamina += Time.deltaTime;
                }
                
                bool g = true;
                if(wPressed)
                {
                    if (!isCrouchingStraight)
                    {
                        isCrouchingStraight = true;
                        playerAnimator.SetBool(isCrouchingStraightHash, true);
                    }
                    g = false;
                }
                else
                {
                    if (isCrouchingStraight)
                    {
                        isCrouchingStraight = false;
                        playerAnimator.SetBool(isCrouchingStraightHash, false);
                    }
                }
                
                if(sPressed && g)
                {
                    if (!isCrouchingBackwards)
                    {
                        isCrouchingBackwards = true;
                        playerAnimator.SetBool(isCrouchingBackwardsHash, true);
                    }
                    g = false;
                }
                else
                {
                    if (isCrouchingBackwards)
                    {
                        isCrouchingBackwards = false;
                        playerAnimator.SetBool(isCrouchingBackwardsHash, false);
                    }
                }
                
                if(dPressed && g)
                {
                    if (!isCrouchingRightwards)
                    {
                        isCrouchingRightwards = true;
                        playerAnimator.SetBool(isCrouchingRightwardsHash, true);
                    }
                    g = false;
                }
                else
                {
                    if (isCrouchingRightwards)
                    {
                        isCrouchingRightwards = false;
                        playerAnimator.SetBool(isCrouchingRightwardsHash, false);
                    }
                }
                
                if(aPressed && g)
                {
                    if (!isCrouchingLeftwards)
                    {
                        isCrouchingLeftwards = true;
                        playerAnimator.SetBool(isCrouchingLeftwardsHash, true);
                    }
                    g = false;
                }
                else
                {
                    if (isCrouchingLeftwards)
                    {
                        isCrouchingLeftwards = false;
                        playerAnimator.SetBool(isCrouchingLeftwardsHash, false);
                    }
                }
                
                isWalkingStraight = false;
                playerAnimator.SetBool(isWalkingStraightHash,false);
                isWalkingBackwards = false;
                playerAnimator.SetBool(isWalkingBackwardsHash,false);
            
                isWalkingRightwards = false;
                playerAnimator.SetBool(isWalkingRightwardsHash,false);
                isWalkingLeftwards = false;
                playerAnimator.SetBool(isWalkingLeftwardsHash,false);
            }
            else // ctrl not pressed
            {
                uncrouch();
                
                ctrlPressed = false;
            
                if (Input.GetKey(KeyCode.LeftShift) && wPressed) //========== Running Control
                {
                    if (upCrouchCoroutineCompleted)
                    {
                        if (alreadyRunning && stamina > 0)
                        {
                            transform.position += Vector3.ClampMagnitude(movement, 1.7f) * (movementSpeed * Time.deltaTime);
                            
                            if (stamina > 0)
                            {
                                stamina -= Time.deltaTime;
                            }
                            
                            isWalkingStraight = false;
                            playerAnimator.SetBool(isWalkingStraightHash,false);
                    
                            isRunning = true;
                            playerAnimator.SetBool(isRunningHash,true);
                        }
                        else if(stamina > 2)
                        {
                            transform.position += Vector3.ClampMagnitude(movement, 1.7f) * (movementSpeed * Time.deltaTime);
                            alreadyRunning = true;

                            if (stamina > 0)
                            {
                                stamina -= Time.deltaTime;
                            }
                            
                            isWalkingStraight = false;
                            playerAnimator.SetBool(isWalkingStraightHash,false);
                    
                            isRunning = true;
                            playerAnimator.SetBool(isRunningHash,true);
                        }
                        else
                        {
                            if (stamina < maxRunningTime)
                            {
                                stamina += Time.deltaTime;
                            }
                            
                            transform.position += Vector3.ClampMagnitude(movement, 1f) * (movementSpeed * Time.deltaTime);
                            alreadyRunning = false;
                            
                            isRunning = false;
                            playerAnimator.SetBool(isRunningHash,false);
                            
                            isWalkingStraight = true;
                            playerAnimator.SetBool(isWalkingStraightHash,true);
                        }
                    }
                    else
                    {
                        transform.position += Vector3.ClampMagnitude(movement, 0.5f) * (movementSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    alreadyRunning = false;
                    
                    if (stamina < maxRunningTime)
                    {
                        stamina += Time.deltaTime;
                    }
                    
                    if (upCrouchCoroutineCompleted)
                    {
                        transform.position += Vector3.ClampMagnitude(movement, 1) * (movementSpeed * Time.deltaTime);
                    }
                    else
                    {
                        transform.position += Vector3.ClampMagnitude(movement, 0.5f) * (movementSpeed * Time.deltaTime);
                    }
                    
                    bool g = true;
                    if (wPressed)  //============================================= W
                    {
                        if (!isWalkingStraight)
                        {
                            isWalkingStraight = true;
                            playerAnimator.SetBool(isWalkingStraightHash,true);
                            isRunning = false;
                            playerAnimator.SetBool(isRunningHash,false);
                        }
                        g = false;
                    }
                    else
                    {
                        if (isWalkingStraight)
                        {
                            isWalkingStraight = false;
                            playerAnimator.SetBool(isWalkingStraightHash,false);
                        }
                    }
                    
                    if (sPressed && g) //============================================= S
                    {
                        if (!isWalkingBackwards)
                        {
                            isWalkingBackwards = true;
                            playerAnimator.SetBool(isWalkingBackwardsHash,true);
                        }
                        g = false;
                    }
                    else
                    {
                        if (isWalkingBackwards)
                        {
                            isWalkingBackwards = false;
                            playerAnimator.SetBool(isWalkingBackwardsHash,false);
                        }
                    }

                    if (dPressed && g) //============================================= D
                    {
                        if (!isWalkingRightwards)
                        {
                            isWalkingRightwards = true;
                            playerAnimator.SetBool(isWalkingRightwardsHash,true);
                        }
                        g = false;
                    }
                    else
                    {
                        if (isWalkingRightwards)
                        {
                            isWalkingRightwards = false;
                            playerAnimator.SetBool(isWalkingRightwardsHash,false);
                        }
                    }

                    if (aPressed && g) //============================================= A
                    {
                        if (!isWalkingLeftwards)
                        {
                            isWalkingLeftwards = true;
                            playerAnimator.SetBool(isWalkingLeftwardsHash,true);
                        }
                    }
                    else
                    {
                        if (isWalkingLeftwards)
                        {
                            isWalkingLeftwards = false;
                            playerAnimator.SetBool(isWalkingLeftwardsHash,false);
                        }
                    }
                }
                
                isCrouchingStraight = false;
                playerAnimator.SetBool(isCrouchingStraightHash,false);
                isCrouchingBackwards = false;
                playerAnimator.SetBool(isCrouchingBackwardsHash,false);
                            
                isCrouchingRightwards = false;
                playerAnimator.SetBool(isCrouchingRightwardsHash,false);
                isCrouchingLeftwards = false;
                playerAnimator.SetBool(isCrouchingLeftwardsHash,false);
            }
            
            if (!isJumping && !isFalling)
            {
                heightCorrection();
            }
        }
        else // movement == 0
        {
            if (stamina < maxRunningTime)
            {
                stamina += Time.deltaTime;
            }
            
            isWalkingStraight = false;
            playerAnimator.SetBool(isWalkingStraightHash,false);
            isWalkingBackwards = false;
            playerAnimator.SetBool(isWalkingBackwardsHash,false);
            
            isWalkingRightwards = false;
            playerAnimator.SetBool(isWalkingRightwardsHash,false);
            isWalkingLeftwards = false;
            playerAnimator.SetBool(isWalkingLeftwardsHash,false);
            
            isRunning = false;
            playerAnimator.SetBool(isRunningHash,false);
            
            isCrouchingStraight = false;
            playerAnimator.SetBool(isCrouchingStraightHash,false);
            isCrouchingBackwards = false;
            playerAnimator.SetBool(isCrouchingBackwardsHash,false);
            
            isCrouchingRightwards = false;
            playerAnimator.SetBool(isCrouchingRightwardsHash,false);
            isCrouchingLeftwards = false;
            playerAnimator.SetBool(isCrouchingLeftwardsHash,false);
            
            if(ctrlPressed)
            {
                crouch();
                
                isIdle = false;
                playerAnimator.SetBool(isIdleHash,false);
                
                isCrouching = true;
                playerAnimator.SetBool(isCrouchingHash,true);
            }
            else
            {
                uncrouch();

                if (upCrouchCoroutineCompleted)
                {
                    isCrouching = false;
                    playerAnimator.SetBool(isCrouchingHash,false);
                
                    isIdle = true;
                    playerAnimator.SetBool(isIdleHash,true);
                }
            }
        }

        if (!isCrouching && !isCrouchingStraight && !isCrouchingBackwards && !isCrouchingRightwards && !isCrouchingLeftwards)
        {
            if (womanMeshIsBackwards)
            {
                if (womanMeshMoveCoroutine != null)
                {
                    StopCoroutine(womanMeshMoveCoroutine);
                }
            
                womanMeshMoveCoroutine = StartCoroutine(womanMeshForwCrouchOffset());
            }
        }
        else
        {
            if (!womanMeshIsBackwards)
            {
                if (womanMeshMoveCoroutine != null)
                {
                    StopCoroutine(womanMeshMoveCoroutine);
                }
            
                womanMeshMoveCoroutine = StartCoroutine(womanMeshBackCrouchOffset());
            }
        }

        if (inventoryManager.isHoldingItem) // itemHoldingRig Constraint Change
        {
            if (!isIdle)
            {
                walkingRigWeightSetZeroCall();
            }
            else
            {
                walkingRigWeightSetOneCall();
            }
        }
        //==============================================================================================================
        //========================================================= Jumping
        if (Input.GetKey(KeyCode.Space) && isGrounded())
        {
            isJumping = true;
            StartCoroutine(jump());
        }

        if (!isJumping && !isFalling && !isGrounded())
        {
            isFalling = true;
            StartCoroutine(fall());
        }
    }
    //==================================================================================================== End of Update

    //========================================================================================================= Movement
    Vector3 movementClip(Vector3 movement)
    {
        foreach (Collider col in collisions)
        {
            Vector3 closestPoint = col.ClosestPoint(movementTransform.position);
            
            Vector3 normal = (movementTransform.position - closestPoint).normalized;
            
            normal.y = 0;
            
            normal.Normalize();
            
            if (normal.sqrMagnitude > 0.0001f)
            {
                float dot = Vector3.Dot(movement, normal);

                if (dot < 0)
                {
                    movement = Vector3.ProjectOnPlane(movement, normal);
                }
            }
        }
        
        collisions.Clear();

        return movement;
    }

    bool isCollisionDetected()
    {
        Vector3 colliderCenter = movementTransform.position;
        
        Collider[] hits = Physics.OverlapBox(colliderCenter, collisionBoxHalfExtents, Quaternion.identity, surroundingLayer);
        if (hits.Length > 0)
        {
            collisions = hits.ToList();
            return true;
        }

        return false;
    }
    
    //========================================================================================================== Jumping
    bool isGrounded()
    {
        return Physics.Raycast(footTransform.position + new Vector3(0.08f, 0f, 0.08f), Vector3.down, groundCheckDistance) ||
               Physics.Raycast(footTransform.position + new Vector3(-0.08f, 0f, 0.08f), Vector3.down, groundCheckDistance) ||
               Physics.Raycast(footTransform.position + new Vector3(0.08f, 0f, -0.08f), Vector3.down, groundCheckDistance) ||
               Physics.Raycast(footTransform.position + new Vector3(-0.08f, 0f, -0.08f), Vector3.down, groundCheckDistance);
    }

    bool IsHeadTouchingCeiling()
    {
        return Physics.Raycast(headTransform.position + new Vector3(0.15f, 0f, 0.15f), Vector3.up, ceilingCheckDistance) ||
               Physics.Raycast(headTransform.position + new Vector3(-0.15f, 0f, 0.15f), Vector3.up, ceilingCheckDistance) ||
               Physics.Raycast(headTransform.position + new Vector3(0.15f, 0f, -0.15f), Vector3.up, ceilingCheckDistance) ||
               Physics.Raycast(headTransform.position + new Vector3(-0.15f, 0f, -0.15f), Vector3.up, ceilingCheckDistance);
    }

    public IEnumerator jump()
    {
        jumpVelocity = jumpFactor;
        
        while (true)
        {
            Vector3 movement = Vector3.zero;
            
            movement.y = jumpVelocity;
            
            transform.position += movement * Time.deltaTime;
            
            jumpVelocity += jumpGravity * Time.deltaTime;

            if (IsHeadTouchingCeiling() && jumpVelocity > 0)
            {
                jumpVelocity = 0;
            }
            
            if (isGrounded() && jumpVelocity <= 0)
            {
                heightCorrection();
                isJumping = false;
                break;
            }
            else
            {
                yield return null;
            }
        }
    }
    
    public IEnumerator fall()
    {
        float fallingVelocity = 0;
        
        while (true)
        {
            Vector3 movement = Vector3.zero;
            
            fallingVelocity += jumpGravity * Time.deltaTime;
            
            movement.y = fallingVelocity;
            
            transform.position += movement * Time.deltaTime;
            
            if (isGrounded())
            {
                heightCorrection();
                isFalling = false;
                break;
            }
            else
            {
                yield return null;
            }
        }
    }

    void heightCorrection()
    {
        RaycastHit hit;

        if (Physics.Raycast(movementTransform.position, Vector3.down, out hit, heightCorrectionDistance, surroundingLayer))
        {
            float distance = footTransform.position.y - hit.point.y;
            float yLevel = footDistance - distance;
        
            Vector3 pos = transform.position;
            pos.y += yLevel;
            transform.position = pos;
        }
    }
    
    //======================================================================================================== Crouching

    void crouch()
    {
        if (!crouchSet)
        {
            crouchSet = true;
            if (!upCrouchCoroutineDone)
            {
                StopCoroutine(crouchingCoroutine);
            }
            crouchingCoroutine = StartCoroutine(downCrouchTransforms());
        }
    }
    
    void uncrouch()
    {
        if (crouchSet)
        {
            crouchSet = false;
            if (!downCrouchCoroutineDone)
            {
                StopCoroutine(crouchingCoroutine);
            }
            crouchingCoroutine = StartCoroutine(upCrouchTransforms());
        }
        else
        {
            if (upCrouchCoroutineInterrupted)
            {
                crouchingCoroutine = StartCoroutine(upCrouchTransforms());
            }
        }
    }

    public IEnumerator downCrouchTransforms()
    {
        downCrouchCoroutineDone = false;
        
        while (true)
        {
            cameraHolderTransform.position += Vector3.down * (cameraHolderCrouchDistance * Time.deltaTime / crouchSetTime);
            headTransform.position += Vector3.down * (headTransformCrouchDistance * Time.deltaTime / crouchSetTime);
            movementTransform.position += Vector3.down * (movementTransformCrouchDistance * Time.deltaTime / crouchSetTime);
            
            Vector3 newPos = Vector3.down * (cameraHolderCrouchDistance * 0.75f * Time.deltaTime / crouchSetTime);
            
            flashlightHoldingRigTargets.position += newPos;
            flashlightHoldingRig2Targets.position += newPos;

            heightCorrectionDistance -= (movementTransformCrouchDistance * Time.deltaTime / crouchSetTime);

            collisionBoxHalfExtents.y -= (movementTransformCrouchDistance * Time.deltaTime / crouchSetTime) / 2;

            if (headTransform.localPosition.y <= 0.4)
            {
                Vector3 pos = cameraHolderTransform.localPosition;
                pos.y = 0.3f;
                cameraHolderTransform.localPosition = pos;
                
                pos = headTransform.localPosition;
                pos.y = 0.4f;
                headTransform.localPosition = pos;

                pos = movementTransform.localPosition;
                pos.y = 0f;
                movementTransform.localPosition = pos;
                
                pos = flashlightHoldingRigTargets.localPosition;
                pos.y = 1.02608f;
                flashlightHoldingRigTargets.localPosition = pos;
                
                pos = flashlightHoldingRig2Targets.localPosition;
                pos.y = -0.2625f;
                flashlightHoldingRig2Targets.localPosition = pos;
                
                heightCorrectionDistance = 0.85f;
                
                collisionBoxHalfExtents = new Vector3(0.5f, 0.4f ,0.5f);

                downCrouchCoroutineDone = true;

                break;
            }
            
            yield return null;
        }
    }
    
    public IEnumerator upCrouchTransforms()
    {
        upCrouchCoroutineDone = false;
        upCrouchCoroutineInterrupted = false;
        upCrouchCoroutineCompleted = false;
        
        while (true)
        {
            if (IsHeadTouchingCeiling())
            {
                upCrouchCoroutineInterrupted = true;
                upCrouchCoroutineDone = true;
                break;
            }
            
            cameraHolderTransform.position += Vector3.up * (cameraHolderCrouchDistance * Time.deltaTime / crouchSetTime);
            headTransform.position += Vector3.up * (headTransformCrouchDistance * Time.deltaTime / crouchSetTime);
            movementTransform.position += Vector3.up * (movementTransformCrouchDistance * Time.deltaTime / crouchSetTime);

            Vector3 newPos = Vector3.up * (cameraHolderCrouchDistance * 0.75f * Time.deltaTime / crouchSetTime);
            
            flashlightHoldingRigTargets.position += newPos;
            flashlightHoldingRig2Targets.position += newPos;
            
            heightCorrectionDistance += (movementTransformCrouchDistance * Time.deltaTime / crouchSetTime);
            
            collisionBoxHalfExtents.y += (movementTransformCrouchDistance * Time.deltaTime / crouchSetTime) / 2;

            if (headTransform.localPosition.y >= 0.8)
            {
                Vector3 pos = cameraHolderTransform.localPosition;
                pos.y = 0.65f;
                cameraHolderTransform.localPosition = pos;
                
                pos = headTransform.localPosition;
                pos.y = 0.8f;
                headTransform.localPosition = pos;
                
                pos = movementTransform.localPosition;
                pos.y = 0.2f;
                movementTransform.localPosition = pos;
                
                pos = flashlightHoldingRigTargets.localPosition;
                pos.y = 1.28858f;
                flashlightHoldingRigTargets.localPosition = pos;
                
                pos = flashlightHoldingRig2Targets.localPosition;
                pos.y = 0f;
                flashlightHoldingRig2Targets.localPosition = pos;

                heightCorrectionDistance = 1.05f;

                collisionBoxHalfExtents = new Vector3(0.5f, 0.6f ,0.5f);
                
                upCrouchCoroutineDone = true;
                
                upCrouchCoroutineCompleted = true;
                
                break;
            }
            
            yield return null;
        }
    }
    
    public IEnumerator womanMeshBackCrouchOffset()
    {
        womanMeshIsBackwards = true;
        
        while (true)
        {
            Vector3 direction = (2 * bodyTransform.forward + bodyTransform.right).normalized;
            
            womanMesh.transform.position -= direction * (womanMeshCrouchingOffsetDistance * Time.deltaTime / crouchSetTime);

            if (womanMesh.transform.localPosition.z <= -womanMeshCrouchingOffsetDistance)
            {
                Vector3 pos = womanMesh.transform.localPosition;
                pos.z = -womanMeshCrouchingOffsetDistance;
                pos.x = -womanMeshCrouchingOffsetDistance / 2;
                womanMesh.transform.localPosition = pos;

                break;
            }

            yield return null;
        }
    }

    public IEnumerator womanMeshForwCrouchOffset()
    {
        womanMeshIsBackwards = false;
        
        while (true)
        {
            Vector3 direction = ( 2 * bodyTransform.forward + bodyTransform.right).normalized;
            
            womanMesh.transform.position += direction * (womanMeshCrouchingOffsetDistance * Time.deltaTime / crouchSetTime);

            if (womanMesh.transform.localPosition.z >= 0)
            {
                Vector3 pos = womanMesh.transform.localPosition;
                pos.z = 0;
                pos.x = 0;
                womanMesh.transform.localPosition = pos;

                break;
            }
            
            yield return null;
        }
    }
    
    //======================================================================================================== Animation

    void walkingRigWeightSetZeroCall()
    {
        if (walkingRigWeightSetCoroutine != null)
        {
            StopCoroutine(walkingRigWeightSetCoroutine);
        }
        walkingRigWeightSetCoroutine = StartCoroutine(walkingRigWeightSetZero());
    }

    void walkingRigWeightSetOneCall()
    {
        if (walkingRigWeightSetCoroutine != null)
        {
            StopCoroutine(walkingRigWeightSetCoroutine);
        }
        walkingRigWeightSetCoroutine = StartCoroutine(walkingRigWeightSetOne());
    }

    public IEnumerator walkingRigWeightSetZero()
    {
        float setTime = 0.15f;
        float newWeight = itemRigLeftArmConstraint.weight;
        while (true)
        {
            newWeight -= Time.deltaTime / setTime;
            
            itemRigLeftArmConstraint.weight = newWeight;
            itemRigLeftForeArmConstraint.weight = newWeight;
            itemRigLeftForeArm2Constraint.weight = newWeight;

            if (newWeight < 0)
            {
                itemRigLeftArmConstraint.weight = 0;
                itemRigLeftForeArmConstraint.weight = 0;
                itemRigLeftForeArm2Constraint.weight = 0;
                break;
            }
            else
            {
                yield return null;
            }
        }
    }
    
    public IEnumerator walkingRigWeightSetOne()
    {
        float setTime = 0.15f;
        float newWeight = itemRigLeftArmConstraint.weight;
        while (true)
        {
            newWeight += Time.deltaTime / setTime;
            
            itemRigLeftArmConstraint.weight = newWeight;
            itemRigLeftForeArmConstraint.weight = newWeight;
            itemRigLeftForeArm2Constraint.weight = newWeight;

            if (newWeight > 1)
            {
                itemRigLeftArmConstraint.weight = 1;
                itemRigLeftForeArmConstraint.weight = 1;
                itemRigLeftForeArm2Constraint.weight = 1;
                break;
            }
            else
            {
                yield return null;
            }
        }
    }
}