using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class inventory : MonoBehaviour {
    public bool IsitemInRange;
    public TextMeshProUGUI HelpMessage;
    public GameObject itemInRange;

    public GameObject[] invSlots;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (IsitemInRange)
        {
            HelpMessage.text = "Press E to pick up " + itemInRange.GetComponent<item>().Item.Name;
            if (Input.GetKeyDown(KeyCode.E))
            {
                itemInRange.GetComponent<onPlayerNearItem>().OnPickUp(transform.parent.gameObject);
                
            }
        }
        if (!IsitemInRange)
        {
            HelpMessage.text = "";
        }
	}
}
