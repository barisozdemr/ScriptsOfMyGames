using UnityEngine;

public class PipeMovement : MonoBehaviour
{
    public double speed = -2.5f;
    public LogicScript logicScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logicScript = GameObject.FindGameObjectWithTag("LogicManager").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = logicScript.speed;
        
        transform.position += new Vector3((float)speed, 0, 0) * Time.deltaTime;

        if (transform.position.x < -11)
        {
            Destroy(gameObject);
        }
    }
}
