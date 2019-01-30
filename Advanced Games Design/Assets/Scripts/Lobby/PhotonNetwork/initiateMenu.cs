using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initiateMenu : MonoBehaviour {

    public photonConnect phConnect;
    public GameObject MainMenu;
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKey)
        {
            phConnect.StartConnection();
            MainMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
	}
}
