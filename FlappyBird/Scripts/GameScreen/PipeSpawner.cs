using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public LogicScript logicScript;
    public GameObject pipePrefab;
    public double spawnDelay;
    public double timer = 6.9;
    public float spawnHeightOffset = 2.5f;
    
    public List<GameObject> pipes;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logicScript = GameObject.FindGameObjectWithTag("LogicManager").GetComponent<LogicScript>();
        spawnDelay = logicScript.spawnDelay/10;
    }

    // Update is called once per frame
    void Update()
    {
        spawnDelay = logicScript.spawnDelay/10;
        
        if (timer >= spawnDelay)
        {
            spawnPipe();
            timer = 0;
        }
        
        timer += 2 * Time.deltaTime;
    }

    void spawnPipe()
    {
        float lowestY = transform.position.y - spawnHeightOffset;
        float HighestY = transform.position.y + spawnHeightOffset;
        
        float positionY = Random.Range(lowestY, HighestY);
        
        GameObject pipe = Instantiate(pipePrefab, new Vector3(transform.position.x, positionY, 0), transform.rotation);
        
        pipes.Add(pipe);
    }

    public void destroyPipes()
    {
        foreach (GameObject pipe in pipes)
        {
            Destroy(pipe);
        }
    }
}
