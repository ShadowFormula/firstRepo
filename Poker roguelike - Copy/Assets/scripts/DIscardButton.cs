using UnityEngine;

public class DiscardButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int RemainingDiscards;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDiscardCount()
    {
        RemainingDiscards--;
    }
}
