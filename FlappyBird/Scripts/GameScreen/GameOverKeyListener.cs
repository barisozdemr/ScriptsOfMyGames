using UnityEngine;

public class GameOverKeyListener : MonoBehaviour
{
    public LogicScript logicScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logicScript = GameObject.FindGameObjectWithTag("LogicManager").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            logicScript.restartGame();
        }
    }
}
