using UnityEngine;
using UnityEngine.Animations.Rigging;

[DefaultExecutionOrder(1000)]
public class RigBuilderRunner : MonoBehaviour
{
    public RigBuilder rigBuilder;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rigBuilder = GetComponent<RigBuilder>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
