using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reach_Text : MonoBehaviour{

    public int reach = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void WriteReach()
    {
        this.GetComponent<Text>().text = "M " + reach.ToString();
    }
}
