using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float spaceVelocity = 8;
    public LogicScript logicScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logicScript = GameObject.FindGameObjectWithTag("LogicManager").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) && !logicScript.gameIsOver) ||
            (Input.GetKeyDown(KeyCode.UpArrow) && !logicScript.gameIsOver) ||
            (Input.GetKeyDown(KeyCode.W) && !logicScript.gameIsOver))
        {
            rb.linearVelocityY = spaceVelocity;
        }
    }
}
