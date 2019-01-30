using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideThis : Photon.MonoBehaviour
{

	// Use this for initialization
	void Awake () {
        if (!photonView.isMine)
        {
            Destroy(this.gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!photonView.isMine)
        {
            Destroy(this.gameObject);
        }

    }
}
