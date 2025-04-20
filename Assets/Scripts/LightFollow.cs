using UnityEngine;

public class LightFollow : MonoBehaviour
{
    public GameObject thePlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(thePlayer.transform);
    }
}
