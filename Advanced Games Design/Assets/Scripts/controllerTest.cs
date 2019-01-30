using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class controllerTest : NetworkBehaviour {


    private Rigidbody rb;
	// Use this for initialization
	void Start () {
        if(!isLocalPlayer)
        {
            Destroy(this);
            return;
        }
        rb = GetComponent<Rigidbody>();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (isLocalPlayer == true)
        {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            rb.AddForce(movement * 10f);
        }
        
	}
}
