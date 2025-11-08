using UnityEngine;
using UnityEngine.UI;

public class StartTimerScript : MonoBehaviour
{
    public double timer = 0;
    public Text timerText;
    public GameObject player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        bool t2 = true;
        bool t1 = true;
        
        if (timer >= 1 && t2)
        {
            timerText.text = "2";
            t2 = false;
        }
        
        if (timer >= 2 && t1)
        {
            timerText.text = "1";
            t1 = false;
        }

        if (timer >= 3)
        {
            this.gameObject.SetActive(false);
            player.GetComponent<Rigidbody2D>().simulated = true;
            this.enabled = false;
        }
    }
}
